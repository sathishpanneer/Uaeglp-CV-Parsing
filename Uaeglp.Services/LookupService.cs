using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services
{
    public class LookupService : ILookupService
    {
        private readonly AppDbContext _appContext;
        private readonly IMapper _mapper;
        public LookupService(AppDbContext appContext, IMapper mapper)
        {
            _appContext = appContext;
            _mapper = mapper;
        }

        public async Task<ILookupResponse> GetAllLookupItemsAsync(DateTime FromDate)
        {
            try
            {
                var lookupItems = await _appContext.LookupItems.Where(k => k.Modified >= FromDate).ToListAsync();
                if(lookupItems.Count == 0)
                {
                    lookupItems = await _appContext.LookupItems.Where(k => k.Created >= FromDate).ToListAsync();
                }

                var educationFieldOfStudy = await _appContext.ProfileEducationFieldOfStudys.Where(k => k.Modified >= FromDate).OrderBy(k => k.TitleEn).ToListAsync();
                if(educationFieldOfStudy.Count == 0)
                {
                    educationFieldOfStudy = await _appContext.ProfileEducationFieldOfStudys.Where(k => k.Created >= FromDate).OrderBy(k => k.TitleEn).ToListAsync();
                }

                var workExperienceJobTitle = await _appContext.ProfileWorkExperienceJobTitle.Where(k => k.Modified >= FromDate).OrderBy(k => k.TitleEn).ToListAsync();
                if (workExperienceJobTitle.Count == 0)
                {
                    workExperienceJobTitle=  await _appContext.ProfileWorkExperienceJobTitle.Where(k => k.Created >= FromDate).OrderBy(k => k.TitleEn).ToListAsync();
                }

                var workField = await _appContext.WorkFields.Where(k => k.Modified >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                if (workField.Count == 0)
                {
                    workField = await _appContext.WorkFields.Where(k => k.Created >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                }

                var organization = await _appContext.GlpOrganizations.Where(k => !EF.Functions.Like(k.NameEn, "%[§\n*''/\",_&#^@()·-•]%") && k.Modified >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                if (organization.Count == 0)
                {
                    organization = await _appContext.GlpOrganizations.Where(k => !EF.Functions.Like(k.NameEn, "%[§\n*''/\",_&#^@()·-•]%") && k.Created >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                }

                var country = await _appContext.Countries.OrderBy(k => k.NameEn).ToListAsync();

                var industry = await _appContext.Industries.Where(k => k.Modified >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                if (industry.Count == 0)
                {
                    industry = await _appContext.Industries.Where(k => k.Created >= FromDate).OrderBy(k => k.NameEn).ToListAsync();
                }

                var profileSkill = await _appContext.ProfileSkills.Where(k => !EF.Functions.Like(k.Name, "%[§\n*''/\",_&#^@()·-•]%") && k.Modified >= FromDate).OrderBy(k => k.Name).ToListAsync();
                if(profileSkill.Count == 0)
                {
                    profileSkill = await _appContext.ProfileSkills.Where(k => !EF.Functions.Like(k.Name, "%[§\n*''/\",_&#^@()·-•]%") && k.Created >= FromDate).OrderBy(k => k.Name).ToListAsync();
                }

                var data = new AllLookupItemsView()
                {
                    LookupItems = _mapper.Map<List<LookupItemView>>(lookupItems),
                    EducationFieldOfStudy = _mapper.Map<List<ProfileEducationFieldOfStudyView>>(educationFieldOfStudy),
                    WorkExperienceJobTitle = _mapper.Map<List<ProfileWorkExperienceJobTitleView>>(workExperienceJobTitle),
                    WorkField = _mapper.Map<List<WorkFieldView>>(workField),
                    Organization = _mapper.Map<List<OrganizationView>>(organization),
                    Country = _mapper.Map<List<CountryView>>(country),
                    Industry = _mapper.Map<List<IndustryView>>(industry),
                    ProfileSkill = _mapper.Map<List<ProfileSkillView>>(profileSkill)

                };

                return new LookupResponse(data);
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
        }

        public async Task<ILookupResponse> GetLookupItemsAsync(LookupType lookupType)
        {
            try
            {
                var degreeList = await _appContext.LookupItems.Where(k => k.LookupId == (int)lookupType).OrderBy(k => k.NameEn)
                    .ToListAsync();

                return new LookupResponse(_mapper.Map<List<LookupItemView>>(degreeList));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }

        }

        public async Task<ILookupResponse> GetDegreesAsync()
        {
            try
            {
                var degreeList = await _appContext.LookupItems.Where(k => k.LookupId == (int)LookupType.EducationalDegree).OrderBy(k => k.NameEn)
                    .ToListAsync();
                return new LookupResponse(_mapper.Map<List<LookupItemView>>(degreeList));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }

        }

        public async Task<ILookupResponse> GetInterestListAsync()
        {
            try
            {
                var degreeList = await _appContext.LookupItems.Where(k => k.LookupId == (int)LookupType.Interest).OrderBy(k => k.NameEn)
                    .ToListAsync();

                return new LookupResponse(_mapper.Map<List<LookupItemView>>(degreeList));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }

        public async Task<ILookupResponse> GetProfileLearningPreferenceListAsync()
        {
            try
            {
                var degreeList = await _appContext.LookupItems.Where(k => k.LookupId == (int)LookupType.Learningpreference).OrderBy(k => k.NameEn)
                    .ToListAsync();

                return new LookupResponse(_mapper.Map<List<LookupItemView>>(degreeList));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
         
        }



        public async Task<ILookupResponse> GetEducationFieldOfStudyAsync()
        {
            try
            {
                var fieldOfStudies = await _appContext.ProfileEducationFieldOfStudys.OrderBy(k => k.TitleEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<ProfileEducationFieldOfStudyView>>(fieldOfStudies));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }

        public async Task<ILookupResponse> GetWorkExperienceJobTitleAsync()
        {
            try
            {
                var fieldOfStudies = await _appContext.ProfileWorkExperienceJobTitle.OrderBy(k => k.TitleEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<ProfileWorkExperienceJobTitleView>>(fieldOfStudies));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
            
        }

        public async Task<ILookupResponse> GetWorkFieldAsync()
        {
            try
            {
                var fieldOfStudies = await _appContext.WorkFields.OrderBy(k => k.NameEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<WorkFieldView>>(fieldOfStudies));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }

        public async Task<ILookupResponse> GetGlpOrganizationListAsync()
        {
            try
            {
                var organizations = await _appContext.GlpOrganizations.OrderBy(k => k.NameEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<OrganizationView>>(organizations));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }

        public async Task<ILookupResponse> GetCountryListAsync()
        {
            try
            {
                var countries = await _appContext.Countries.OrderBy(k => k.NameEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<CountryView>>(countries));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
          
        }

        public async Task<ILookupResponse> GetIndustryListAsync()
        {
            try
            {
                var industries = await _appContext.Industries.Where(k => !EF.Functions.Like(k.NameEn, "%[§\n*''/\",_&#^@()·-•]%")).OrderBy(k => k.NameEn).ToListAsync();

                return new LookupResponse(_mapper.Map<List<IndustryView>>(industries));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }

        public async Task<ILookupResponse> GetProfileSkillsAsync()
        {
            try
            {

                var countries = await _appContext.ProfileSkills.Where(k=> !EF.Functions.Like(k.Name, "%[§\n*''/\",_&#^@()·-•]%")).OrderBy(k => k.Name)
                       .ToListAsync();

                return new LookupResponse(_mapper.Map<List<ProfileSkillView>>(countries));
            }
            catch (Exception e)
            {
                return new LookupResponse(e);
            }
           
        }


    }
}
