using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;

namespace Uaeglp.Services
{
    public class ReportProblemService : IReportProblemService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly FileDbContext _fileDbContext;
        private readonly IUserIPAddress _userIPAddress;

        public ReportProblemService(AppDbContext appDbContext, IMapper mapper, IEmailService emailService, IEncryptionManager encryption, FileDbContext fileDbContext, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _emailService = emailService;
            _fileDbContext = fileDbContext;
            _userIPAddress = userIPAddress;
        }

        public async Task<IReportProblemResponse> ReportProblemAsync(ReportProblemView view)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {view.ToJsonString()} UserIPAddress: { _userIPAddress.GetUserIP().Result }");

                var email = await _appDbContext.UserInfos.Where(k => k.UserId == view.UserID).Select(k => k.Email).FirstOrDefaultAsync();

                if (email == null) return new ReportProblemResponse(ClientMessageConstant.UserNotFound, HttpStatusCode.NotFound);

                var firstName = await _appDbContext.Profiles.Where(k => k.Id == view.UserID).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == view.UserID).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;

                var data = new ReportProblem()
                {
                    UserID = view.UserID,
                    ReportDescription = view.ReportDescription,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = userName,
                    ModifiedBy = userName,
                };

                if(view.ReportFile != null)
                {
                    data.ReportFileID = (await SaveFileAsync(view.ReportFile, email)).Id;
                }

                await _appDbContext.ReportProblems.AddAsync(data);
                await _appDbContext.SaveChangesAsync();

                await _emailService.SendReportProblemEmailAsync(view.ReportDescription, email, userName);

                var reportProblem = _mapper.Map<ReportProblemModelView>(data);

                return new ReportProblemResponse(reportProblem);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }

        }

        private async Task<File> SaveFileAsync(IFormFile file, string email)
        {
            var data = new File()
            {
                IdGuid = Guid.NewGuid(),
                SizeMb = file.Length.ToFileMB(),
                Name = file.FileName,
                ProviderName = "SqlProvider",
                ExtraParams = "",
                Created = DateTime.UtcNow,
                MimeType = file.ContentType,
                Modified = DateTime.UtcNow,
                CreatedBy = email != null ? email : "",
                ModifiedBy = email != null ? email : ""
            };

            var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;
            await UploadIntoFileDbAsync(savedEntity.IdGuid, file);
            await _appDbContext.SaveChangesAsync();
            return savedEntity;
        }

        private async Task UploadIntoFileDbAsync(Guid id, IFormFile formFile)
        {
            var fileDb = new FileDB()
            {
                Id = id,
                Bytes = formFile.OpenReadStream().ToBytes()
            };

            await _fileDbContext.FileDB.AddAsync(fileDb);
            await _fileDbContext.SaveChangesAsync();
        }
    }
}
