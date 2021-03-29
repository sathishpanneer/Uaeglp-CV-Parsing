using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using File = Uaeglp.Models.File;
using Profile = Uaeglp.Models.Profile;

namespace Uaeglp.Services
{
    public class LeadershipPointSystemService : ILeadershipPointSystemService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly FileDbContext _fileDbContext;

        public LeadershipPointSystemService(IMapper mapper, AppDbContext dbContext, FileDbContext fileDbContext)
        {
            _mapper = mapper;
            _appDbContext = dbContext;
            _fileDbContext = fileDbContext;
        }


        public async Task<ILeadershipPointSystemResponse> GetLeadershipPointSystemAsync(int profileId)
        {

            try
            {

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId);

                if (profile == null)
                {
                    return new LeadershipPointSystemResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var badges = _appDbContext.Badges.Where(k => !k.IsDeleted).OrderBy(k => k.MinimumPoints).ToList();
                var criteria = _appDbContext.Criteria.Include(k=>k.CriteriaCategory).Where(k => !k.IsDeleted).OrderBy(k => k.Points).ToList();

                var claimPoints = await _appDbContext.CriteriaClaims.Include(y => y.Criteria).Include(y => y.Criteria.CriteriaCategory)
                    .Where(k => k.ProfileId == profileId && k.StatusId == (int)ClaimStatusType.Accepted && !k.IsDeleted).SumAsync(k => k.Criteria.Points);

                var model = new LeadershipPointSystemView
                {
                    ProfileId = profile.Id,
                    CurrentPoints = profile.Lpspoints,
                    Badges = _mapper.Map<List<BadgeView>>(badges),
                    CriteriaList = _mapper.Map<List<CriteriaView>>(criteria)
                };

                profile.Lpspoints = claimPoints;

                await _appDbContext.SaveChangesAsync();


                return new LeadershipPointSystemResponse(model);
            }
            catch (Exception e)
            {
                return new LeadershipPointSystemResponse(e);
            }

        }

        public async Task<ILeadershipPointSystemResponse> GetCriteriaClaimedPointsAsync(int profileId, int criteriaId)
        {

            try
            {

                var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId);

                if (profile == null)
                {
                    return new LeadershipPointSystemResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var criteriaClaims = await _appDbContext.CriteriaClaims.Include(y => y.Criteria).Include(k=>k.Status)
                    .Where(k => k.ProfileId == profileId && !k.IsDeleted && k.Criteria.Id == criteriaId).ToListAsync();


                var earnedPoints = criteriaClaims.Where(k => k.StatusId == (int)ClaimStatusType.Accepted && !k.IsDeleted).Sum(k => k.Criteria.Points);


                var model = new CriteriaClaimedPointsView()
                {
                    ProfileId = profile.Id,
                    CriteriaId = criteriaId,
                    CriteriaClaimedPoints = earnedPoints,
                    TotalClaimedPoints = profile.Lpspoints,
                    ClaimedList = _mapper.Map<List<CriteriaClaimView>>(criteriaClaims.Where(k => k.StatusId == (int)ClaimStatusType.Accepted && !k.IsDeleted).ToList()),
                    PendingList = _mapper.Map<List<CriteriaClaimView>>(criteriaClaims.Where(k => k.StatusId == (int)ClaimStatusType.Pending && !k.IsDeleted).ToList())
                };

                return new LeadershipPointSystemResponse(model);
            }
            catch (Exception e)
            {
                return new LeadershipPointSystemResponse(e);
            }

        }

        public async Task<ILeadershipPointSystemResponse> GetCriteriaMoreDetailsAsync(Guid? correlationId)
        {

            try
            {

                var files = await _appDbContext.Files.Where(k => k.CorrelationId == correlationId).ToListAsync();


                var fileView = _mapper.Map<List<FileView>>(files);

                return new LeadershipPointSystemResponse(fileView);
            }
            catch (Exception e)
            {
                return new LeadershipPointSystemResponse(e);
            }

        }

        public async Task<ILeadershipPointSystemResponse> AddCriteriaClaimAsync(CriteriaClaimRequestView model)
        {

            try
            {

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.UserId == model.ProfileId);

                if (userInfo == null)
                {
                    return new LeadershipPointSystemResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }


                var criteria = await _appDbContext.Criteria.FirstOrDefaultAsync(k => k.Id == model.CriteriaId);

                if (criteria == null)
                {
                    return new LeadershipPointSystemResponse(ClientMessageConstant.FileNotFound, HttpStatusCode.NotFound);
                }

                model.CorrelationId = Guid.NewGuid();

                await SaveFile(model, userInfo);

                var orderNumber = _appDbContext.CriteriaClaims
                    .Where(k => k.ProfileId == model.ProfileId && k.CriteriaId == model.CriteriaId)
                    .OrderBy(k => k.Order).LastOrDefault()?.Order ?? 0;


                var claim = new CriteriaClaim()
                {
                    ProfileId = model.ProfileId,
                    RequestDate = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    CorrelationId = model.CorrelationId,
                    CriteriaId = criteria.Id,
                    StatusId = criteria.RequiresApproval ? (int)ClaimStatusType.Pending : (int)ClaimStatusType.Accepted,
                    IsDeleted = false,
                    Created = DateTime.UtcNow,
                    ModifiedBy = userInfo.Email,
                    CreatedBy = userInfo.Email,
                    Details = model.Label,
                    Order = orderNumber + 1
                };


                await _appDbContext.CriteriaClaims.AddAsync(claim);
                await _appDbContext.SaveChangesAsync();

                await UpdateLPSPointsAsync(model);

                var criteriaClaimed = await GetCriteriaClaimedPointsAsync(model.ProfileId, model.CriteriaId);

                return new LeadershipPointSystemResponse(criteriaClaimed.CriteriaClaimedPointsView);
            }
            catch (Exception e)
            {
                return new LeadershipPointSystemResponse(e);
            }

        }

        private async Task UpdateLPSPointsAsync(CriteriaClaimRequestView model)
        {
            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == model.ProfileId);

            var criteriaClaims = await _appDbContext.CriteriaClaims.Include(y => y.Criteria)
                .Where(k => k.ProfileId == model.ProfileId && !k.IsDeleted).ToListAsync();


            var earnedPoints = criteriaClaims.Where(k => k.StatusId == (int) ClaimStatusType.Accepted && !k.IsDeleted)
                .Sum(k => k.Criteria.Points);

            profile.Lpspoints = earnedPoints;

            await _appDbContext.SaveChangesAsync();
        }

        private async Task SaveFile(CriteriaClaimRequestView model, UserInfo userInfo)
        {
            foreach (var formFile in model.AttachmentFile)
            {
                if (formFile == null) { continue; }
                var data = new File()
                {
                    IdGuid = Guid.NewGuid(),
                    CorrelationId = model.CorrelationId,
                    SizeMb = GetFileSize(formFile.Length),
                    Name = formFile.FileName,
                    ProviderName = "SqlProvider",
                    Created = DateTime.UtcNow,
                    MimeType = formFile.ContentType,
                    Modified = DateTime.UtcNow,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                await _appDbContext.Files.AddAsync(data);
                await _appDbContext.SaveChangesAsync();

                var fileDb = new FileDB()
                {
                    Id = data.IdGuid,
                    Bytes = StreamToBytes(formFile.OpenReadStream())
                };

                await _fileDbContext.FileDB.AddAsync(fileDb);
                await _fileDbContext.SaveChangesAsync();
            }

        }

        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private decimal GetFileSize(long length)
        {
            if (length == 0L)
                return decimal.Zero;
            decimal num = Convert.ToDecimal(Math.Pow(1024.0, 2.0));
            return Math.Round(Convert.ToDecimal(length) / num, 3);
        }


    }
}
