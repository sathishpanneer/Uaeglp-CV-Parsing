using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Minio;
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
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;
using Uaeglp.ViewModels.ProgramViewModels;
using File = Uaeglp.Models.File;
using Profile = Uaeglp.Models.Profile;

namespace Uaeglp.Services
{
    public class ProgramService : IProgramService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly AppDbContext _appDbContext;
        private readonly MongoDbContext _mongoDbcontext;
        private readonly IMapper _mapper;
        private readonly FileDbContext _fileDbContext;
        private readonly IApplicationProgressStatusService _applicationProgressService;
        private readonly IEmailService _emailService;
        private readonly MinIoConfig _minIoConfig;
        private readonly IUserIPAddress _userIPAddress;


        public ProgramService(AppDbContext appDbContext, IMapper mapper, FileDbContext fileDbContext, IApplicationProgressStatusService applicationProgressService, MongoDbContext mongoDbcontext, IEmailService emailService, IOptions<MinIoConfig> minIoConfig, IUserIPAddress userIPAddress)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _fileDbContext = fileDbContext;
            _applicationProgressService = applicationProgressService;
            _mongoDbcontext = mongoDbcontext;
            _emailService = emailService;
            _minIoConfig = minIoConfig.Value;
            _userIPAddress = userIPAddress;
        }


        public async Task<IProgramResponse> GetCompletedProgramAsync(int userId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var completedPrograms = await _appDbContext.Applications.Where(k => k.ProfileId == userId && (k.StatusItemId == (int)ApplicationProgressStatus.Accepted || k.StatusItemId == (int)ApplicationProgressStatus.Alumni)).Join(
                    _appDbContext.Batches.Include(m => m.Programme).Include(m => m.Programme.ProgrammeTypeItem),
                    a => a.BatchId, b => b.Id,
                    (a, b) => new CompletedProgramView
                    {
                        BatchNumber = b.Number,
                        BatchYear = b.Year,
                        Program = _mapper.Map<ProgramView>(b.Programme),
                        StatusItemId = a.StatusItemId ?? 0,
                        ProgramTypeLookup = _mapper.Map<LookupItemView>(b.Programme.ProgrammeTypeItem),

                    }).ToListAsync();

                return new ProgramResponse(completedPrograms);
            }
            catch (Exception e)
            {
                return new ProgramResponse(ClientMessageConstant.WeAreUnableToProcessYourRequest, HttpStatusCode.InternalServerError);
            }

        }


        public async Task<IProgramResponse> GetAllProgramAsync(int profileId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var programs = await _appDbContext.Programmes.Include(m => m.ProgrammeTypeItem).Include(k => k.Batches).ToListAsync();

                var programBatchViews = new List<ProgramView>();

                var programViews = _mapper.Map<List<ProgramView>>(programs);

                foreach(var item in programViews)
                {
                    var batchList = item.Batches.ToList();
                    foreach(var item2 in batchList)
                    {
                       // Type myTypeB = typeof(ProgramView);
                        //PropertyInfo fieldInfo = ProgramView..GetProperty("BatchView");
                        

                        ProgramView progView = new ProgramView()
                        {
                            Id = item.Id,
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            DescriptionEn = item.DescriptionEn,
                            DescriptionAr = item.DescriptionAr,
                            Order = item.Order,
                            Fees = item.Fees,
                            Duration = item.Duration,
                            IsHidden = item.IsHidden,
                            ShortTitleEn = item.ShortTitleEn,
                            ShortTitleAr = item.ShortTitleAr,
                            ProgrammeTypeItemId = item.ProgrammeTypeItemId,
                            ImageId = item.ImageId,
                            DescriptionHtmlEn = item.DescriptionHtmlEn,
                            DescriptionHtmlAr = item.DescriptionHtmlAr,
                            SubDescriptionHtmlEn = item.SubDescriptionHtmlEn,
                            SubDescriptionHtmlAr = item.SubDescriptionHtmlAr,
                            ProgrammeTypeItem = item.ProgrammeTypeItem,
                            isReminderSet = item.isReminderSet,
                            Batches = item.Batches,
                            Applications = item.Applications,
                            Application_reference = item.Application_reference,
                            CompletedUsersList = item.CompletedUsersList,
                            BatchView = item2,
                        };

                        programBatchViews.Add(progView);
                    }
                }

                


                var applications = _mapper.Map<List<ApplicationView>>(await _appDbContext.Applications
                    .Include(k => k.StatusItem).Include(k => k.ReviewStatusItem).Include(k => k.AssessmentItem)
                    .Include(k => k.SecurityItem).Include(k => k.VideoAssessmentStatus)
                    .Where(k => k.ProfileId == profileId).ToListAsync());

                programBatchViews.ForEach(k => k.Applications = applications);

                
                foreach (var programView in programBatchViews)
                {
                    programView.CompletedUsersList =
                       await GetProgramCompletedUserDetailsAsync(profileId, programView.BatchId ?? 0);

                    var reminder = await _appDbContext.Reminders.Where(x => x.UserID == profileId && x.ActivityId == programView.BatchId && x.ApplicationId == 1).FirstOrDefaultAsync();
                    programView.isReminderSet = reminder != null ? true : false;

                    List<ApplicationReference> _ref = new List<ApplicationReference>();
                    foreach (var item in programView.Applications)
                    {
                        _ref.Add(new ApplicationReference()
                        {
                            ApplicationId = item.Id,
                            ReferenceNumber = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.ApplicationID != null && a.ApplicationID == item.Id && a.ProfileID == item.ProfileId)?.ReferenceNumber ?? ""
                    });
                        programView.Application_reference = _ref;
                        //item.ReferenceNumber = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.ApplicationID != null && a.ApplicationID == item.Id && a.ProfileID == item.ProfileId).ReferenceNumber ?? "";
                        }

                }

                return new ProgramResponse(programBatchViews);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IProgramResponse> GetProgramDetailsAsync(int profileId, int batchId)
        {
            try
            {
                var programs = await _appDbContext.Programmes.Include(m => m.ProgrammeTypeItem).Include(k => k.Batches).ToListAsync();

                var programViews = _mapper.Map<List<ProgramView>>(programs);
                var programBatchViews = new List<ProgramView>();

                foreach (var item in programViews)
                {
                    var batchList = item.Batches.ToList();
                    foreach (var item2 in batchList)
                    {
                        // Type myTypeB = typeof(ProgramView);
                        //PropertyInfo fieldInfo = ProgramView..GetProperty("BatchView");


                        ProgramView progView = new ProgramView()
                        {
                            Id = item.Id,
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            DescriptionEn = item.DescriptionEn,
                            DescriptionAr = item.DescriptionAr,
                            Order = item.Order,
                            Fees = item.Fees,
                            Duration = item.Duration,
                            IsHidden = item.IsHidden,
                            ShortTitleEn = item.ShortTitleEn,
                            ShortTitleAr = item.ShortTitleAr,
                            ProgrammeTypeItemId = item.ProgrammeTypeItemId,
                            ImageId = item.ImageId,
                            DescriptionHtmlEn = item.DescriptionHtmlEn,
                            DescriptionHtmlAr = item.DescriptionHtmlAr,
                            SubDescriptionHtmlEn = item.SubDescriptionHtmlEn,
                            SubDescriptionHtmlAr = item.SubDescriptionHtmlAr,
                            ProgrammeTypeItem = item.ProgrammeTypeItem,
                            isReminderSet = item.isReminderSet,
                            Batches = item.Batches,
                            Applications = item.Applications,
                            Application_reference = item.Application_reference,
                            CompletedUsersList = item.CompletedUsersList,
                            BatchView = item2
                        };

                        programBatchViews.Add(progView);
                    }
                }

                var applications = _mapper.Map<List<ApplicationView>>(await _appDbContext.Applications
                    .Include(k => k.StatusItem).Include(k => k.ReviewStatusItem).Include(k => k.AssessmentItem)
                    .Include(k => k.SecurityItem).Include(k => k.VideoAssessmentStatus)
                    .Where(k => k.ProfileId == profileId).ToListAsync());

                programBatchViews.ForEach(k => k.Applications = applications);


                var programView = programBatchViews.FirstOrDefault(k => k.BatchId == batchId);
                if (programView != null)
                {
                    programView.CompletedUsersList =
                        await GetProgramCompletedUserDetailsAsync(profileId, programView.BatchId ?? 0);

                    var reminder = await _appDbContext.Reminders.Where(x => x.UserID == profileId && x.ActivityId == programView.BatchId && x.ApplicationId == 1).FirstOrDefaultAsync();
                    programView.isReminderSet = reminder != null ? true : false;

                    List<ApplicationReference> _ref = new List<ApplicationReference>();
                    //foreach (var item in programView.Applications)
                    //{
                    //    _ref.Add(new ApplicationReference()
                    //    {
                    //        ApplicationId = item.Id,
                    //        ReferenceNumber = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.ApplicationID != null && a.ApplicationID == item.Id && a.ProfileID == item.ProfileId)?.ReferenceNumber ?? ""
                    //    });


                    //    programView.Application_reference = _ref;
                    //    //item.ReferenceNumber = _appDbContext.ParticipationReferences.FirstOrDefault(a => a.ApplicationID != null && a.ApplicationID == item.Id && a.ProfileID == item.ProfileId).ReferenceNumber ?? "";
                    //}

                    var application = await _appDbContext.Applications.Where(x => x.BatchId == batchId && x.ProfileId == profileId).FirstOrDefaultAsync();

                    if(application != null)
                    {
                        var participationRef = await _appDbContext.ParticipationReferences.Where(a => a.ApplicationID != null && a.ApplicationID == application.Id).FirstOrDefaultAsync();

                        _ref.Add(new ApplicationReference()
                        {
                            ApplicationId = application.Id,
                            ReferenceNumber = participationRef?.ReferenceNumber
                        });
                        programView.Application_reference = _ref;
                    }
                }

                return new ProgramResponse(programView);
            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IProgramResponse> GetBatchDetailsAsync(int profileId, int batchId)
        {
            try
            {

                var programs = await _appDbContext.Programmes.Include(m => m.ProgrammeTypeItem).Include(k => k.Batches).ToListAsync();

                var programViews = _mapper.Map<List<ProgramView>>(programs);

                var programBatchViews = new List<ProgramView>();
                foreach (var item in programViews)
                {
                    var batchList = item.Batches.ToList();
                    foreach (var item2 in batchList)
                    {
                        // Type myTypeB = typeof(ProgramView);
                        //PropertyInfo fieldInfo = ProgramView..GetProperty("BatchView");


                        ProgramView progView = new ProgramView()
                        {
                            Id = item.Id,
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            DescriptionEn = item.DescriptionEn,
                            DescriptionAr = item.DescriptionAr,
                            Order = item.Order,
                            Fees = item.Fees,
                            Duration = item.Duration,
                            IsHidden = item.IsHidden,
                            ShortTitleEn = item.ShortTitleEn,
                            ShortTitleAr = item.ShortTitleAr,
                            ProgrammeTypeItemId = item.ProgrammeTypeItemId,
                            ImageId = item.ImageId,
                            DescriptionHtmlEn = item.DescriptionHtmlEn,
                            DescriptionHtmlAr = item.DescriptionHtmlAr,
                            SubDescriptionHtmlEn = item.SubDescriptionHtmlEn,
                            SubDescriptionHtmlAr = item.SubDescriptionHtmlAr,
                            ProgrammeTypeItem = item.ProgrammeTypeItem,
                            isReminderSet = item.isReminderSet,
                            Batches = item.Batches,
                            Applications = item.Applications,
                            Application_reference = item.Application_reference,
                            CompletedUsersList = item.CompletedUsersList,
                            BatchView = item2
                        };

                        programBatchViews.Add(progView);
                    }
                }
                var applications = _mapper.Map<List<ApplicationView>>(await _appDbContext.Applications
                    .Include(k => k.StatusItem).Include(k => k.ReviewStatusItem).Include(k => k.AssessmentItem)
                    .Include(k => k.SecurityItem).Include(k => k.VideoAssessmentStatus)
                    .Where(k => k.ProfileId == profileId).ToListAsync());

                programBatchViews.ForEach(k => k.Applications = applications);


                var programView = programBatchViews.FirstOrDefault(k => k.BatchId == batchId);
                if (programView != null)
                {
                    programView.CompletedUsersList =
                        await GetProgramCompletedUserDetailsAsync(profileId, programView.BatchId ?? 0);
                }

                var profileCompleted = await _applicationProgressService.GetProfileCompletedDetailsAsync(profileId);

                profileCompleted.Program = programView;

                return new ProgramResponse(profileCompleted);
            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IProgramResponse> GetBatchsDetailAsync(int profileId, int batchId)
        {
            try
            {

                var programs = await _appDbContext.Programmes.Include(m => m.ProgrammeTypeItem).Include(k => k.Batches).ToListAsync();

                var programViews = _mapper.Map<List<ProgramView>>(programs);
                var programBatchViews = new List<ProgramView>();

                foreach (var item in programViews)
                {
                    var batchList = item.Batches.ToList();
                    foreach (var item2 in batchList)
                    {
                        // Type myTypeB = typeof(ProgramView);
                        //PropertyInfo fieldInfo = ProgramView..GetProperty("BatchView");


                        ProgramView progView = new ProgramView()
                        {
                            Id = item.Id,
                            TitleEn = item.TitleEn,
                            TitleAr = item.TitleAr,
                            DescriptionEn = item.DescriptionEn,
                            DescriptionAr = item.DescriptionAr,
                            Order = item.Order,
                            Fees = item.Fees,
                            Duration = item.Duration,
                            IsHidden = item.IsHidden,
                            ShortTitleEn = item.ShortTitleEn,
                            ShortTitleAr = item.ShortTitleAr,
                            ProgrammeTypeItemId = item.ProgrammeTypeItemId,
                            ImageId = item.ImageId,
                            DescriptionHtmlEn = item.DescriptionHtmlEn,
                            DescriptionHtmlAr = item.DescriptionHtmlAr,
                            SubDescriptionHtmlEn = item.SubDescriptionHtmlEn,
                            SubDescriptionHtmlAr = item.SubDescriptionHtmlAr,
                            ProgrammeTypeItem = item.ProgrammeTypeItem,
                            isReminderSet = item.isReminderSet,
                            Batches = item.Batches,
                            Applications = item.Applications,
                            Application_reference = item.Application_reference,
                            CompletedUsersList = item.CompletedUsersList,
                            BatchView = item2
                        };

                        programBatchViews.Add(progView);
                    }
                }
                var applications = _mapper.Map<List<ApplicationView>>(await _appDbContext.Applications
                    .Include(k => k.StatusItem).Include(k => k.ReviewStatusItem).Include(k => k.AssessmentItem)
                    .Include(k => k.SecurityItem).Include(k => k.VideoAssessmentStatus)
                    .Where(k => k.ProfileId == profileId).ToListAsync());

                programBatchViews.ForEach(k => k.Applications = applications);


                var programView = programBatchViews.FirstOrDefault(k => k.BatchId == batchId);
                if (programView != null)
                {
                    programView.CompletedUsersList =
                        await GetProgramCompletedUserDetailsAsync(profileId, programView.BatchId ?? 0);
                }

                var profileCompleted = await _applicationProgressService.GetProgramCompletedDetailsAsync(profileId, batchId);

                profileCompleted.Program = programView;
                
                var isProgramOpen =  await _appDbContext.Batches.Where(k => k.Id == batchId && !k.IsClosed && k.DateRegFrom <= DateTime.Now.Date && k.DateRegTo >= DateTime.Now.Date).FirstOrDefaultAsync();

                profileCompleted.isProgramOpen = isProgramOpen != null ? true : false;

                return new ProgramResponse(profileCompleted);
            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IProgramResponse> GetBatchQuestionsAsync(int profileId, int batchId)
        {
            try
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {profileId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");
                var batchDetails = await _appDbContext.Batches.FirstOrDefaultAsync(k => k.Id == batchId);
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

                return new ProgramResponse(questionViews);
            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message,HttpStatusCode.InternalServerError);
            }
        }

        public async Task<IProgramResponse> AddOrUpdateApplicationAnswerAsync(ProgramAnswerViewModel model)
        {
            try
            {
                //if (model.Answers.Count == 0)
                //{
                //    List<ApplicationAnswerViewModel> _model = new List<ApplicationAnswerViewModel>();
                //    _model.Add(new ApplicationAnswerViewModel() { 
                //    QuestionId = 165,
                //    QuestionType = ApplicationQuestionType.Text,
                //    Text = "Test Answer"
                    
                //    });
                //    _model.Add(new ApplicationAnswerViewModel()
                //    {
                //        QuestionId = 141,
                //        QuestionType = ApplicationQuestionType.MultiSelect,
                //        SelectedOptionId = 134
                //    });
                //    _model.Add(new ApplicationAnswerViewModel()
                //    {
                //        QuestionId = 170,
                //        QuestionType = ApplicationQuestionType.MultipleChoice,
                //        MultipleChoice = "158,159"
                //    });

                //    model.Answers = _model;
                //}
                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.Id == model.ProfileId);
                if (userInfo == null)
                {
                    return new ProgramResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }
                var isExist = true;

                var appId = await GetApplicationIdAsync(model.ProfileId, model.BatchId, userInfo);
                foreach (var answer in model.Answers)
                {
                    var questionAnswer =
                        await _appDbContext.QuestionAnswers.Include(a=>a.Questionansweroptions).FirstOrDefaultAsync(k =>
                            k.ProfileId == model.ProfileId && k.ApplicationId == appId && k.QuestionId == answer.QuestionId) ??
                        GetQuestionAnswer(answer, model.ProfileId, appId, ref isExist);
                    questionAnswer.ModifiedBy = userInfo.Email;
                    questionAnswer.CreatedBy = userInfo.Email;

                    switch (answer.QuestionType)
                    {
                        case ApplicationQuestionType.Text:
                            questionAnswer.Text = answer.Text;
                            break;
                        case ApplicationQuestionType.MultiSelect:
                            //questionAnswer.SelectedOptionId = answer.SelectedOptionId;
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

                await _applicationProgressService.UpdateApplicationProgressAsync(appId, model.ProfileId, model.BatchId);

                return new ProgramResponse();
            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }


        private QuestionAnswer GetQuestionAnswer(ApplicationAnswerViewModel answer, int profileId, int appId, ref bool isExist)
        {

            isExist = false;
            return new QuestionAnswer()
            {
                ProfileId = profileId,
                ApplicationId = appId,
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
            if(fileType == (int)ApplicationQuestionType.File)
            {
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  DocFile : {fileType} ");
                await UploadIntoFileDbAsync(savedEntity.IdGuid, file);

            } else if(fileType == (int)ApplicationQuestionType.VideoAttachment)
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
            logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  UserIPAddress: { _userIPAddress.GetUserIP().Result }");
        
            try
            {
                if (appSetting.MinIoForDev != true)
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey).WithSSL();
                    Run(minio, formFile, appSetting.BucketName, appSetting.Location, id, appSetting.FilePath).Wait();
                    return true;
                }
                else
                {
                    var minio = new MinioClient(appSetting.EndPoint, appSetting.AccessKey, appSetting.SecretKey);
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
        private async static Task Run(MinioClient minio, IFormFile _request, string bucketName, string location, Guid id, string fileLocation)
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


        private async Task<int> GetApplicationIdAsync(int profileId, int batchId, UserInfo userInfo)
        {
            var application = await
                _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == profileId && k.BatchId == batchId);

            if (application == null)
            {
                application = new Application()
                {
                    BatchId = batchId,
                    ProfileId = profileId,
                    StatusItemId = (int)ApplicationProgressStatus.InProgress,
                    
                    TotalScore = (await _appDbContext.Profiles.Include(k => k.ScoringProfile).FirstOrDefaultAsync())?.ScoringProfile?.ProfileTotalScore ?? 0,
                   Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy = userInfo.Email,
                    ModifiedBy = userInfo.Email
                };

                await _appDbContext.Applications.AddAsync(application);
                await _appDbContext.SaveChangesAsync();

                await _applicationProgressService.CreateApplicationProgressAsync(application.Id, profileId);
            }

            var parti = await _appDbContext.ParticipationReferences.FirstOrDefaultAsync(k => k.ApplicationID == application.Id && k.ProfileID == profileId);

            if (parti == null)
            {
                await GetReferenceAsync(application.Id, batchId, userInfo);
            }

            return application.Id;
        }

        public async Task<IProgramResponse> GetReferenceAsync(int profileId, int batchId)
        {
            try
            {

                var userInfo = await _appDbContext.UserInfos.FirstOrDefaultAsync(k => k.Id == profileId);
                if (userInfo == null)
                {
                    return new ProgramResponse(ClientMessageConstant.ProfileNotExist, HttpStatusCode.NotFound);
                }

                var application = await
                 _appDbContext.Applications.FirstOrDefaultAsync(k => k.ProfileId == profileId && k.BatchId == batchId);
                
                if (application == null)
                {
                    return new ProgramResponse(ClientMessageConstant.FileNotFound,
                        HttpStatusCode.NotFound);
                }
                var parti = await _appDbContext.ParticipationReferences.FirstOrDefaultAsync(k => k.ApplicationID == application.Id && k.ProfileID == profileId);

                if (parti == null)
                {

                    parti = await GetReferenceAsync(application.Id, batchId, userInfo);

                    application.ParticipationReferenceID = parti.Id;
                    await _appDbContext.SaveChangesAsync();

                }


                application.StatusItemId = (int)ApplicationProgressStatus.Submitted;

                await _appDbContext.SaveChangesAsync();

                var toEmail = await _appDbContext.UserInfos.Where(x => x.UserId == profileId).Select(x => x.Email).FirstOrDefaultAsync();
                var firstName = await _appDbContext.Profiles.Where(k => k.Id == profileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
                var lastName = await _appDbContext.Profiles.Where(k => k.Id == profileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
                var userName = firstName + " " + lastName;
                var batch = await _appDbContext.Batches.Where(x => x.Id == batchId).FirstOrDefaultAsync();
                var programName = await _appDbContext.Programmes.Where(x => x.Id == batch.ProgrammeId).Select(x => x.TitleEn).FirstOrDefaultAsync();
                logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  Sending Program Submission mail to : {toEmail} Program Name : {programName} UserName : {userName} ");
                await _emailService.SendProgramSubmissionEmailAsync(toEmail, programName, userName);

                return new ProgramResponse(parti.ReferenceNumber);

            }
            catch (Exception e)
            {
                return new ProgramResponse(e.Message, HttpStatusCode.InternalServerError);
            }
        }

        private async Task<ParticipationReference> GetReferenceAsync(int applicationId, int batchId, UserInfo userInfo)
        {

            var batch = await _appDbContext.Batches.FirstOrDefaultAsync(k => k.Id == batchId);

            var programme = await _appDbContext.Programmes.FirstOrDefaultAsync(k => k.Id == batch.ProgrammeId);

            var ref1 = batch.ReferenceNumber;

            var ref2 = applicationId.ToString("D3");

            var ref3 = new Random().Next(0, 99999).ToString("D4");

            var refNumber = $"{ref1}-{ref3}";


            var parti = new ParticipationReference()
            {
                ReferenceNumber = refNumber,
                ApplicationID = applicationId,
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

        private async Task<List<PublicProfileView>> GetProgramCompletedUserDetailsAsync(int profileId, int batchId)
        {

            try
            {
                //var userIds = await _appDbContext.Applications
                //    .Where(k => k.BatchId == batchId &&
                //                (k.StatusItemId == (int) ApplicationProgressStatus.Accepted ||
                //                 k.StatusItemId == (int) ApplicationProgressStatus.Alumni)).Select(m => m.ProfileId)
                //    .ToListAsync();

                var userIds = await _appDbContext.ProfileBatchSelectedAlumni
                    .Where(k => k.BatchID == batchId).Select(m => m.ProfileID)
                    .ToListAsync();

                var completedUsers = await _appDbContext.Profiles
                                                        .Include(k => k.ResidenceCountry)
                                                        .Include(k => k.ProfileWorkExperiences)
                                                        .Where(k => userIds.Contains(k.Id))
                                                        .OrderByDescending(k => k.Id)
                                                        .ToListAsync();


                var users = await GetProfileViewModelAsync(completedUsers);

                foreach (var userInfo in users)
                {
                    var user = await _mongoDbcontext.Users.Find(x => x.Id == userInfo.Id).FirstOrDefaultAsync();
                    if (user == null) { continue; }
                    userInfo.IsAmFollowing = user.FollowersIDs.Any(k => k == profileId);
                }
                return users;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private async Task<List<PublicProfileView>> GetProfileViewModelAsync(List<Profile> followingProfiles)
        {
            var followers = followingProfiles.Select(k => new PublicProfileView()
            {
                Id = k.Id,
                FirstNameAr = k.FirstNameAr,
                FirstNameEn = k.FirstNameEn,
                LastNameAr = k.LastNameAr,
                LastNameEn = k.LastNameEn,
                SecondNameAr = k.SecondNameAr,
                SecondNameEn = k.SecondNameEn,
                ThirdNameAr = k.ThirdNameAr,
                ThirdNameEn = k.ThirdNameEn,
                FollowersCount = k.FollowersCount,
                FollowingCount = k.FollowingCount,
                PostCount = k.PostsCount,
                LPSPoint = k.Lpspoints,
                CompletePercentage = k.CompletenessPercentage
            }).ToList();

            foreach (var data in followers)
            {
                var experiences = await ProfileWorkExperienceAsync(data.Id);
                data.Designation = experiences?.OrderByDescending(k => k.DateFrom).FirstOrDefault()
                    ?.ExperienceJobTitleView?.TitleEn;
                data.DesignationAr = experiences?.OrderByDescending(k => k.DateFrom).FirstOrDefault()
                    ?.ExperienceJobTitleView?.TitleAr;
                data.UserImageFileId = (await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == data.Id))
                                       ?.OriginalImageFileId ?? 0;
            }

            return followers;
        }

        private async Task<List<ProfileWorkExperienceView>> ProfileWorkExperienceAsync(int userId)
        {
            var workExperiences = await _appDbContext.ProfileWorkExperiences.Include(k => k.Organization)
                .Include(k => k.Country).Include(k => k.FieldOfwork).Include(k => k.Industry)
                .Include(k => k.LineManagerTitle).Include(k => k.Title).Where(k => k.ProfileId == userId).ToListAsync();

            var data = _mapper.Map<List<ProfileWorkExperienceView>>(workExperiences);

            return data;
        }
    }
}
