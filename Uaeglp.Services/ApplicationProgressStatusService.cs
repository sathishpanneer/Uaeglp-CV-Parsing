using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProgramViewModels;
using NLog;
using Uaeglp.Utilities;

namespace Uaeglp.Services
{
    public class ApplicationProgressStatusService : IApplicationProgressStatusService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IUserIPAddress _userIPAddress;

        public ApplicationProgressStatusService(AppDbContext appDbContext, IMapper mapper, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _userIPAddress = userIPAddress;
        }


        public async Task<bool> CreateApplicationProgressAsync(int applicationId, int profileId)
        {
            try
            {

                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == profileId);
                var profile = await _appDbContext.Profiles
                    .Include(k => k.ProfileEducations)
                    .Include(k => k.ProfileWorkExperiences)
                    .Include(k => k.ProfileAchievements)
                    .Include(k => k.ProfileTrainings)
                    .Include(k => k.ProfileAssessmentToolScores)
                    .Include(k => k.ProfileMemberships)
                    .FirstOrDefaultAsync(k => k.Id == profileId).ConfigureAwait(false);

                var application = await _appDbContext.Applications.FirstOrDefaultAsync(k => k.Id == applicationId);

                application.CompletionPercentage = 0;
                await _appDbContext.SaveChangesAsync();

                foreach (ApplicationSectionType sectionId in Enum.GetValues(typeof(ApplicationSectionType)))
                {
                    var applicationProgress = new ApplicationProgress
                    {
                        ApplicationId = applicationId,
                        ApplicationSectionItemId = (int)sectionId,
                        ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.InComplete,
                        Created = DateTime.Now,
                        CreatedBy = userInfo.Email,
                        Modified = DateTime.Now,
                        ModifiedBy = userInfo.Email
                    };

                    switch (sectionId)
                    {
                        case ApplicationSectionType.Education:

                            if (profile.ProfileEducations.Any())
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }

                            break;
                        case ApplicationSectionType.WorkExperience:

                            if (profile.ProfileWorkExperiences.Any())
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }


                            break;
                        case ApplicationSectionType.Membership:
                            if (profile.ProfileMemberships.Any())
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }

                            break;
                        case ApplicationSectionType.CandidateInformation:
                            if (profile.FirstNameEn != null && profile.SecondNameEn != null &&
                                profile.ThirdNameEn != null && profile.LastNameEn != null &&
                                profile.FirstNameAr != null && profile.SecondNameAr != null &&
                                 profile.ThirdNameAr != null && profile.LastNameAr != null &&
                                profile.ResidenceCountryId.HasValue && profile.Address != null &&
                                 profile.PassportNumber != null && profile.PassportIssueEmirateItemId.HasValue &&
                                                                     profile.BirthDate.HasValue && userInfo.Mobile != null &&
                                                                      profile.Eid != null && userInfo.User.OriginalImageFileId.HasValue)
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }
                            break;
                        case ApplicationSectionType.Training:
                            if (profile.ProfileTrainings.Any())
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Pending;
                            }
                            break;
                        case ApplicationSectionType.Achievement:

                            if (profile.ProfileAchievements.Any())
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }
                            break;
                        case ApplicationSectionType.ProgramDetails:
                            applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                            break;
                        case ApplicationSectionType.Attachment:
                            if (profile.LastEducationCertificateFileId.HasValue
                                && profile.UaeidfileId.HasValue
                                && profile.CvfileId.HasValue
                                && profile.PassportFileId.HasValue
                                && profile.FamilyBookFileId.HasValue)
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }
                            break;
                        case ApplicationSectionType.AssessmentTools:

                            var assessmentTools = await _appDbContext.BatchAssessmentTools
                                .Include(m => m.AssessmentTool).Where(m => m.BatchId == application.BatchId)
                                .Select(y => y.AssessmentTool).ToListAsync();
                            if (assessmentTools.Select(assessment => assessment.ProfileAssessmentToolScores.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false).All(isCompleted => isCompleted))
                            {
                                applicationProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                                //application.CompletionPercentage += 10;
                            }

                            break;
                    }

                    await _appDbContext.ApplicationProgresses.AddAsync(applicationProgress);
                    await _appDbContext.SaveChangesAsync();
                }
                return true;

            }
            catch (Exception e)
            {
                return false;
            }


        }

        public async Task<List<QuestionViewModel>> GetBatchQuestionsAsync(int profileId, int batchId)
        {
            //try
            //{
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var batchDetails = await _appDbContext.Batches.FirstOrDefaultAsync(k => k.Id == batchId && !k.IsClosed && k.DateRegFrom <= DateTime.Now && k.DateRegTo >= DateTime.Now);
                if (batchDetails == null)
                {
                    return null;
                }

                var questionGroups = await _appDbContext.QuestionGroupsQuestions.Include(m => m.Group)
                    .Include(k => k.Question).Include(k => k.Question.QuestionTypeItem).Include(k => k.Question.Options)
                    .Where(k => k.GroupId == batchDetails.QuestionGroupId).ToListAsync();

                var questions = questionGroups.Select(k => k.Question).ToList();

                var questionViews = _mapper.Map<List<QuestionViewModel>>(questions);

                var application = await
                    _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == profileId && k.BatchId == batchId);

                if (application != null)
                {
                    var listdata = await _appDbContext.QuestionAnswers.Include(k => k.AnswerFile).Include(k => k.Questionansweroptions)
                        .Where(k => k.ProfileId == profileId && k.ApplicationId == application.Id).ToListAsync();

                    var answers = _mapper.Map<List<QuestionAnswerViewModel>>(listdata);
                    foreach (var item in answers)
                    {
                        var options = listdata.FirstOrDefault(a => a.Id == item.Id).Questionansweroptions.ToList();
                        item.MultiSelectedOptionId = options.Select(a => new QuestionAnswerOptionViewModel()
                        {
                            optionID = a.optionID,
                            QuestionanswerID = a.QuestionanswerID
                        }).ToList();
                    }
                    questionViews.ForEach(k => k.Answered = answers.FirstOrDefault(y => y.QuestionId == k.Id));
                }

                return questionViews;
            //}
            //catch (Exception e)
            //{
            //    //return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            //}
        }

        public async Task UpdateApplicationProgressAsync(int applicationId, int profileId, int batchId)
        {

            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == profileId);
            var progressList = await _appDbContext.ApplicationProgresses.Where(c => c.ApplicationId == applicationId)
                .ToListAsync();
            var profile = await _appDbContext.Profiles
               
                .FirstOrDefaultAsync(k => k.Id == profileId).ConfigureAwait(false);

            var application = await _appDbContext.Applications.FirstOrDefaultAsync(k => k.Id == applicationId);


            var profileEducation = await _appDbContext.ProfileEducations.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileWorkExperiences = await _appDbContext.ProfileWorkExperiences.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileAchievements = await _appDbContext.ProfileAchievements.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileTrainings = await _appDbContext.ProfileTrainings.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileAssessmentToolScores = await _appDbContext.ProfileAssessmentToolScores.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileMemberships = await _appDbContext.ProfileMemberships.Where(k => k.ProfileId == profileId).ToListAsync();
            var applications = await _appDbContext.Applications.Where(k => k.ProfileId == profileId).ToListAsync();

            var data = await GetBatchQuestionsAsync(profileId, batchId);

            var questionCount = 0;
            var totalQuestionsCount = data.Count();
            foreach (var item in data)
            {
                if(item.Answered != null)
                {
                    questionCount++;
                }
            }

            var questionPercentage = ((questionCount * 100 )/ totalQuestionsCount);


            application.CompletionPercentage = questionPercentage;
            await _appDbContext.SaveChangesAsync();
            foreach (var appProgress in progressList)
            {

                var sectionType = (ApplicationSectionType)appProgress.ApplicationSectionItemId;

                switch (sectionType)
                {
                    case ApplicationSectionType.Education:

                        if (profileEducation.Any())
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }

                        break;
                    case ApplicationSectionType.WorkExperience:

                        if (profileWorkExperiences.Any())
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }


                        break;
                    case ApplicationSectionType.Membership:
                        if (profileMemberships.Any())
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }

                        break;
                    case ApplicationSectionType.CandidateInformation:
                        if (profile.FirstNameEn != null && profile.SecondNameEn != null &&
                            profile.ThirdNameEn != null && profile.LastNameEn != null &&
                            profile.FirstNameAr != null && profile.SecondNameAr != null &&
                             profile.ThirdNameAr != null && profile.LastNameAr != null &&
                            profile.ResidenceCountryId.HasValue && profile.Address != null &&
                             profile.PassportNumber != null && profile.PassportIssueEmirateItemId.HasValue &&
                                                                 profile.BirthDate.HasValue && userInfo.Mobile != null &&
                                                                  profile.Eid != null && userInfo.User.OriginalImageFileId.HasValue)
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }
                        break;
                    case ApplicationSectionType.Training:
                        if (profileTrainings.Any())
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Pending;
                            //application.CompletionPercentage += 10;
                        }
                        break;
                    case ApplicationSectionType.Achievement:

                        if (profileAchievements.Any())
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }
                        break;
                    case ApplicationSectionType.ProgramDetails:
                        appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                        //application.CompletionPercentage += 10;
                        break;
                    case ApplicationSectionType.Attachment:
                        if (profile.LastEducationCertificateFileId.HasValue
                            && profile.UaeidfileId.HasValue
                            && profile.CvfileId.HasValue
                            && profile.PassportFileId.HasValue
                            && profile.FamilyBookFileId.HasValue)
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }
                        break;
                    case ApplicationSectionType.AssessmentTools:

                        var assessmentTools = await _appDbContext.BatchAssessmentTools
                            .Include(m => m.AssessmentTool).Where(m => m.BatchId == application.BatchId)
                            .Select(y => y.AssessmentTool).ToListAsync();
                        if (assessmentTools.Select(assessment => assessment.ProfileAssessmentToolScores.Where(k => k.ProfileId == profileId).OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false).All(isCompleted => isCompleted))
                        {
                            appProgress.ApplicationSectionStatusItemId = (int)ApplicationSectionStatus.Completed;
                            //application.CompletionPercentage += 10;
                        }

                        break;
                }
               if( appProgress.Id == 0)
                {
                    await _appDbContext.ApplicationProgresses.AddAsync(appProgress);
                }
         
                await _appDbContext.SaveChangesAsync();
            }
        }

        public async Task<ProfileCompletedViewModel> GetProfileCompletedDetailsAsync(int profileId)
        {

            var programViewDetails = new ProfileCompletedViewModel(); 
            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == profileId);
          
            var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(k => k.Id == profileId).ConfigureAwait(false);


            var profileEducation = await _appDbContext.ProfileEducations.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileWorkExperiences = await _appDbContext.ProfileWorkExperiences.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileAchievements = await _appDbContext.ProfileAchievements.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileTrainings = await _appDbContext.ProfileTrainings.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileAssessmentToolScores = await _appDbContext.ProfileAssessmentToolScores.Where(k => k.ProfileId == profileId).ToListAsync();
            var profileMemberships = await _appDbContext.ProfileMemberships.Where(k => k.ProfileId == profileId).ToListAsync();
            var applications = await _appDbContext.Applications.Where(k => k.ProfileId == profileId).ToListAsync();

            programViewDetails.Education = profileEducation.Count;
            programViewDetails.Achievements = profileAchievements.Count;
            programViewDetails.WorkExperiences = profileWorkExperiences.Count;
          //  programViewDetails.Membership = profile.ProfileMemberships.Count;
            programViewDetails.Trainings = profileTrainings.Count;

            if (profile.LastEducationCertificateFileId.HasValue
                && profile.UaeidfileId.HasValue
                && profile.CvfileId.HasValue
                && profile.PassportFileId.HasValue
                && profile.FamilyBookFileId.HasValue)
            {
                programViewDetails.IsAllDocumentUploaded = true;
            }

            if (profile.FirstNameEn != null && profile.SecondNameEn != null &&
                profile.ThirdNameEn != null && profile.LastNameEn != null &&
                profile.FirstNameAr != null && profile.SecondNameAr != null &&
                profile.ThirdNameAr != null && profile.LastNameAr != null &&
                profile.ResidenceCountryId.HasValue && profile.Address != null &&
                profile.PassportNumber != null && profile.PassportIssueEmirateItemId.HasValue &&
                profile.BirthDate.HasValue && userInfo.Mobile != null &&
                profile.Eid != null && userInfo.User.OriginalImageFileId.HasValue)
            {
                programViewDetails.IsCandidateInfoCompleted = true;
            }

            programViewDetails.IsAssessmentCompleted = true;

            var profileToolScore = profileAssessmentToolScores.ToList();

            var personalityAssessment =
                await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Personality);

            if (personalityAssessment != null)
            {
                var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                if (!isCompleted)
                {
                    programViewDetails.IsAssessmentCompleted = false;
                }
            }

            var eQAssessment =
                await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.EQ);

            if (eQAssessment != null)
            {
                var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                if (!isCompleted)
                {
                    programViewDetails.IsAssessmentCompleted = false;
                }
            }


            var wellBeingAssessment =
                await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing);

            if (wellBeingAssessment != null)
            {
                var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;

                if (!isCompleted)
                {
                    programViewDetails.IsAssessmentCompleted = false;
                }
            }

            programViewDetails.ProfileCompletedPercentage = profile.CompletenessPercentage;

            return programViewDetails;

        }

        public async Task<ProgramCompletedDetailsViewModel> GetProgramCompletedDetailsAsync(int profileId, int batchId)
        {
            try
            {

                var programViewDetails = new ProgramCompletedDetailsViewModel();
                var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.UserId == profileId);

                var appSetting = await _appDbContext.ApplicationSettings.ToListAsync();
                var programValidationMonth = appSetting.FirstOrDefault(k => k.Key == "programProfileDataValidateDuration")?.Value;
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  ProgramProfileDataValidateDuration: {programValidationMonth}");

                var profile = await _appDbContext.Profiles
                    .Include(k => k.ProfileAssessmentToolScores)
                    .FirstOrDefaultAsync(k => k.Id == profileId).ConfigureAwait(false);


                programViewDetails.ProfileModuleDetails = new List<DetailsViewModel>
            {
                new DetailsViewModel
                {
                    Module = "education" ,
                    ModuleCount = await _appDbContext.ProfileEducations.Where(k=>k.ProfileId == profileId).CountAsync(),
                    Updatedrecently = (await _appDbContext.ProfileEducations.Where(k=>k.ProfileId == profileId).OrderByDescending(k=>k.Created).FirstOrDefaultAsync())?.Created > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                },
                 new DetailsViewModel
                {
                    Module = "experience" ,
                    ModuleCount = await _appDbContext.ProfileWorkExperiences.Where(k=>k.ProfileId == profileId).CountAsync(),
                    Updatedrecently =(await _appDbContext.ProfileWorkExperiences.Where(k=>k.ProfileId == profileId).OrderByDescending(k=>k.Created).FirstOrDefaultAsync())?.Created > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                },
                  new DetailsViewModel
                {
                    Module = "training" ,
                    ModuleCount = await _appDbContext.ProfileTrainings.Where(k=>k.ProfileId == profileId).CountAsync(),
                    Updatedrecently = (await _appDbContext.ProfileTrainings.Where(k=>k.ProfileId == profileId).OrderByDescending(k=>k.Created).FirstOrDefaultAsync())?.Created > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                },
                   new DetailsViewModel
                {
                    Module = "achievements" ,
                    ModuleCount = await _appDbContext.ProfileAchievements.Where(k=>k.ProfileId == profileId).CountAsync(),
                    Updatedrecently =(await _appDbContext.ProfileAchievements.Where(k=>k.ProfileId == profileId).OrderByDescending(k=>k.Created).FirstOrDefaultAsync())?.Created > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                },
                    new DetailsViewModel
                {
                    Module = "membership" ,
                    ModuleCount = await _appDbContext.ProfileMemberships.Where(k=>k.ProfileId == profileId).CountAsync(),
                    Updatedrecently =(await _appDbContext.ProfileMemberships.Where(k=>k.ProfileId == profileId).OrderByDescending(k=>k.Created).FirstOrDefaultAsync())?.Created > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                },
            };

                programViewDetails.ProfileDocuments = new List<DetailsViewModel>
            {
                new DetailsViewModel
                {
                   DocumentType = "Passport",
                   Available = profile.PassportFileId.HasValue,
                    Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                }, new DetailsViewModel
                {
                   DocumentType = "emirateID",
                   Available = profile.UaeidfileId.HasValue,
                    Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                }, new DetailsViewModel
                {
                   DocumentType = "education",
                   Available = profile.LastEducationCertificateFileId.HasValue,
                    Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                }, new DetailsViewModel
                {
                   DocumentType = "cv",
                   Available = profile.CvfileId.HasValue,
                    Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                }, new DetailsViewModel
                {
                   DocumentType = "familybook",
                   Available = profile.FamilyBookFileId.HasValue,
                    Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                }
            };


                if (userInfo.Mobile != null)
                {
                    programViewDetails.ProfileContactDetails = new DetailsViewModel
                    {
                        Available = true,
                        Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                    };
                }
                if (userInfo.User.OriginalImageFileId.HasValue)
                {
                    programViewDetails.ProfileImg = new DetailsViewModel
                    {
                        Available = true,
                        Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                    };
                }
                if (profile.FirstNameEn != null && profile.SecondNameEn != null &&
                    profile.ThirdNameEn != null && profile.LastNameEn != null &&
                    profile.FirstNameAr != null && profile.SecondNameAr != null &&
                    profile.ThirdNameAr != null && profile.LastNameAr != null &&
                    profile.ResidenceCountryId.HasValue && profile.Address != null &&
                    profile.PassportNumber != null && profile.PassportIssueEmirateItemId.HasValue &&
                    profile.BirthDate.HasValue && profile.Eid != null)
                {
                    programViewDetails.ProfilePersonalDetails = new DetailsViewModel
                    {
                        Available = true,
                        Updatedrecently = profile.Modified > DateTime.Now.AddMonths(-Convert.ToInt32(programValidationMonth))
                    };
                }



                //programViewDetails.IsAssessmentCompleted = true;
                programViewDetails.ProgramPendingAssessmentDetails = new List<AssessmentDratail>() { };

                var profileToolScore = await _appDbContext.BatchAssessmentTools.Where(x => x.BatchId == batchId).ToListAsync();


                foreach (var item in profileToolScore)
                {
                        var assesment = await _appDbContext.ProfileAssessmentToolScores.Where(x => x.ProfileId == profileId && x.AssessmentToolId == item.AssessmentToolId).FirstOrDefaultAsync();
                        var titile = await _appDbContext.AssessmentTools.Where(x => x.Id == item.AssessmentToolId).Select(x => x.NameEn).FirstOrDefaultAsync();
                        var p2 = new AssessmentDratail()
                        {
                            Title = titile,
                            IsCompleted = assesment?.IsCompleted ?? false,

                        };

                        programViewDetails.ProgramPendingAssessmentDetails.Add(p2);
                }


                //var personalityAssessment =
                //    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Personality);

                //if (personalityAssessment != null)
                //{
                //    var p1 = new AssessmentDratail()
                //    {
                //        Title = personalityAssessment.NameEn
                //    };
                //    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                //    if (isCompleted)
                //    {
                //        p1.IsCompleted = true;
                //    }
                //    programViewDetails.ProgramPendingAssessmentDetails.Add(p1);
                //}

                //var eQAssessment =
                //    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.EQ);

                //if (eQAssessment != null)
                //{
                //    var p1 = new AssessmentDratail()
                //    {
                //        Title = eQAssessment.NameEn
                //    };
                //    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;
                //    if (isCompleted)
                //    {
                //        p1.IsCompleted = true;
                //    }
                //    programViewDetails.ProgramPendingAssessmentDetails.Add(p1);
                //}


                //var wellBeingAssessment =
                //    await _appDbContext.AssessmentTools.FirstOrDefaultAsync(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing);

                //if (wellBeingAssessment != null)
                //{
                //    var p1 = new AssessmentDratail()
                //    {
                //        Title = wellBeingAssessment.NameEn
                //    };
                //    var isCompleted = profileToolScore.OrderByDescending(k => k.Order).FirstOrDefault()?.IsCompleted ?? false;

                //    if (isCompleted)
                //    {
                //        p1.IsCompleted = true;
                //    }
                //    programViewDetails.ProgramPendingAssessmentDetails.Add(p1);
                //}

                programViewDetails.ProfileCompletedPercentage = profile.CompletenessPercentage;

                return programViewDetails;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
        }
    }
}
