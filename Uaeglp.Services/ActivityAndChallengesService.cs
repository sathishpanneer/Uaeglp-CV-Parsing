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
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Minio.Exceptions;
using MongoDB.Driver;
using NLog;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ActivityViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;
using File = Uaeglp.Models.File;

namespace Uaeglp.Services
{
    public class ActivityAndChallengesService : IActivityAndChallengesService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbcontext;
        private readonly IMapper _mapper;
        private readonly FileDbContext _fileDbContext;
        private readonly IApplicationProgressStatusService _applicationProgressService;
        private readonly MinIoConfig _minIoConfig;
        private readonly IUserIPAddress _userIPAddress;

        public ActivityAndChallengesService(AppDbContext appDbContext, IMapper mapper, FileDbContext fileDbContext, IApplicationProgressStatusService applicationProgressService, MongoDbContext mongoDbcontext, IOptions<MinIoConfig> minIoConfig, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _fileDbContext = fileDbContext;
            _applicationProgressService = applicationProgressService;
            _mongoDbcontext = mongoDbcontext;
            _minIoConfig = minIoConfig.Value;
            _userIPAddress = userIPAddress;
        }

        public async Task<IActivityAndChallengesResponse> GetActivityCategoryAsync(int profileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var initiativeCategories = await _appDbContext.InitiativeCategories.Include(k => k.Initiatives).ToListAsync();

                var categoriesViewModels = _mapper.Map<List<InitiativeCategoryViewModel>>(initiativeCategories);
                foreach (var item in categoriesViewModels)
                {
                    item.Activities = item.Activities.Where(a => a.InitiativeTypeItemId == (int)InitiativeType.EngagementActivities && a.IsActive == true).ToList();
                    foreach (var item1 in item.Activities.ToList())
                    {
                        if (item1.FileId != null)
                        {
                            item1.FileName = _appDbContext.Files.FirstOrDefault(k => k.IdGuid == item1.FileId)?.Name ?? "";
                        }

                        var batch = await _appDbContext.BatchInitiatives.Where(x => x.InitiativeId == item1.Id).ToListAsync();

                        if (batch.Count() > 0)
                        {
                            var applicationList = new List<Application>();
                            foreach (var item2 in batch)
                            {
                                var application = await _appDbContext.Applications.Where(x => (x.BatchId == item2.BatchId && x.ProfileId == profileId) &&
                                                                                         (x.StatusItemId == 59006 || x.StatusItemId == 59009)).FirstOrDefaultAsync();
                                if (application != null)
                                {
                                    applicationList.Add(application);
                                }
                            }

                            if (applicationList.Count == 0)
                            {
                                item.Activities.Remove(item1);
                            }
                        }
                    }

                }

                var userActivity = await _appDbContext.InitiativeProfiles.Include(k => k.Initiative).ThenInclude(m => m.QuestionGroup)
                    .Include(m => m.StatusItem).Where(k => k.ProfileId == profileId).ToListAsync();


                var userInitiatives = userActivity.Select(k =>
                {
                    var ini = _mapper.Map<InitiativeViewModel>(k.Initiative);
                    if (ini.FileId != null && ini.FileId != new Guid())
                    {
                        ini.FileName = _appDbContext.Files.FirstOrDefault(k => k.IdGuid == ini.FileId)?.Name ?? "";
                    }
                    ini.InitiativeStatus = _mapper.Map<LookupItemView>(k.StatusItem);
                    ini.ReferenceNumber = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.Id == k.ParticipationReferenceID)?.ReferenceNumber ?? "";
                    return ini;
                }).ToList();



                var allInitiatives =
                    _mapper.Map<List<InitiativeViewModel>>(await _appDbContext.Initiatives.Include(k => k.InitiativeTypeItem).Where(k => k.IsActive == true).ToListAsync());
                foreach (var item1 in allInitiatives.ToList())
                {
                    if (item1.FileId != null && item1.FileId != new Guid())
                    {
                        item1.FileName = _appDbContext.Files.FirstOrDefault(k => k.IdGuid == item1.FileId)?.Name ?? "";
                    }

                    var batch = await _appDbContext.BatchInitiatives.Where(x => x.InitiativeId == item1.Id).ToListAsync();

                    if(batch.Count() > 0)
                    {
                        var applicationList = new List<Application>();
                        foreach (var item in batch)
                        {
                            var application = await _appDbContext.Applications.Where(x => (x.BatchId == item.BatchId && x.ProfileId == profileId) &&
                                                                                     (x.StatusItemId == 59006 || x.StatusItemId == 59009)).FirstOrDefaultAsync();
                            if (application != null)
                            {
                                applicationList.Add(application);
                            }
                        }

                        if (applicationList.Count == 0)
                        {
                            allInitiatives.Remove(item1);
                        }
                    }
                    
                }

                var lookupItemsList = _mapper.Map<List<LookupItemView>>(await _appDbContext.LookupItems.Where(k => k.LookupId == (int)LookupType.InitiativeType)
                    .ToListAsync());

                foreach (InitiativeType type in Enum.GetValues(typeof(InitiativeType)))
                {
                    if (type == InitiativeType.EngagementActivities || type == InitiativeType.MyActivity)
                    {
                        continue; ;
                    }

                    var lookUp = lookupItemsList.FirstOrDefault(k => k.Id == (int)type);
                    categoriesViewModels.Add(new InitiativeCategoryViewModel()
                    {
                        Id = (int)type,
                        TitleEn = lookUp?.NameEn,
                        TitleAr = lookUp?.NameAr,
                        Activities = allInitiatives.Where(k => k.InitiativeTypeItemId == (int)type).ToList()
                });
                }

                categoriesViewModels.ForEach(k => k.Activities.ForEach(m => m.InitiativeStatus = _mapper.Map<LookupItemView>(userActivity.FirstOrDefault(c => c.InitiativeId == m.Id)?.StatusItem)));

                categoriesViewModels.Insert(0, new InitiativeCategoryViewModel()
                {
                    Id = (int)InitiativeType.MyActivity,
                    TitleEn = "My Activity",
                    TitleAr = "نشاطي",
                    Activities = userInitiatives
                });

                foreach (var item2 in categoriesViewModels)
                {

                    foreach (var item3 in item2.Activities)
                    {
                        var reminder = await _appDbContext.Reminders.Where(x => x.UserID == profileId && x.ActivityId == item3.Id && x.ApplicationId == 2).FirstOrDefaultAsync();
                        item3.isReminderSet = reminder != null ? true : false;
                    }
                }

                return new ActivityAndChallengesResponse(categoriesViewModels);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new ActivityAndChallengesResponse(e);
            }
        }

        public async Task<IActivityAndChallengesResponse> GetActivityListAsync(int profileId, int categoryId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  ProfileId: {profileId} CategoryId: {categoryId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userActivity = await _appDbContext.InitiativeProfiles.Include(k => k.Initiative)
                    .Include(m => m.StatusItem).Where(k => k.ProfileId == profileId).ToListAsync();

                if (categoryId == (int)InitiativeType.MyActivity)
                {
                    var userInitiatives = userActivity.Select(k =>
                    {
                        var ini = _mapper.Map<InitiativeViewModel>(k.Initiative);
                        if (ini.FileId != null && ini.FileId != new Guid())
                        {
                            ini.FileName = _appDbContext.Files.FirstOrDefault(k => k.IdGuid == ini.FileId)?.Name ?? "";
                        }
                        ini.InitiativeStatus = _mapper.Map<LookupItemView>(k.StatusItem);
                        return ini;
                    }).ToList();

                    return new ActivityAndChallengesResponse(userInitiatives);
                }

                var initiatives = await _appDbContext.Initiatives.Where(k => (k.CategoryId == categoryId || k.InitiativeTypeItemId == categoryId) && (k.IsActive == true)).ToListAsync();

                var initiativeViewModels = _mapper.Map<List<InitiativeViewModel>>(initiatives);

                foreach (var item1 in initiativeViewModels.ToList())
                {
                    if (item1.FileId != null && item1.FileId != new Guid())
                    {
                        item1.FileName = _appDbContext.Files.FirstOrDefault(k => k.IdGuid == item1.FileId)?.Name ?? "";
                    }

                    var batch = await _appDbContext.BatchInitiatives.Where(x => x.InitiativeId == item1.Id).ToListAsync();

                    if (batch.Count() > 0)
                    {
                        var applicationList = new List<Application>();
                        foreach (var item2 in batch)
                        {
                            var application = await _appDbContext.Applications.Where(x => (x.BatchId == item2.BatchId && x.ProfileId == profileId) &&
                                                                                     (x.StatusItemId == 59006 || x.StatusItemId == 59009)).FirstOrDefaultAsync();
                            if (application != null)
                            {
                                applicationList.Add(application);
                            }
                        }

                        if (applicationList.Count == 0)
                        {
                            initiativeViewModels.Remove(item1);
                        }
                    }

                    var reminder = await _appDbContext.Reminders.Where(x => x.UserID == profileId && x.ActivityId == item1.Id && x.ApplicationId == 2).FirstOrDefaultAsync();
                    item1.isReminderSet = reminder != null ? true : false;
                }

                var initiativeCategories = await _appDbContext.InitiativeCategories.ToListAsync();
                foreach (var item in initiativeCategories)
                {
                    if (categoryId == item.Id)
                    {
                        initiativeViewModels = initiativeViewModels.Where(a => a.InitiativeTypeItemId == (int)InitiativeType.EngagementActivities).ToList();
                    }

                }

                initiativeViewModels.ForEach( m =>
               {
                   m.InitiativeStatus =
                          _mapper.Map<LookupItemView>(userActivity.FirstOrDefault(c => c.InitiativeId == m.Id)
                              ?.StatusItem);
                   if (m.FileId != new Guid())
                   {

                       var fileDb =  _fileDbContext.FileDB.FirstOrDefault(k => k.Id == m.FileId);

                       if (fileDb != null)
                       {
                           var file =  _appDbContext.Files.FirstOrDefault(k => k.IdGuid == fileDb.Id);
                           m.FileName = file.Name;
                       }


                   }
               }
                );

                return new ActivityAndChallengesResponse(initiativeViewModels);

            }
            catch (Exception e)
            {
                logger.Error(e);
                return new ActivityAndChallengesResponse(e);
            }
        }


        public async Task<IActivityAndChallengesResponse> GetActivityAsync(int profileId, int categoryId, int activityId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  ProfileId: {profileId} CategoryId: {categoryId} ActivityId: {activityId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var userActivity = await _appDbContext.InitiativeProfiles.Include(k => k.Initiative)
                    .Include(m => m.StatusItem).Where(k => k.ProfileId == profileId).ToListAsync();

                if (categoryId == (int)InitiativeType.MyActivity)
                {
                    var userInitiative = userActivity.Select(k =>
                    {
                        var ini = _mapper.Map<InitiativeViewModel>(k.Initiative);
                        ini.InitiativeStatus = _mapper.Map<LookupItemView>(k.StatusItem);
                        return ini;
                    }).ToList().FirstOrDefault(c => c.Id == activityId);
                    if(userInitiative != null)
                    {
                        return new ActivityAndChallengesResponse(userInitiative);
                    } else
                    {
                        return new ActivityAndChallengesResponse(ClientMessageConstant.Deeplink, HttpStatusCode.NotFound);
                    }
                    
                    
                }

                var initiative = await _appDbContext.Initiatives.FirstOrDefaultAsync(k => (k.CategoryId == categoryId || k.InitiativeTypeItemId == categoryId) && k.Id == activityId);

                var initiativeViewModel = _mapper.Map<InitiativeViewModel>(initiative);

                if (initiativeViewModel != null)
                {
                    var reminder = await _appDbContext.Reminders.Where(x => x.UserID == profileId && x.ActivityId == initiativeViewModel.Id && x.ApplicationId == 2).FirstOrDefaultAsync();
                    initiativeViewModel.isReminderSet = reminder != null ? true : false;

                    initiativeViewModel.InitiativeStatus =
                               _mapper.Map<LookupItemView>(userActivity.FirstOrDefault(c => c.InitiativeId == initiativeViewModel.Id)
                                   ?.StatusItem);
                    if (initiativeViewModel.FileId != new Guid())
                    {
                        var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == initiativeViewModel.FileId);

                        if (fileDb != null)
                        {
                            var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.IdGuid == fileDb.Id);
                            initiativeViewModel.FileName = file.Name;
                        }
                    }
                }
                return new ActivityAndChallengesResponse(initiativeViewModel);

            }
            catch (Exception e)
            {
                logger.Error(e);
                return new ActivityAndChallengesResponse(e);
            }
        }

        public async Task<IActivityAndChallengesResponse> GetActivityQuestionsAsync(int profileId, int initiativeId)
        {
            try
            {
                var initiative = await _appDbContext.Initiatives.Include(k => k.QuestionGroup)
                    .Include(m => m.QuestionAnswers).FirstOrDefaultAsync(k => k.Id == initiativeId);

                if (initiative == null)
                {
                    return new ActivityAndChallengesResponse(ClientMessageConstant.FileNotFound,
                        HttpStatusCode.NotFound);
                }

                var questionGroups = await _appDbContext.QuestionGroupsQuestions.Include(m => m.Group)
                    .Include(k => k.Question).Include(k => k.Question.QuestionTypeItem).Include(k => k.Question.Options)
                    .Where(k => k.GroupId == initiative.QuestionGroupId).ToListAsync();

                if (!questionGroups.Any())
                {
                    return new ActivityAndChallengesResponse(ClientMessageConstant.FileNotFound,
                        HttpStatusCode.NotFound);
                }

                var questions = questionGroups.Select(k => k.Question).ToList();

                var questionViews = _mapper.Map<List<QuestionViewModel>>(questions);

                //var answers = _mapper.Map<List<QuestionAnswerViewModel>>(await _appDbContext.QuestionAnswers.Include(k => k.AnswerFile)
                //    .Where(k => k.ProfileId == profileId && k.InitiativeId == initiativeId).ToListAsync());

                var listdata = await _appDbContext.QuestionAnswers.Include(k => k.AnswerFile).Include(k => k.Questionansweroptions)
                       .Where(k => k.ProfileId == profileId && k.InitiativeId == initiativeId).ToListAsync();

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

                return new ActivityAndChallengesResponse(questionViews);

            }
            catch (Exception e)
            {
                return new ActivityAndChallengesResponse(e);
            }
        }

        public async Task<IActivityAndChallengesResponse> AddActivityAnswerAsync(ActivityAnswerViewModel model)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  input: {model.ToJsonString()} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                InitiativeProfile initiativeProfile;

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.Id == model.ProfileId);
                if (userInfo == null)
                {
                    return new ActivityAndChallengesResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }
                var isExist = true;
                initiativeProfile = await _appDbContext.InitiativeProfiles.FirstOrDefaultAsync(k => k.ProfileId == model.ProfileId && k.InitiativeId == model.ActivityId);

                if (initiativeProfile == null)
                {
                    initiativeProfile = new InitiativeProfile()
                    {
                        InitiativeId = model.ActivityId,
                        ProfileId = model.ProfileId,
                       // StatusItemId = (int)InitiativeStatus.PendingApproval,
                        CreatedBy = userInfo.Email,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        ModifiedBy = userInfo.Email

                    };

                    await _appDbContext.InitiativeProfiles.AddAsync(initiativeProfile);
                    await _appDbContext.SaveChangesAsync();

                }

                var parti = await GetReferenceAsync(initiativeProfile.Id, model.ActivityId, userInfo);

                initiativeProfile = await _appDbContext.InitiativeProfiles.FirstOrDefaultAsync(k => k.ProfileId == model.ProfileId && k.InitiativeId == model.ActivityId);

                initiativeProfile.ParticipationReferenceID = parti.Id;

                await _appDbContext.SaveChangesAsync();

                foreach (var answer in model.Answers)
                {
                    var questionAnswer =
                        await _appDbContext.QuestionAnswers.Include(a => a.Questionansweroptions).FirstOrDefaultAsync(k =>
                            k.ProfileId == model.ProfileId && k.InitiativeId == model.ActivityId && k.QuestionId == answer.QuestionId) ??
                        GetQuestionAnswer(answer, model, ref isExist);
                    questionAnswer.CreatedBy = userInfo.Email;
                    questionAnswer.ModifiedBy = userInfo.Email;

                    switch (answer.QuestionType)
                    {
                        case ApplicationQuestionType.Text:
                            questionAnswer.Text = answer.Text;
                            break;
                        case ApplicationQuestionType.MultiSelect:
                           // questionAnswer.SelectedOptionId = answer.SelectedOptionId;
                            break;
                        case ApplicationQuestionType.TrueOrFalse:
                            questionAnswer.YnquestionAnswer = answer.YesNoAnswer;
                            break;
                        case ApplicationQuestionType.Scale:
                            questionAnswer.Scale = answer.Scale;
                            break;
                        case ApplicationQuestionType.File:
                            questionAnswer.AnswerFileId = (await SaveFileAsync(answer.AnswerFile, model.ProfileId, (int)ApplicationQuestionType.File)).Id;
                            break;
                        case ApplicationQuestionType.VideoAttachment:
                            questionAnswer.AnswerFileId = (await SaveFileAsync(answer.AnswerFile, model.ProfileId, (int)ApplicationQuestionType.VideoAttachment)).Id;
                            break;
                    }

                    if (!isExist)
                    {
                        await _appDbContext.QuestionAnswers.AddAsync(questionAnswer);
                        await _appDbContext.SaveChangesAsync();
                        switch (answer.QuestionType)
                        {
                            case ApplicationQuestionType.MultiSelect:
                                _appDbContext.QuestionAnswerOptions.Add(new QuestionAnswerOption()
                                {
                                    QuestionanswerID = questionAnswer.Id,
                                    optionID = answer.SelectedOptionId ?? 0
                                });

                                break;
                            case ApplicationQuestionType.MultipleChoice:
                                // questionAnswer.YnquestionAnswer = answer.YesNoAnswer;
                                char[] spearator = { ',' };
                                var Ids = answer.MultipleChoice.Split(spearator);
                                foreach (var item in Ids)
                                {
                                    if (item != "")
                                    {
                                        _appDbContext.QuestionAnswerOptions.Add(new QuestionAnswerOption()
                                        {
                                            QuestionanswerID = questionAnswer.Id,
                                            optionID = Convert.ToInt32(item)
                                        });
                                    }
                                }
                                break;

                        }
                        //await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        switch (answer.QuestionType)
                        {
                            case ApplicationQuestionType.MultiSelect:
                                if (questionAnswer.Questionansweroptions != null)
                                {
                                    List<QuestionAnswerOption> _optionsdata = new List<QuestionAnswerOption>();
                                    _optionsdata = questionAnswer.Questionansweroptions.ToList();
                                    foreach (var item in _optionsdata)
                                    {
                                        _appDbContext.QuestionAnswerOptions.Remove(item);
                                        await _appDbContext.SaveChangesAsync();
                                    }
                                }

                                _appDbContext.QuestionAnswerOptions.Add(new QuestionAnswerOption()
                                {
                                    QuestionanswerID = questionAnswer.Id,
                                    optionID = answer.SelectedOptionId ?? 0
                                });
                                await _appDbContext.SaveChangesAsync();
                                break;
                            case ApplicationQuestionType.MultipleChoice:
                                // questionAnswer.YnquestionAnswer = answer.YesNoAnswer;
                                if (questionAnswer.Questionansweroptions != null)
                                {
                                    List<QuestionAnswerOption> _optionsdata = new List<QuestionAnswerOption>();
                                    _optionsdata = questionAnswer.Questionansweroptions.ToList();
                                    foreach (var item in _optionsdata)
                                    {
                                        _appDbContext.QuestionAnswerOptions.Remove(item);
                                        await _appDbContext.SaveChangesAsync();
                                    }
                                }
                                char[] spearator = { ',' };
                                var Ids = answer.MultipleChoice.Split(spearator);
                                foreach (var item in Ids)
                                {
                                    if (item != "")
                                    {
                                        _appDbContext.QuestionAnswerOptions.Add(new QuestionAnswerOption()
                                        {
                                            QuestionanswerID = questionAnswer.Id,
                                            optionID = Convert.ToInt32(item)
                                        });
                                    }
                                }

                                break;

                        }
                    }

                    await _appDbContext.SaveChangesAsync();
                }

                var initiativeProfileView = _mapper.Map<InitiativeProfileViewModel>(initiativeProfile);

                initiativeProfileView.ReferenceNumber = parti.ReferenceNumber;
                return new ActivityAndChallengesResponse(initiativeProfileView);
            }

            catch (Exception e)
            {
                logger.Error(e);
                return new ActivityAndChallengesResponse(e);
            }
        }

        public async Task<IActivityAndChallengesResponse> GetReferenceAsync( int profileId, int activityId)
        {
            try
            {

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.Id == profileId);
                if (userInfo == null)
                {
                    return new ActivityAndChallengesResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var initiativeProfile = await _appDbContext.InitiativeProfiles.FirstOrDefaultAsync(k => k.ProfileId == profileId && k.InitiativeId == activityId);
                var initiative = await _appDbContext.Initiatives.FirstOrDefaultAsync(k => k.Id == activityId);
                if (initiativeProfile == null)
                {
                    
                    if (initiative != null && initiative.QuestionGroupId == null)
                    {
                        initiativeProfile = new InitiativeProfile()
                        {
                            InitiativeId = activityId,
                            ProfileId = profileId,
                            StatusItemId = (int)InitiativeStatus.Accepted,
                            CreatedBy = userInfo.Email,
                            Created = DateTime.Now,
                            Modified = DateTime.Now,
                            ModifiedBy = userInfo.Email
                        };

                        await _appDbContext.InitiativeProfiles.AddAsync(initiativeProfile);

                        initiative.RemainingSeatsCount = initiative.RemainingSeatsCount - 1;

                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return new ActivityAndChallengesResponse(ClientMessageConstant.FileNotFound,
                        HttpStatusCode.NotFound);
                    }
                    
                }
                var parti = await _appDbContext.ParticipationReferences.FirstOrDefaultAsync(k => k.InitiativeParticipationID == initiativeProfile.Id && k.ProfileID == profileId);

                if (parti == null)
                {

                    parti = await GetReferenceAsync(initiativeProfile.Id, activityId, userInfo);

                    initiativeProfile.ParticipationReferenceID = parti.Id;
                    await _appDbContext.SaveChangesAsync();

                }
                if (initiative != null && initiative.QuestionGroupId != null)
                {
                    initiativeProfile.StatusItemId = (int)InitiativeStatus.PendingApproval;
                }
                await _appDbContext.SaveChangesAsync();
                var initiativeProfileView = _mapper.Map<InitiativeProfileViewModel>(initiativeProfile);

                initiativeProfileView.ReferenceNumber = parti.ReferenceNumber;
                return new ActivityAndChallengesResponse(initiativeProfileView);

            }
            catch (Exception e)
            {

                return new ActivityAndChallengesResponse(e);
            }
        }



        private async Task<ParticipationReference> GetReferenceAsync(int initiativeProfileId, int initiativeId, UserInfo userInfo)
        {
            try
            {
                var initiative =  await _appDbContext.Initiatives.FirstOrDefaultAsync(k => k.Id == initiativeId);

                var ref1 = initiative?.ReferenceNumber?.ToString();

                var ref2 = initiativeProfileId.ToString("D3");

                var ref3 = new Random().Next(0, 99999).ToString("D4");

                var refNumber =  $"{ref1}-{ref3}";


                var data = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.InitiativeParticipationID == initiativeProfileId && a.ProfileID == userInfo.Id);
                if (data != null)
                {
                    return data;
                }
                var parti = new ParticipationReference()
                {
                    ReferenceNumber = refNumber,
                    InitiativeParticipationID = initiativeProfileId,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    ProfileID = userInfo.Id,
                    InitiativeTypeID = 98001
                };

                await _appDbContext.ParticipationReferences.AddAsync(parti);
                await _appDbContext.SaveChangesAsync();

                return parti;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new ParticipationReference();
            }

        }

        private QuestionAnswer GetQuestionAnswer(ApplicationAnswerViewModel answer, ActivityAnswerViewModel model, ref bool isExist)
        {
            isExist = false;
            return new QuestionAnswer()
            {
                ProfileId = model.ProfileId,
                InitiativeId = model.ActivityId,
                QuestionId = answer.QuestionId,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

        }

        private async Task<File> SaveFileAsync(IFormFile file, int userId, int fileType)
        {
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AnswerFileType : {fileType} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
            var userInfo = await _appDbContext.UserInfos.Include(k => k.User).FirstOrDefaultAsync(k => k.Id == userId);
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
                CreatedBy = userInfo.Email,
                ModifiedBy = userInfo.Email
            };

            var savedEntity = (await _appDbContext.Files.AddAsync(data)).Entity;
            if (fileType == (int)ApplicationQuestionType.File)
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  DocFile : {fileType}");
                await UploadIntoFileDbAsync(savedEntity.IdGuid, file);

            }
            else if (fileType == (int)ApplicationQuestionType.VideoAttachment)
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  VideoFile : {fileType}");
                minioAudioVideoUpload(file, savedEntity.IdGuid);
            }
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

        public bool minioAudioVideoUpload(IFormFile formFile, Guid id)
        {
            var appSetting = new MinIoConfig()
            {
                EndPoint = _minIoConfig.EndPoint,
                AccessKey = _minIoConfig.AccessKey,
                SecretKey = _minIoConfig.SecretKey,
                BucketName = _minIoConfig.BucketName,
                Location = _minIoConfig.Location,
                MinIoForDev = _minIoConfig.MinIoForDev,
                FilePath = _minIoConfig.FilePath
            };

            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  EndPoint : {appSetting.EndPoint}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  AccessKey : {appSetting.AccessKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  SecretKey : {appSetting.SecretKey}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  BucketName : {appSetting.BucketName}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Location : {appSetting.Location}");
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  MinIoForDev : {appSetting.MinIoForDev}");
            try
            {
                if (appSetting.MinIoForDev != true)
                {
                    var minio = new Minio.MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey).WithSSL();
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, id, appSetting.FilePath).Wait();
                    return true;
                }
                else
                {
                    var minio = new Minio.MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey);
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, id, appSetting.FilePath).Wait();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
        private async static Task Run(Minio.MinioClient minio, IFormFile _request, string bucketName, string location, Guid id, string fileLocation)
        {
            if (!Directory.Exists(fileLocation))
            {
                Directory.CreateDirectory(fileLocation);
            }

            string FilePath = "";
            using (var fileStream = new FileStream(fileLocation + _request.FileName, FileMode.Create))
            {
                await _request.CopyToAsync(fileStream);
                FilePath = fileStream.Name;
            }

            //var objectName = _request.FileName;
            var objectName = id.ToString();
            var filePath = FilePath;

            var contentType = _request.ContentType;

            //var contentType = "";
            //if (fileType == "audio")
            //{
            //    contentType = "audio/mp3";
            //}
            //else if (fileType == "video")
            //{
            //    contentType = "video/mp4";
            //}

            try
            {
                // Make a bucket on the server, if not already present.
                bool found = await minio.BucketExistsAsync(bucketName);
                if (!found)
                {
                    await minio.MakeBucketAsync(bucketName, location);
                }
                // Upload a file to bucket.
                await minio.PutObjectAsync(bucketName, objectName, filePath, contentType);
                System.IO.File.Delete(filePath);
                Console.WriteLine("Successfully uploaded " + objectName);

            }
            catch (MinioException e)
            {
                logger.Error(e);
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }

        }

        public async Task<IActivityAndChallengesResponse> DeleteParticipantAsync(int profileId, int activityId)
        {
            try
            {
                var userActivity = await _appDbContext.InitiativeProfiles.Include(k => k.Initiative)
                    .Include(m => m.StatusItem).Where(k => k.ProfileId == profileId).ToListAsync();

                
                var initiative = await _appDbContext.Initiatives.FirstOrDefaultAsync(k => k.Id == activityId);

                var initiativeViewModel = _mapper.Map<InitiativeViewModel>(initiative);


                initiativeViewModel.InitiativeStatus =
                           _mapper.Map<LookupItemView>(userActivity.FirstOrDefault(c => c.InitiativeId == initiativeViewModel.Id)
                               ?.StatusItem);
                if (initiativeViewModel.FileId != new Guid())
                {
                    var fileDb = await _fileDbContext.FileDB.FirstOrDefaultAsync(k => k.Id == initiativeViewModel.FileId);

                    if (fileDb != null)
                    {
                        var file = await _appDbContext.Files.FirstOrDefaultAsync(k => k.IdGuid == fileDb.Id);
                        initiativeViewModel.FileName = file.Name;
                    }
                }

                if (initiativeViewModel.QuestionGroupId == null)
                {
                    var Iniprofile = _appDbContext.InitiativeProfiles.FirstOrDefault(a => a.InitiativeId == activityId && a.ProfileId == profileId);
                    if (Iniprofile != null && Iniprofile.StatusItemId == (int)InitiativeStatus.Accepted)
                    {
                        var parti = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.Id == Iniprofile.ParticipationReferenceID);
                        if (parti != null)
                        {
                            _appDbContext.Remove(Iniprofile);
                            await _appDbContext.SaveChangesAsync();
                            _appDbContext.Remove(parti);
                            await _appDbContext.SaveChangesAsync();
                           
                        }
                    }
                }

                return new ActivityAndChallengesResponse(initiativeViewModel);

            }
            catch (Exception e)
            {
                return new ActivityAndChallengesResponse(e);
            }
        }
    }
}
