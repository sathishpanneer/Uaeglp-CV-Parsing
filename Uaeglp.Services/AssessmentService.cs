using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Uaeglp.Contracts;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.Models.Reports;
using Uaeglp.Repositories;
using Uaeglp.Services.Communication;
using Uaeglp.Utilities;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.MongoModels;
using MongoDB.Bson;
using MongoDB.Driver;
using NLog;
using System.Data;
using ClosedXML.Excel;
using System.IO;

namespace Uaeglp.Services
{
	public class AssessmentService : IAssessmentService
	{
		private readonly AppDbContext _appDbContext;
		private readonly IMapper _mapper;
		private readonly IEncryptionManager _encryptor;
		private readonly IFileService _fileService;
		private readonly IPushNotificationService _pushNotificationService;
		private readonly MongoDbContext _mongoDbContext;
		private readonly IEmailService _emailService;
		private static ILogger logger = LogManager.GetCurrentClassLogger();
		private readonly IUserIPAddress _userIPAddress;


		private readonly int educationPHDHolderScore = 10;
		private readonly int educationPHDHolderFromTop200QSRankScore = 5;
		private readonly int educationStudyingPHDScore = 2;
		private readonly int educationMasterHolderScore = 6;
		private readonly int educationMasterHolderFromTop200QSRankScore = 5;
		private readonly int educationStudyingMasterScore = 2;
		private readonly int educationBachelorScore = 3;
		private readonly int educationBachelorFromTop200QSRankScore = 5;
		private readonly int educationHigherDiplomaScore = 2;
		private readonly int educationDiplomaScore = 1;
		private readonly int experience1_3YearsScore = 5;
		private readonly int experience4_7YearsScore = 7;
		private readonly int experienceAbove7YearsScore = 10;
		private readonly int experienceInMulinationalOrganizationScore = 2;
		private readonly int experienceInBothPublicAndPrivateScore = 5;
		private readonly int experienceInUAEPrioritySectorScore = 3;
		private readonly int assessmentPersonalityWeight = 20;
		private readonly int assessmentWellbeingWeight = 5;
		private readonly int assessmentEQWeight = 15;
		private readonly int WellBeingHappinessPositiveID = 1012;
		private readonly int WellBeingHappinessNegativeID = 1013;
		private readonly double AffectiveBalanceMean = 4.35;
		private readonly double AffectiveBalanceStandardDeviation = 6.078;

		private readonly int AssessmentPersonalityIndividualReportQuestionGroup = 200;
		private readonly int AssessmentCareerInterestIndividualReportQuestionGroup = 210;
		private readonly int CognitiveVerbalID = 1;
		private readonly int CognitiveNumericalID = 2;
		private readonly int CognitiveAbstractID = 3;

		public Dictionary<ProfileSectionPercentage, int> ProfileSectionPercentages = new Dictionary<ProfileSectionPercentage, int>()
	{
	  {
		ProfileSectionPercentage.profileImg,
		5
	  },
	  {
		ProfileSectionPercentage.BasicInfoPrcange,
		15
	  },
	  {
		ProfileSectionPercentage.EducationPrcange,
		10
	  },
	  {
		ProfileSectionPercentage.WorkExpeincePrcange,
		25
	  },
	  {
		ProfileSectionPercentage.AchievementPrcange,
		20
	  },
	  {
		ProfileSectionPercentage.TrainingPrcange,
		5
	  },
	  {
		ProfileSectionPercentage.PersonalityAssessment,
		7
	  },
	  {
		ProfileSectionPercentage.EQAssessment,
		6
	  },
	  {
		ProfileSectionPercentage.WellbeingAssessment,
		7
	  }
	};

		public AssessmentService(AppDbContext appContext, IEncryptionManager encryption, IMapper mapper, IFileService fileService, IPushNotificationService pushNotificationService, MongoDbContext mongoDbContext, IEmailService emailService, IUserIPAddress userIPAddress)
		{
			_appDbContext = appContext;
			_mapper = mapper;
			_encryptor = encryption;
			_fileService = fileService;
			_pushNotificationService = pushNotificationService;
			_mongoDbContext = mongoDbContext;
			_emailService = emailService;
			_userIPAddress = userIPAddress;
		}

		public async Task<IAssessmentResponse<AssessmentToolReportViewModel>> GetReportByProfile(int ProfileID, int? AssessmentToolID)
		{
			try
			{
				AssessmentToolReportViewModel toolReportViewModel = new AssessmentToolReportViewModel();
				List<AssessmentToolReportView> assessmentToolReportViewList = new List<AssessmentToolReportView>();
				var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.Id == ProfileID);
				var user = await _appDbContext.UserInfos.FirstOrDefaultAsync(e => e.Id == ProfileID);
				var source = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID).Select(e => new
				{
					ID = e.Id,
					ProfileID = e.ProfileId,
					Profile = e.Profile,
					Order = e.Order,
					Score = e.Score,
					Created = e.Created,
					AssessmentToolID = e.AssessmentToolId,
					IsCompleted = e.IsCompleted,
					Modified = e.Modified,
					IsFailed = e.IsFailed,
					ProfileNameEn = profile.FirstNameEn + " " + profile.LastNameEn,
					ProfileNameAr = profile.FirstNameAr + " " + profile.LastNameAr,
					ProfileEmail = user.Email,
					ProfileMobile = user.Mobile
				});
				if (AssessmentToolID.HasValue)
				{
					source = source.Where(e => e.AssessmentToolID == AssessmentToolID.Value);
				}
				toolReportViewModel.CompletedCount = source.Where(e => e.IsCompleted == true).Count();
				toolReportViewModel.InProgressCount = source.Where(e => !e.IsCompleted == true).Count();
				assessmentToolReportViewList.AddRange(
					source
					.Select(e => new AssessmentToolReportView
					{
						ProfileAssessmentID = e.ID,
						ProfileAssessmentIDEncrypted = _encryptor.Encrypt(e.ID.ToString()),
						ProfileID = e.ProfileID,
						Order = e.Order,
						Score = e.Score,
						AssessmentToolID = e.AssessmentToolID,
						IsCompleted = e.IsCompleted == null ? false : e.IsCompleted.Value,
						InProgress = e.IsCompleted == null ? true : !e.IsCompleted.Value,
						IsFailed = e.IsFailed.Value,
						ProfileNameEn = e.ProfileNameEn,
						ProfileNameAr = e.ProfileNameAr,
						ProfileEmail = e.ProfileEmail,
						ProfileMobile = e.ProfileMobile,
						CompletedDate = e.Modified.ToString("yyyy/MM/dd hh:mm tt"),
						Trial = e.Order,
					}).ToList());

				foreach (var item in assessmentToolReportViewList)
				{
					var assessment = GetAssessment(item.AssessmentToolID);
					item.AssessmneToolCategoryID = assessment.AssessmentToolCategory;
					item.AssessmentName = new EnglishArabicView()
					{
						Arabic = assessment.NameAr,
						English = assessment.NameEn
					};
					item.Groups = GetAssessmentGroupsview(item.AssessmentToolID, ProfileID);
					item.FeedbackSubmited = IsFeedbackSubmited(item.ProfileAssessmentID);
				}

				toolReportViewModel.ProfileIDEncrypted = _encryptor.Encrypt(ProfileID.ToString());
				toolReportViewModel.AssessmentToolReport = assessmentToolReportViewList;
				return new AssessmentResponse<AssessmentToolReportViewModel>(toolReportViewModel);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentToolReportViewModel>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentToolReportViewModel>> GetProfileDrillDownByGroup_New(int ProfileID, int groupId, int? AssessmentToolID)
		{
			try
			{
				AssessmentToolReportViewModel toolReportViewModel = new AssessmentToolReportViewModel();
				List<AssessmentToolReportView> assessmentToolReportViewList = new List<AssessmentToolReportView>();
				var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(e => e.Id == ProfileID);
				var user = await _appDbContext.UserInfos.FirstOrDefaultAsync(e => e.Id == ProfileID);
				var source = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID).Select(e => new
				{
					ID = e.Id,
					ProfileID = e.ProfileId,
					Profile = e.Profile,
					Order = e.Order,
					Score = e.Score,
					Created = e.Created,
					AssessmentToolID = e.AssessmentToolId,
					IsCompleted = e.IsCompleted,
					Modified = e.Modified,
					IsFailed = e.IsFailed,
					ProfileNameEn = profile.FirstNameEn + " " + profile.LastNameEn,
					ProfileNameAr = profile.FirstNameAr + " " + profile.LastNameAr,
					ProfileEmail = user.Email,
					ProfileMobile = user.Mobile
				});
				if (AssessmentToolID.HasValue)
				{
					source = source.Where(e => e.AssessmentToolID == AssessmentToolID.Value);
				}
				

				foreach (var e in source.ToList())
				{
					var datacnt = GetAssessmentGroupsById(e.AssessmentToolID, e.ProfileID, groupId);
					if (datacnt.Count > 0)
					{
						AssessmentToolReportView dat = new AssessmentToolReportView();
						dat.ProfileAssessmentID = e.ID;
							dat.ProfileAssessmentIDEncrypted = _encryptor.Encrypt(e.ID.ToString());
						dat.ProfileID = e.ProfileID;
						dat.Order = e.Order;
						dat.Score = e.Score;
						dat.AssessmentToolID = e.AssessmentToolID;
						dat.IsCompleted = e.IsCompleted == null ? false : e.IsCompleted.Value;
						dat.InProgress = e.IsCompleted == null ? true : !e.IsCompleted.Value;
						dat.IsFailed = e.IsFailed == null ? false : e.IsFailed.Value;
						dat.ProfileNameEn = e.ProfileNameEn;
						dat.ProfileNameAr = e.ProfileNameAr;
						dat.ProfileEmail = e.ProfileEmail;
						dat.ProfileMobile = e.ProfileMobile;
						dat.CompletedDate = e.Modified.ToString("yyyy/MM/dd hh:mm tt");
						dat.Trial = e.Order;

						assessmentToolReportViewList.Add(dat);
					}
				}
				toolReportViewModel.CompletedCount = assessmentToolReportViewList.Where(e => e.IsCompleted == true).Count();
				toolReportViewModel.InProgressCount = assessmentToolReportViewList.Where(e => !e.IsCompleted == true).Count();
				foreach (var item in assessmentToolReportViewList)
				{
					var assessment = GetAssessment(item.AssessmentToolID);
					item.AssessmneToolCategoryID = assessment.AssessmentToolCategory;
					item.AssessmentName = new EnglishArabicView()
					{
						Arabic = assessment.NameAr,
						English = assessment.NameEn
					};
					item.Groups = GetAssessmentGroupsById(item.AssessmentToolID, ProfileID, groupId);
					item.FeedbackSubmited = IsFeedbackSubmited(item.ProfileAssessmentID);
				}
								
				toolReportViewModel.ProfileIDEncrypted = _encryptor.Encrypt(ProfileID.ToString());
				toolReportViewModel.AssessmentToolReport = assessmentToolReportViewList;
				return new AssessmentResponse<AssessmentToolReportViewModel>(toolReportViewModel);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentToolReportViewModel>(e);
			}
		}

		public async Task<IAssessmentResponse<object>> GetListByProfile(int ProfileID)
		{
			try
			{
				var assessmentIdsMergeViewBatch = new List<int>();
				var assessmentIdsforAssessmentCycle = new List<int>();
				var assessmentIdsforAssignedAssessments = new List<int>();
				var batchAssessmentTools = await _appDbContext.Batches.Include(x => x.BatchAssessmentTools).Where(e => !e.IsClosed && e.DateRegFrom <= DateTime.Now && e.DateRegTo >= DateTime.Now)
					.Select(e => e.BatchAssessmentTools).ToListAsync();

				batchAssessmentTools.ForEach(a => assessmentIdsMergeViewBatch.AddRange(a.Select(r => r.AssessmentToolId).ToList()));

				var assessmentMatrix = await _appDbContext.Applications.Include(x => x.Batch)
					.ThenInclude(x => x.AssessmentMatrix).Where(e => e.ProfileId == ProfileID && e.StatusItemId == (int?)59004)
					.Select(e => e.Batch).Where(b => b.AssessmentStartDate <= (DateTime?)DateTime.Now && b.AssessmentEndDate > (DateTime?)DateTime.Now)
					.Select(e => e.AssessmentMatrix).ToListAsync();

				assessmentMatrix.ForEach(a => assessmentIdsforAssessmentCycle.AddRange(a.MatricesTool.Select(r => r.AssessmentToolId).ToList()));

				var assessmentToolMatrix = await _appDbContext.AssignedAssessments.Include(x => x.AssessmentToolMatrix).ThenInclude(x => x.MatricesTool)
					.Where(e => e.ProfileId == ProfileID && e.DateFrom <= DateTime.Now && e.DateTo > DateTime.Now)
					.Select(e => e.AssessmentToolMatrix).ToListAsync();

				assessmentToolMatrix.ForEach(a => assessmentIdsforAssignedAssessments.AddRange(a.MatricesTool.Select(r => r.AssessmentToolId).ToList()));

				var assessmentToolIds = await _appDbContext.AssessmentTools.Where(e => e.IsDefaultVisible).Select(e => e.Id).ToListAsync();
				assessmentIdsMergeViewBatch.AddRange(assessmentToolIds);
				assessmentIdsMergeViewBatch.AddRange(assessmentIdsforAssessmentCycle);
				assessmentIdsMergeViewBatch.AddRange(assessmentIdsforAssignedAssessments);

				var assessmentTools = _appDbContext.AssessmentTools.Where(a => assessmentIdsMergeViewBatch.Contains(a.Id)).ToList();
				if (assessmentTools.Count() == 0)
					return new AssessmentResponse<object>(new object());

				//var assessmentToolsView = assessmentTools.Select(e => new AssessmentToolView()
				//{
				//	ID = e.Id,
				//	Name = new EnglishArabicView()
				//	{
				//		Arabic = e.NameAr,
				//		English = e.NameEn
				//	},
				//	AssessmentToolTypeID = e.AssessmentToolType,
				//	IDEncrypted = this._encryptor.Encrypt(e.Id.ToString()),
				//	ValidityRangeNumber = e.ValidityRangeNumber,
				//	Valid = this.CheckValidity(e.Id, ProfileID, e.ValidityRangeNumber),
				//	Description = new EnglishArabicView()
				//	{
				//		Arabic = e.DescriptionAr,
				//		English = e.DescriptionEn
				//	},
				//	//MobImageID = e.MobImageId,
				//	Image = e.ImageId != new Guid() ? _fileService.GetFile(e.ImageId) : new FileViewModel(),
				//	ImageID = e.ImageId,
				//	MobImageID = e.MobImageId,
				//	HasQuestionDirect = e.HasQuestionDirect,
				//	IsRequiredAssessment = assessmentIdsforAssessmentCycle.Contains(e.Id),
				//	TimeLeft = CheckValidity(e.Id, ProfileID, e.ValidityRangeNumber) ? new TimeDiffereceView() : this.GetMonthDifference(e.Id, ProfileID, e.ValidityRangeNumber),
				//	IsAssignedAssessment = assessmentIdsforAssignedAssessments.Contains(e.Id),
				//	Groups = GetAssessmentGroups(e.Id, ProfileID),
				//	HasCompetency = CheckCompetencyExist(e.Id),
				//	IsProcessing = CheckProcessing(e.Id, ProfileID),
				//	IsCompleted = IsCompleted(e.Id, ProfileID),
				//	IsFailed = IsFailed(e.Id, ProfileID),
				//	TotalQuestions = GetQuestionCountByAssessment(e.Id)
				//}).OrderBy(e => assessmentIdsMergeViewBatch.IndexOf(e.ID)).ToList();


				var assessmentToolsObject = assessmentTools.Select(e => new
				{
					ID = e.Id,
					NameAr = e.NameAr,
					NameEn = e.NameEn,
					DescriptionAr = e.DescriptionAr,
					DescriptionEn = e.DescriptionEn,
					DurationAr = e.DurationAr,
					DurationEn = e.DurationEn,
					InstructionsEn = e.InstructionsEn,
					InstructionsAr = e.InstructionsAr,
					ImageID = e.ImageId,
					MobImageID = e.MobImageId,
					AssessmentToolTypeID = e.AssessmentToolType,
					HasQuestionDirect = e.HasQuestionDirect,
					TimeLeft = CheckValidity(e.Id, ProfileID, e.ValidityRangeNumber) ? new TimeDiffereceView() : this.GetMonthDifference(e.Id, ProfileID, e.ValidityRangeNumber),
					IsAssignedAssessment = assessmentIdsforAssignedAssessments.Contains(e.Id),
					Groups = GetAssessmentGroupsview(e.Id, ProfileID),
					HasCompetency = CheckCompetencyExist(e.Id),
					IsProcessing = CheckProcessing(e.Id, ProfileID),
					IsCompleted = IsCompleted(e.Id, ProfileID, e.ValidityRangeNumber),
					IsFailed = IsFailed(e.Id, ProfileID, e.ValidityRangeNumber),
					TotalQuestions = GetQuestionCountByAssessment(e.Id),
					ProfileAssessmentToolScoreId = GetProfileAssessmentTool(e.Id, ProfileID),
					AssessmentCategoryId = e.AssessmentToolCategory
				}).OrderBy(e => assessmentIdsMergeViewBatch.IndexOf(e.ID)).ToList();

				return new AssessmentResponse<object>(assessmentToolsObject);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<object>(e);
			}
		}

		public async Task<IAssessmentResponse<List<AssessmentToolView>>> GetListByProfileAndGroupID(int ProfileID, int GroupID)
		{
			try
			{
				var assessmentToolMatrix = await _appDbContext.AssessmentGroups.Where(e => e.Id == GroupID && e.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID)).Select(e => e.AssessmentToolMatrix).Include(x => x.MatricesTool).FirstOrDefaultAsync();
				var assessmentIdsattacheViewGrp = new List<int>();
				assessmentIdsattacheViewGrp.AddRange(assessmentToolMatrix.MatricesTool.Select(e => e.AssessmentToolId).ToList());
				var assessmentToolsView = _appDbContext.AssessmentTools.Where(a => assessmentIdsattacheViewGrp.Contains(a.Id)).Select(e => new AssessmentToolView
				{
					ID = e.Id,
					Name = new EnglishArabicView()
					{
						Arabic = e.NameAr,
						English = e.NameEn
					},
					AssessmentToolTypeID = e.AssessmentToolType,
					ValidityRangeNumber = e.ValidityRangeNumber,
					DurationEn = e.DurationEn,
					DurationAr = e.DurationAr,
					InstructionsAr = e.InstructionsAr,
					InstructionsEn = e.InstructionsEn,
					Description = new EnglishArabicView()
					{
						Arabic = e.DescriptionAr,
						English = e.DescriptionEn
					},
					ImageID = e.ImageId,
					MobImageID = e.MobImageId,
					HasQuestionDirect = e.HasQuestionDirect
				}).ToList();

				return new AssessmentResponse<List<AssessmentToolView>>(assessmentToolsView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<List<AssessmentToolView>>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentToolView>> GetAssessmentDetails(int ID, LanguageType lang = LanguageType.AR)
		{
			try
			{
				var assessmentTool = await _appDbContext.AssessmentTools.Include(x => x.QuestionItems).FirstOrDefaultAsync(x => x.Id == ID);
				if (assessmentTool == null)
					throw new ArgumentException("Invalid assessmenttoolID: " + (object)ID);
				AssessmentToolView assessmentToolView = new AssessmentToolView()
				{
					ID = assessmentTool.Id,
					IDEncrypted = this._encryptor.Encrypt(ID.ToString()),
					Name = new EnglishArabicView()
				};
				assessmentToolView.Name.English = assessmentTool.NameEn;
				assessmentToolView.Name.Arabic = assessmentTool.NameAr;
				assessmentToolView.HasSubScale = assessmentTool.HasSubscale;
				assessmentToolView.AssessmentToolTypeID = assessmentTool.AssessmentToolType;
				assessmentToolView.AssessmentToolCategID = assessmentTool.AssessmentToolCategory;
				assessmentToolView.ValidityRangeNumber = assessmentTool.ValidityRangeNumber;
				assessmentToolView.Description = new EnglishArabicView();
				assessmentToolView.Description.English = assessmentTool.DescriptionEn;
				assessmentToolView.Description.Arabic = assessmentTool.DescriptionAr;
				assessmentToolView.DurationAr = assessmentTool.DurationAr;
				assessmentToolView.DurationEn = assessmentTool.DurationEn;
				assessmentToolView.InstructionsEn = assessmentTool.InstructionsEn;
				assessmentToolView.InstructionsAr = assessmentTool.InstructionsAr;
				assessmentToolView.MobImageID = assessmentTool.MobImageId;
				assessmentToolView.ImageID = assessmentTool.ImageId;
				assessmentToolView.Mean = assessmentTool.Mean;
				assessmentToolView.StandardDeviation = assessmentTool.StandardDeviation;
				assessmentToolView.HasQuestionDirect = assessmentTool.HasQuestionDirect;
				assessmentToolView.HasQuestionHead = assessmentTool.HasQuestionHead;
				assessmentToolView.IsDefaultVisible = assessmentTool.IsDefaultVisible;
				assessmentToolView.CoverPage = new EnglishArabicView()
				{
					Arabic = assessmentTool.CoverPageAr,
					English = assessmentTool.CoverPageEn
				};
				assessmentToolView.CoverPageName = (lang == LanguageType.EN) ? assessmentTool.CoverPageEn : assessmentTool.CoverPageAr;
				//if (assessmentToolView.ImageID != new Guid())
				//	assessmentToolView.Image = _fileService.GetFile(assessmentTool.ImageId);
				assessmentToolView.Groups = GetAssessmentGroups(assessmentTool.Id, 0);
				assessmentToolView.HasCompetency = CheckCompetencyExist(assessmentTool.Id);
				assessmentToolView.TotalQuestions = GetQuestionCountByAssessment(assessmentTool.Id);
				return new AssessmentResponse<AssessmentToolView>(assessmentToolView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentToolView>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentQuestionsView>> GetAssessmentQuestions(int AssessmentToolID, int profileID, LanguageType lang = LanguageType.AR)
		{
			var assessmentToolView = await _appDbContext.AssessmentTools.FirstOrDefaultAsync(x => x.Id == AssessmentToolID);
			var isValid = CheckValidity(assessmentToolView.Id, profileID, assessmentToolView.ValidityRangeNumber);
			if (!isValid) return new AssessmentResponse<AssessmentQuestionsView>(new AssessmentQuestionsView
			{
				ProfileScoreViewModel = new ProfileScoreView()
			});

			if (!assessmentToolView.HasQuestionHead)
			{
				return new AssessmentResponse<AssessmentQuestionsView>(new AssessmentQuestionsView
				{
					ProfileAssessmentQuestionView = await GetQuestionsByTest(AssessmentToolID, profileID, false)
				});
			}

			var byTest = await GetQuestionsHeadByTest_New(AssessmentToolID, profileID);
			if (byTest.IsFailed)
				await UpdateProfileCompletionAfterSubmission(profileID, AssessmentToolID, lang);

			return new AssessmentResponse<AssessmentQuestionsView>(new AssessmentQuestionsView
			{
				ProfileAssessmentHeadQuestionView = byTest
			});
		}

		public async Task<IAssessmentResponse<SubmitedAssessmentAnswersView>> SubmitAssessmentAnswers(int assessmentId, int profileId, int skip, int order, AssessmentAnswersView assessmentAnswersView)
		{
			try
			{
				var user = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == profileId);
				if (user == null) throw new ArgumentException($"user does not exist with the ID {profileId}");
				await AssignQuestionItemScores(assessmentAnswersView.QuestionItemScores, user, assessmentAnswersView.HasQuestionDirect, false, assessmentId, order);
				await UpdateTrialProcessing(assessmentId, user, order);
				await AssignAssessmentToolScore(assessmentId, user, assessmentAnswersView.HasQuestionDirect, false, order, false);
				

				var byTestFiltered = await GetByTestFiltered(assessmentId, profileId, assessmentAnswersView.HasSubScale, assessmentAnswersView.HasQuestionDirect, skip, assessmentAnswersView.QuestionIDs);
				return new AssessmentResponse<SubmitedAssessmentAnswersView>(new SubmitedAssessmentAnswersView
				{
					SkippedCount = byTestFiltered.SkippedCount,
					TotalTestCount = byTestFiltered.TotalTestCount,
					ProfileAssessmentQuestionView = byTestFiltered
				});
			}
			catch (Exception ex)
			{
				return new AssessmentResponse<SubmitedAssessmentAnswersView>(ex);
			}
		}

		public async Task<IAssessmentResponse<SubmitedAssessmentHeadAnswersView>> SubmitAssessmentHeadAnswers(int assessmentId, int profileId,
			List<QuestionScoreView> QuestionItemScores,
			  List<AssessmentNavigationObjectView> AssessmentNavigationObject,
			  int TotalTestCount,
			  int Order)
		  {
			try
			{
				var user = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == profileId);
				if (user == null) throw new ArgumentException($"user does not exist with the ID {profileId}");
				await AssignQuestionItemScores(QuestionItemScores, user, false, true, assessmentId, Order);
				await AssignAssessmentToolScore(assessmentId, user, false, true, Order, false);

				var byTestFiltered = await GetByTestFiltered(AssessmentNavigationObject, TotalTestCount, Order);
				byTestFiltered.TotalAnsweredCount = _appDbContext.ProfileQuestionItemScores.Count(a => a.ProfileId == profileId 
				&& a.AssessmentToolId == assessmentId);
				return new AssessmentResponse<SubmitedAssessmentHeadAnswersView>(new SubmitedAssessmentHeadAnswersView
				{
					SkippedCount = byTestFiltered.TotalAnsweredCount,
					TotalTestCount = byTestFiltered.TotalTestCount,
					ProfileAssessmentHeadQuestionView = byTestFiltered
				});
			}
			catch (Exception ex)
			{
				return new AssessmentResponse<SubmitedAssessmentHeadAnswersView>(ex);
			}
		}

		public async Task<IAssessmentResponse<string>> SubmitAllScores(int assessmentId, int profileId, int order, AssessmentAnswersView assessmentAnswersView, LanguageType lang = LanguageType.AR)
		{
			try
			{
				var user = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == profileId);
				if (user == null) throw new ArgumentException($"user does not exist with the ID {profileId}");
				await AssignQuestionItemScores(assessmentAnswersView.QuestionItemScores, user, assessmentAnswersView.HasQuestionDirect, false, assessmentId, order);
				//await UpdateTrialProcessing(assessmentId, user, order);
				await ExecuteCalcalution(assessmentId, user, assessmentAnswersView.HasQuestionDirect, order, lang);

				return new AssessmentResponse<string>("Successfully submitted");
			}
			catch (Exception ex)
			{
				return new AssessmentResponse<string>($"Submission failed with error: {ex.Message}");
			}
		}

		public async Task<IAssessmentResponse<string>> SubmitHeadAllScores(int assessmentId, int profileId, int order,
			List<QuestionScoreView> questionItemScores, LanguageType lang = LanguageType.AR)
		{
			try
			{
				var user = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == profileId);
				if (user == null) throw new ArgumentException($"user does not exist with the ID {profileId}");
				await AssignQuestionItemScores(questionItemScores, user, false, true, assessmentId, order);
				await AssignScorePerEachBlock(user.UserId, assessmentId, order);
				await AssignAssessmentToolScore(assessmentId, user, false, true, order, true);
				await UpdateProfileCompletionAfterSubmission(profileId, assessmentId, lang);

				return new AssessmentResponse<string>("Successfully submitted");
			}
			catch (Exception ex)
			{
				return new AssessmentResponse<string>($"Submission failed with error: {ex.Message}");
			}
		}

		public async Task<IAssessmentResponse<decimal>> CalculatePercentge(int profileID)
		{
			try
			{
				var percentage = await ReCalculateCompleteness(profileID);
				return new AssessmentResponse<decimal>(percentage.Percentage);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<decimal>(e);
			}
		}

		public async Task<IAssessmentResponse<List<AssessmentGroupView>>> GetAssessmentGroupsByUserId(int userId, bool isAdmin = false)
		{
			try
			{
				List<AssessmentGroupView> assessmentGroups;
				var member = _appDbContext.AssessmentGroupMembers.FirstOrDefault(a => a.ProfileId == userId);
				if (isAdmin)
				{
					assessmentGroups = await _appDbContext.AssessmentGroups.Select(t => new AssessmentGroupView
					{
						ID = t.Id,
						NameEN = t.NameEn,
						NameAR = t.NameAr,
						Color = t.Color,
						LogoID = t.LogoId,
						DateFrom = t.DateFrom,
						DateTo = t.DateTo,
						AssessmentToolMatrixID = t.AssessmentToolMatrixId
					}).ToListAsync();
				}
				else
				{
					assessmentGroups = await _appDbContext.AssessmentGroups
						.Include(x => x.AssessmentGroupMembers)
						.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == userId && m.IsCoordinator)).Select(t => new AssessmentGroupView
						{
							ID = t.Id,
							NameEN = t.NameEn,
							NameAR = t.NameAr,
							Color = t.Color,
							LogoID = t.LogoId,
							DateFrom = t.DateFrom,
							DateTo = t.DateTo,
							AssessmentToolMatrixID = t.AssessmentToolMatrixId
						}).ToListAsync();
				}
				
					foreach (var item in assessmentGroups)
					{
						var members = await GetAssessmentGroupMembers(item.ID);
						item.Members = members.Take(5).ToList();
						item.MembersCount = members.Count;
					}
				
				if (assessmentGroups == null || assessmentGroups.Count == 0)
					return new AssessmentResponse<List<AssessmentGroupView>>(new List<AssessmentGroupView>());

				return new AssessmentResponse<List<AssessmentGroupView>>(assessmentGroups);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<List<AssessmentGroupView>>(e);
			}
		}

		public async Task<IAssessmentResponse<List<ProfileView>>> GetMembers(int assessmentGroupId)
		{
			try
			{
				var group = await _appDbContext.AssessmentGroups.FirstOrDefaultAsync(p => p.Id == assessmentGroupId);

				if (group == null)
					return new AssessmentResponse<List<ProfileView>>(new Exception($"no group exist for group id: {assessmentGroupId}"));

				var members = _appDbContext.AssessmentGroupMembers.Where(p => p.AssessmentGroupId == assessmentGroupId)
					.OrderByDescending(m => m.Rank)
					.ThenByDescending(m => m.AssessmentCompletionPercentage).ToList();

				if (members == null || members.Count == 0)
					return new AssessmentResponse<List<ProfileView>>(new Exception($"no group member exist for group: {assessmentGroupId}"));

				var profiles = new List<ProfileView>();
				foreach (var member in members)
				{
					var profile = await _appDbContext.Profiles.Include(x => x.IdNavigation).FirstOrDefaultAsync(x => x.Id == member.ProfileId);
					profiles.Add(new ProfileView()
					{
						Id = member.ProfileId,
						NameEN = member.Profile.IdNavigation.NameEn,
						NameAR = member.Profile.IdNavigation.NameAr,
						Email = member.Profile.IdNavigation.Username,
						IDEncrypted = this._encryptor.Encrypt(member.ProfileId.ToString()),
						IsCoordinator = member.IsCoordinator,
						AssessmentCompletionPercentage = member.AssessmentCompletionPercentage,
						Rank = member.Rank,
						AssessmentTotalCount = member.TotalCount,
						AssessmentCompletionCount = member.CompletedCount,
						UserImageFileId = profile.IdNavigation?.OriginalImageFileId ?? 0
					});
				}

				return new AssessmentResponse<List<ProfileView>>(profiles.ToList());
			}
			catch (Exception e)
			{
				return new AssessmentResponse<List<ProfileView>>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentGroupMember>> AddMember(int profileId, int groupId)
		{
			try
			{
				var profile = await _appDbContext.Profiles.FirstOrDefaultAsync(x => x.Id == profileId);
				var member = new AssessmentGroupMember
				{
					ProfileId = profile.Id,
					AssessmentGroupId = groupId,
					Created = DateTime.Now,
					Modified = DateTime.Now,
					CreatedBy = "APP",
					ModifiedBy = "APP"
				};
				_appDbContext.AssessmentGroupMembers.Add(member);
				await _appDbContext.SaveChangesAsync();

				var userId = profileId;
				var email = await _appDbContext.UserInfos.Where(k => k.UserId == member.ProfileId).Select(k => k.Email).FirstOrDefaultAsync();
				var firstName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
				var lastName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
				var userName = firstName + " " + lastName;
				var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == groupId).Select(x => x.NameEn).FirstOrDefaultAsync();
				var notifyText = "You have been added to assessment group" + " " + groupName;
				var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == member.ProfileId && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
				if (customNotificationData?.isEnabled == true || customNotificationData == null)
				{
					await AddNotificationAsync(member.ProfileId, ActionType.AddNewItem, member.Id, ParentType.AssessmentGroup, userId, notifyText);
				}

				await _emailService.SendAssessmentReminderAsync(email, groupName, userName);
				return new AssessmentResponse<AssessmentGroupMember>(new AssessmentGroupMember
				{
					ProfileId = profile.Id,
					AssessmentGroupId = groupId,
					Created = DateTime.Now,
					Modified = DateTime.Now,
					CreatedBy = "APP",
					ModifiedBy = "APP",
					Id = member.Id
				});
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentGroupMember>(e);
			}
		}

		public async Task<IUserRecommendationResponse> AddNotificationAsync(int userId, ActionType actionId, int memberId, ParentType parentTypeId, int senderUserId, string notifyText)
		{
			try
			{
				logger.Info($"{ GetType().Name}  {  ExtensionUtility.GetCurrentMethod() }  userId: {userId} UserIPAddress: {  _userIPAddress.GetUserIP().Result }");

				//var profileId = userId != senderUserId ? senderUserId : userId;
				var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync() ??
												await AddNotificationObjectAsync(userId);

				var notificationObj = new Notification
				{
					ID = ObjectId.GenerateNewId(),
					ActionID = (int)actionId,
					IsRead = false,
					ParentID = memberId.ToString(),
					ParentTypeID = (int)parentTypeId,
					SenderID = senderUserId
				};

				notificationGenericObject.Notifications.Add(notificationObj);

				if (userId != senderUserId)
				{
					notificationGenericObject.UnseenNotificationCounter += 1;
					var notificationView = _mapper.Map<NotificationView>(notificationObj);
					await FillNotificationUserDetailsAsync(userId, new List<NotificationView>() { notificationView });

					var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
					foreach (var deviceId in deviceIds)
					{
						await _pushNotificationService.SendRecommendPushNotificationAsync(notificationView, notifyText, deviceId);
					}

					logger.Info("Notification sent");

				}

				await _mongoDbContext.NotificationGenericObjects.ReplaceOneAsync(x => x.UserID == userId, notificationGenericObject);



				var notificationGenericObjectView = new NotificationGenericObjectView
				{
					ID = notificationGenericObject.ID.ToString(),
					UserID = notificationGenericObject.UserID,
					UnseenNotificationCounter = notificationGenericObject.UnseenNotificationCounter,
					NotificationsList = _mapper.Map<List<NotificationView>>(notificationGenericObject.Notifications)
				};

				return new UserRecommendationResponse(notificationGenericObjectView);
			}
			catch (Exception e)
			{
				return new UserRecommendationResponse(e);
			}
		}

		//public async Task<bool> PushNotification(NotificationView notification, string notifyText, int userId)
		//{
		//	var deviceIds = await _appDbContext.UserDeviceInfos.Where(k => k.UserId == userId).Select(k => k.DeviceId).ToListAsync();
		//	foreach (var deviceId in deviceIds)
		//	{
		//		await _pushNotificationService.SendRecommendPushNotificationAsync(notification, notifyText, deviceId);
		//	}
		//	return true;
		//}

		private async Task FillNotificationUserDetailsAsync(int userId, List<NotificationView> notificationsList)
		{
			foreach (var notification in notificationsList)
			{
				var user = await _appDbContext.Users.FirstOrDefaultAsync(k => k.Id == notification.SenderID);
				notification.UserNameEn = user?.NameEn;
				notification.UserNameAr = user?.NameAr;
				notification.UserImageFileId = user?.OriginalImageFileId ?? 0;
				notification.RedirectUrlPath = notification.ParentTypeID == ParentType.User
					? $"/api/Profile/get-public-profile/{userId}/{notification.SenderID}"
					: notification.RedirectUrlPath;
			}
		}
		private async Task<NotificationGenericObject> AddNotificationObjectAsync(int userId)
		{
			try
			{
				var notificationGenericObject = await _mongoDbContext.NotificationGenericObjects.Find(x => x.UserID == userId).FirstOrDefaultAsync();

				if (notificationGenericObject != null) return notificationGenericObject;

				notificationGenericObject = new NotificationGenericObject
				{
					ID = ObjectId.GenerateNewId(),
					UserID = userId,
					UnseenNotificationCounter = 0,
					Notifications = new List<Notification>()
				};

				await _mongoDbContext.NotificationGenericObjects.InsertOneAsync(notificationGenericObject);
				return notificationGenericObject;
			}
			catch (Exception e)
			{
				throw e;
			}
		}



		public async Task<IAssessmentResponse<bool>> DeleteMember(int memberID, int groupID)
		{
			try
			{
				var member = await _appDbContext.AssessmentGroupMembers.FirstOrDefaultAsync(x => x.ProfileId == memberID && x.AssessmentGroupId == groupID);
				if (member == null) return new AssessmentResponse<bool>(new Exception("no such group member found"));
				_appDbContext.AssessmentGroupMembers.Remove(member);
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentGroupView>> AddAssessmentGroup(AssessmentGroupView assessmentGroupView)
		{
			try
			{
				var assessmentGroup = new AssessmentGroup()
				{
					NameEn = assessmentGroupView.NameEN,
					NameAr = assessmentGroupView.NameAR,
					Color = assessmentGroupView.Color,
					AssessmentToolMatrixId = assessmentGroupView.AssessmentToolMatrixID,
					LogoId = assessmentGroupView.LogoID.HasValue ? assessmentGroupView.LogoID.Value : 0,
					DateTo = assessmentGroupView.DateTo,
					DateFrom = assessmentGroupView.DateFrom
				};
				_appDbContext.AssessmentGroups.Add(assessmentGroup);
				await _appDbContext.SaveChangesAsync();
				assessmentGroupView.ID = assessmentGroup.Id;
				return new AssessmentResponse<AssessmentGroupView>(assessmentGroupView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentGroupView>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentGroupView>> UpdateAssessmentGroup(AssessmentGroupView assessmentGroupView)
		{
			try
			{
				AssessmentGroup assessmentGroup = _appDbContext.AssessmentGroups.FirstOrDefault(x => x.Id == assessmentGroupView.ID);
				if (assessmentGroup == null)
					throw new ArgumentException("Invalid AssessmentGroupID: " + (object)assessmentGroupView.ID);
				assessmentGroup.NameEn = assessmentGroupView.NameEN;
				assessmentGroup.NameAr = assessmentGroupView.NameAR;
				assessmentGroup.Color = assessmentGroupView.Color;
				assessmentGroup.AssessmentToolMatrixId = assessmentGroupView.AssessmentToolMatrixID;
				assessmentGroup.DateFrom = assessmentGroupView.DateFrom;
				assessmentGroup.DateTo = assessmentGroupView.DateTo;
				assessmentGroup.Color = assessmentGroupView.Color;
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<AssessmentGroupView>(assessmentGroupView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentGroupView>(e);
			}
		}

		public async Task<IAssessmentResponse<bool>> DeleteAssessmentGroup(int id)
		{
			try
			{
				AssessmentGroup entity = _appDbContext.AssessmentGroups.FirstOrDefault(x => x.Id == id);
				if (entity == null)
					return new AssessmentResponse<bool>(new ArgumentException("Invalid AssessmentGroupID: " + (object)id));
				if (entity.AssessmentGroupMembers.Count > 0)
					_appDbContext.AssessmentGroupMembers.RemoveRange(_appDbContext.AssessmentGroupMembers.Where(a => a.AssessmentGroupId == id));
				_appDbContext.AssessmentGroups.Remove(entity);
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentGroupView>> GetAssessmentGroup(int id)
		{
			try
			{
				AssessmentGroupView assessmentGroupView = await _appDbContext.AssessmentGroups.Where(b => b.Id == id).Select(t => new AssessmentGroupView
				{
					ID = t.Id,
					NameEN = t.NameEn,
					NameAR = t.NameAr,
					Color = t.Color,
					LogoID = new int?(t.LogoId),
					LogoIDEncrypted = this._encryptor.Encrypt(t.LogoId.ToString()),
					DateFrom = t.DateFrom,
					DateTo = t.DateTo,
					AssessmentToolMatrixID = t.AssessmentToolMatrixId,
				}).FirstOrDefaultAsync(e => e.ID == id);

				return new AssessmentResponse<AssessmentGroupView>(assessmentGroupView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentGroupView>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentIndividualToolReportView>> GenerateIndividualProfileReport(int ProfileAssessmentID)
		{
			try
			{
				var individualToolReportView = new AssessmentIndividualToolReportView();
				var profileAssessment = await _appDbContext.ProfileAssessmentToolScores.Include(x => x.Profile).Include(x => x.AssessmentTool).FirstOrDefaultAsync(e => e.Id == ProfileAssessmentID);
				if (profileAssessment != null)
				{
					int assessmentToolCategory = profileAssessment.AssessmentTool.AssessmentToolCategory;
					individualToolReportView.AssessmnetToolCategoryID = assessmentToolCategory;
					if (profileAssessment.Score > new Decimal(10))
						profileAssessment.Score = new Decimal(10);
					if (profileAssessment.Score < Decimal.One)
						profileAssessment.Score = Decimal.One;
					individualToolReportView.OverallScore = profileAssessment.Score;
					switch (assessmentToolCategory)
					{
						case 82002:

							var data = _appDbContext.ProfileAssessmentToolScores.Where(e => e.IsCompleted == true &&
							  e.AssessmentTool.AssessmentToolCategory == 82002 && e.AssessmentToolId == profileAssessment.AssessmentToolId)
							  .ToList();

							var groupedAssessmentScores = data.GroupBy(x => new { x.AssessmentToolId, x.ProfileId }).ToList();
							//var groupedAssessmentScores = _appDbContext.ProfileAssessmentToolScores.Include(x=>x.AssessmentTool).Where(e => e.IsCompleted == true && 
							//e.AssessmentTool.AssessmentToolCategory == 82002 && e.AssessmentToolId == profileAssessment.AssessmentToolId)
							//.GroupBy(e => 
							//	e.ProfileId
							//).ToList();
							var matrix = new List<Decimal>();
							foreach (var source in groupedAssessmentScores)
								matrix.Add(source.OrderByDescending(e => e.Order).FirstOrDefault().Score);
							var num1 = Math.Round(AssessmentCalculations.PercentRank(matrix, profileAssessment.Score) * new Decimal(100));
							individualToolReportView.OverallScore = num1;
							AssessmentNarrativeReport assessmentNarrativeReport1 = new AssessmentNarrativeReport();
							if (num1 >= Decimal.One && num1 <= new Decimal(20))
								assessmentNarrativeReport1 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 500);
							else if (num1 >= new Decimal(21) && num1 <= new Decimal(40))
								assessmentNarrativeReport1 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 501);
							else if (num1 >= new Decimal(41) && num1 <= new Decimal(60))
								assessmentNarrativeReport1 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 502);
							else if (num1 >= new Decimal(61) && num1 <= new Decimal(80))
								assessmentNarrativeReport1 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 503);
							else if (num1 >= new Decimal(81) && num1 <= new Decimal(100))
								assessmentNarrativeReport1 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 504);
							individualToolReportView.profileScaleNarrativeDescriptionsView.Add(new ProfileScaleNarrativeDescriptionView()
							{
								ScaleName = new EnglishArabicView()
								{
									English = "Overall Emotional Intelligence",
									Arabic = "الذكاء العاطفي العام"
								},
								Description = new EnglishArabicView()
								{
									English = assessmentNarrativeReport1.TextEn,
									Arabic = assessmentNarrativeReport1.TextAr
								}
							});
							break;
						case 82003:
							AssessmentNarrativeReport assessmentNarrativeReport2 = new AssessmentNarrativeReport();
							if (profileAssessment.Score >= Decimal.One && profileAssessment.Score <= new Decimal(2))
								assessmentNarrativeReport2 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 400);
							else if (profileAssessment.Score >= new Decimal(3) && profileAssessment.Score <= new Decimal(4))
								assessmentNarrativeReport2 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 401);
							else if (profileAssessment.Score >= new Decimal(5) && profileAssessment.Score <= new Decimal(6))
								assessmentNarrativeReport2 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 402);
							else if (profileAssessment.Score >= new Decimal(7) && profileAssessment.Score <= new Decimal(8))
								assessmentNarrativeReport2 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 403);
							else if (profileAssessment.Score >= new Decimal(9) && profileAssessment.Score <= new Decimal(10))
								assessmentNarrativeReport2 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 404);
							individualToolReportView.profileScaleNarrativeDescriptionsView.Add(new ProfileScaleNarrativeDescriptionView()
							{
								ScaleName = new EnglishArabicView()
								{
									English = "Overall Well-being",
									Arabic = "الرفاه العام"
								},
								Description = new EnglishArabicView()
								{
									English = assessmentNarrativeReport2.TextEn,
									Arabic = assessmentNarrativeReport2.TextAr
								}
							});
							break;
					}
					string englishName = profileAssessment.Profile.FirstNameEn + " " + profileAssessment.Profile.SecondNameEn + " " + profileAssessment.Profile.LastNameEn;
					string arabicName = profileAssessment.Profile.FirstNameAr + " " + profileAssessment.Profile.SecondNameAr + " " + profileAssessment.Profile.LastNameAr;

					individualToolReportView.ProfileName = new EnglishArabicView()
					{
						English = !string.IsNullOrWhiteSpace(englishName) ? englishName : arabicName,
						Arabic = !string.IsNullOrWhiteSpace(arabicName) ? arabicName : englishName
					};
					individualToolReportView.AssessmentToolName = new EnglishArabicView()
					{
						English = profileAssessment.AssessmentTool.NameEn,
						Arabic = profileAssessment.AssessmentTool.NameAr
					};
					individualToolReportView.CompletedOn = profileAssessment.Created.ToShortDateString();
					if (assessmentToolCategory == 82001 || assessmentToolCategory == 82003 || (assessmentToolCategory == 82002 || assessmentToolCategory == 82005))
					{
						List<ProfileScaleScore> list = _appDbContext.ProfileScaleScores.Include(x => x.Scale).Where(e => e.ProfileId == profileAssessment.ProfileId && e.Scale.Factor.AssessmentToolId == profileAssessment.AssessmentToolId && e.Order == profileAssessment.Order).ToList();
						foreach (ProfileScaleScore profileScaleScore in list)
						{
							var scaleScoreReportView = new ProfileScaleScoreReportView();
							scaleScoreReportView.ScaleName = new EnglishArabicView()
							{
								English = profileScaleScore.Scale.NameEn,
								Arabic = profileScaleScore.Scale.NameAr
							};
							scaleScoreReportView.ScaleID = profileScaleScore.ScaleId;
							scaleScoreReportView.ScaleLowDescription = new EnglishArabicView()
							{
								English = profileScaleScore.Scale.LowDescriptionEn,
								Arabic = profileScaleScore.Scale.LowDescriptionAr
							};
							scaleScoreReportView.ScaleHighDescription = new EnglishArabicView()
							{
								English = profileScaleScore.Scale.HighDescriptionEn,
								Arabic = profileScaleScore.Scale.HighDescriptionAr
							};
							scaleScoreReportView.Score = int.Parse(Math.Floor(profileScaleScore.Stenscore.Value).ToString());
							if (scaleScoreReportView.Score > 10)
								scaleScoreReportView.Score = 10;
							else if (scaleScoreReportView.Score < 1)
								scaleScoreReportView.Score = 1;
							individualToolReportView.profileScaleScoreReportsView.Add(scaleScoreReportView);
							ProfileScaleNarrativeDescriptionView narrativeDescriptionView = new ProfileScaleNarrativeDescriptionView();
							narrativeDescriptionView.ScaleName = new EnglishArabicView()
							{
								English = profileScaleScore.Scale.NameEn,
								Arabic = profileScaleScore.Scale.NameAr
							};
							switch (assessmentToolCategory)
							{
								case 82001:
								case 82002:
								case 82005:
									if (scaleScoreReportView.Score >= 1 && scaleScoreReportView.Score <= 3)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeLowDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeLowDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 4 && scaleScoreReportView.Score <= 7)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeAverageDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeAverageDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 8 && scaleScoreReportView.Score <= 10)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeHighDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeHighDescriptionAr
										};
										break;
									}
									break;
								case 82003:
									if (scaleScoreReportView.Score >= 1 && scaleScoreReportView.Score <= 2)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeLowDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeLowDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 3 && scaleScoreReportView.Score <= 4)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeBelowAverageDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeBelowAverageDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 5 && scaleScoreReportView.Score <= 6)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeAverageDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeAverageDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 7 && scaleScoreReportView.Score <= 8)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeAboveAverageDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeAboveAverageDescriptionAr
										};
										break;
									}
									if (scaleScoreReportView.Score >= 9 && scaleScoreReportView.Score <= 10)
									{
										narrativeDescriptionView.Description = new EnglishArabicView()
										{
											English = profileScaleScore.Scale.NarrativeHighDescriptionEn,
											Arabic = profileScaleScore.Scale.NarrativeHighDescriptionAr
										};
										break;
									}
									break;
							}
							individualToolReportView.profileScaleNarrativeDescriptionsView.Add(narrativeDescriptionView);
						}
						if (assessmentToolCategory == 82001 || assessmentToolCategory == 82005)
						{
							var assessmentReportFeedback = await _appDbContext.AssessmentReportFeedbacks
								.Include(x => x.QuestionAnswers)
								.ThenInclude(qa => qa.Question)
								.FirstOrDefaultAsync(e => e.ProfileAssesmentToolScoreId == ProfileAssessmentID);
							if (assessmentReportFeedback != null)
							{
								foreach (QuestionAnswer questionAnswer in assessmentReportFeedback.QuestionAnswers)
									individualToolReportView.ProfileAnswerQuestionGroups.Add(new ProfileAnswerQuestionGroupView()
									{
										Question = new EnglishArabicView()
										{
											English = questionAnswer.Question.TextEn,
											Arabic = questionAnswer.Question.TextAr
										},
										Answer = new EnglishArabicView()
										{
											English = questionAnswer.Text,
											Arabic = questionAnswer.Text
										}
									});
							}
							else
							{
								int num2;
								switch (assessmentToolCategory)
								{
									case 82001:
										num2 = AssessmentPersonalityIndividualReportQuestionGroup;
										break;
									case 82005:
										num2 = AssessmentCareerInterestIndividualReportQuestionGroup;
										break;
									default:
										num2 = 0;
										break;
								}
								int config = num2;
								var questions = await _appDbContext.Questions.Include(x => x.QuestionGroupsQuestions).Where(e => e.QuestionGroupsQuestions.Any(q => q.Group.Id == config)).ToListAsync();
								foreach (Question question in questions)
									individualToolReportView.ProfileAnswerQuestionGroups.Add(new ProfileAnswerQuestionGroupView()
									{
										Question = new EnglishArabicView()
										{
											English = question.TextEn,
											Arabic = question.TextAr
										}
									});
							}
						}
						if (assessmentToolCategory == 82005)
						{
							foreach (ProfileScaleScore profileScaleScore in list.OrderByDescending(e => e.Stenscore.Value).Take(3).ToList())
								individualToolReportView.profileHighestJobNarrativeDescriptionsView.Add(new ProfileScaleNarrativeDescriptionView()
								{
									ScaleName = new EnglishArabicView(profileScaleScore.Scale.NameEn, profileScaleScore.Scale.NameAr),
									Description = new EnglishArabicView()
									{
										English = profileScaleScore.Scale.HighestPreferedJobTypesDescriptionEn,
										Arabic = profileScaleScore.Scale.HighestPreferedJobTypesDescriptionAr
									}
								});
							foreach (ProfileScaleScore profileScaleScore in list.OrderBy(e => e.Stenscore.Value).Take(3).ToList())
								individualToolReportView.profileLowestJobNarrativeDescriptionsView.Add(new ProfileScaleNarrativeDescriptionView()
								{
									ScaleName = new EnglishArabicView(profileScaleScore.Scale.NameEn, profileScaleScore.Scale.NameAr),
									Description = new EnglishArabicView()
									{
										English = profileScaleScore.Scale.LowestPreferedJobTypesDescriptionEn,
										Arabic = profileScaleScore.Scale.LowestPreferedJobTypesDescriptionAr
									}
								});
						}
					}
					else
					{
						switch (assessmentToolCategory)
						{
							case 82006:

								var data = _appDbContext.ProfileAssessmentToolScores.Where(e => e.IsCompleted == true &&
							  e.AssessmentTool.AssessmentToolCategory == 82006 && e.AssessmentToolId == profileAssessment.AssessmentToolId)
							  .ToList();

								var queryable2 = data.GroupBy(x => new { x.AssessmentToolId, x.ProfileId }).ToList();

								List<Decimal> Scores = new List<Decimal>();
								foreach (var source in queryable2)
									Scores.Add(source.OrderByDescending(e => e.Order).FirstOrDefault().Score);
								individualToolReportView.PercentileScore = AssessmentCalculations.GetPercentile(Scores, profileAssessment.Score);
								int percentileScore = individualToolReportView.PercentileScore;
								string str1 = "";
								string str2 = "";
								if (percentileScore >= 0 && percentileScore <= 29)
								{
									str1 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 305)).TextEn;
									str2 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 305)).TextAr;
								}
								else if (percentileScore >= 30 && percentileScore <= 49)
								{
									str1 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 306)).TextEn;
									str2 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 306)).TextAr;
								}
								else if (percentileScore >= 50 && percentileScore <= 79)
								{
									str1 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 307)).TextEn;
									str2 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 307)).TextAr;
								}
								else if (percentileScore >= 80 && percentileScore <= 89)
								{
									str1 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 308)).TextEn;
									str2 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 308)).TextAr;
								}
								else if (percentileScore >= 90 && percentileScore <= 100)
								{
									str1 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 309)).TextEn;
									str2 = (await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 309)).TextAr;
								}
								individualToolReportView.OverAllPercentileDescription = new EnglishArabicView()
								{
									English = str1.Replace("x%", percentileScore.ToString() + "%").ToString(),
									Arabic = str2.Replace("x%", percentileScore.ToString() + "%").ToString()
								};
								var list1 = _appDbContext.ProfilePillarScores.Include(x => x.Pillar).Where(e => e.ProfileId == profileAssessment.ProfileId && e.AssessmenttoolId == profileAssessment.AssessmentToolId && e.Order == profileAssessment.Order).DistinctBy(x => x.PillarId).ToList();
								individualToolReportView.profilePillarScoreReportView = new List<ProfilePillarScoreReportView>();
								foreach (ProfilePillarScore profilePillarScore in list1)
								{
									ProfilePillarScoreReportView pillarScoreReportView = new ProfilePillarScoreReportView();
									pillarScoreReportView.PillarName = new EnglishArabicView()
									{
										English = profilePillarScore.Pillar.NameEn,
										Arabic = profilePillarScore.Pillar.NameAr
									};
									pillarScoreReportView.PillarDescription = new EnglishArabicView()
									{
										English = profilePillarScore.Pillar.DescriptionEn,
										Arabic = profilePillarScore.Pillar.DescriptionAr
									};
									pillarScoreReportView.Score = int.Parse(Math.Floor(profilePillarScore.Stenscore.Value).ToString());
									if (pillarScoreReportView.Score > 10)
										pillarScoreReportView.Score = 10;
									else if (pillarScoreReportView.Score < 1)
										pillarScoreReportView.Score = 1;
									individualToolReportView.profilePillarScoreReportView.Add(pillarScoreReportView);
								}
								AssessmentNarrativeReport assessmentNarrativeReport3 = new AssessmentNarrativeReport();
								if (profileAssessment.Score >= Decimal.One && profileAssessment.Score <= new Decimal(2))
									assessmentNarrativeReport3 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 300);
								else if (profileAssessment.Score >= new Decimal(3) && profileAssessment.Score <= new Decimal(4))
									assessmentNarrativeReport3 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 301);
								else if (profileAssessment.Score >= new Decimal(5) && profileAssessment.Score <= new Decimal(6))
									assessmentNarrativeReport3 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 302);
								else if (profileAssessment.Score >= new Decimal(7) && profileAssessment.Score <= new Decimal(8))
									assessmentNarrativeReport3 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 303);
								else if (profileAssessment.Score >= new Decimal(9) && profileAssessment.Score <= new Decimal(10))
									assessmentNarrativeReport3 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 304);
								individualToolReportView.OverAllDescription = new EnglishArabicView()
								{
									English = assessmentNarrativeReport3.TextEn,
									Arabic = assessmentNarrativeReport3.TextAr
								};
								break;
							case 82008:
								var narrativeDescriptionView1 = new ProfileCognitiveNarrativeDescriptionView();
								AssessmentNarrativeReport assessmentNarrativeReport4 = new AssessmentNarrativeReport();
								if (profileAssessment.Score >= Decimal.One && profileAssessment.Score <= new Decimal(2))
									assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 100);
								else if (profileAssessment.Score >= new Decimal(3) && profileAssessment.Score <= new Decimal(4))
									assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 101);
								else if (profileAssessment.Score >= new Decimal(5) && profileAssessment.Score <= new Decimal(6))
									assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 102);
								else if (profileAssessment.Score >= new Decimal(7) && profileAssessment.Score <= new Decimal(8))
									assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 103);
								else if (profileAssessment.Score >= new Decimal(9) && profileAssessment.Score <= new Decimal(10))
									assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 104);
								narrativeDescriptionView1.OverallScoreNarrativeDescription = new EnglishArabicView()
								{
									English = assessmentNarrativeReport4.TextEn,
									Arabic = assessmentNarrativeReport4.TextAr
								};
								var profilesubassessment = await _appDbContext.ProfileSubAssessmentToolScores.Include(x => x.SubAssessmenttool).Where(e => e.ProfileId == profileAssessment.ProfileId && e.Order == profileAssessment.Order && e.AssessmentToolId == profileAssessment.AssessmentToolId).ToListAsync();
								foreach (var assessmentToolScore in profilesubassessment)
								{
									Decimal num2 = Math.Floor(assessmentToolScore.Stenscore.GetValueOrDefault());
									if (num2 > new Decimal(10))
										num2 = new Decimal(10);
									else if (num2 < Decimal.One)
										num2 = Decimal.One;
									if (assessmentToolScore.SubAssessmenttool.Id == CognitiveVerbalID)
									{
										if (num2 >= Decimal.One && num2 <= new Decimal(2))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 110);
										else if (num2 >= new Decimal(3) && num2 <= new Decimal(4))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 111);
										else if (num2 >= new Decimal(5) && num2 <= new Decimal(6))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 112);
										else if (num2 >= new Decimal(7) && num2 <= new Decimal(8))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 113);
										else if (num2 >= new Decimal(9) && num2 <= new Decimal(10))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 114);
										narrativeDescriptionView1.VerbalScoreNarrativeDescription = new EnglishArabicView()
										{
											English = assessmentNarrativeReport4.TextEn,
											Arabic = assessmentNarrativeReport4.TextAr
										};
									}
									else if (assessmentToolScore.SubAssessmenttool.Id == CognitiveNumericalID)
									{
										if (num2 >= Decimal.One && num2 <= new Decimal(2))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 105);
										else if (num2 >= new Decimal(3) && num2 <= new Decimal(4))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 106);
										else if (num2 >= new Decimal(5) && num2 <= new Decimal(6))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 107);
										else if (num2 >= new Decimal(7) && num2 <= new Decimal(8))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 108);
										else if (num2 >= new Decimal(9) && num2 <= new Decimal(10))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 109);
										narrativeDescriptionView1.NumericalScoreNarrativeDescription = new EnglishArabicView()
										{
											English = assessmentNarrativeReport4.TextEn,
											Arabic = assessmentNarrativeReport4.TextAr
										};
									}
									else if (assessmentToolScore.SubAssessmenttool.Id == CognitiveAbstractID)
									{
										if (num2 >= Decimal.One && num2 <= new Decimal(2))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 115);
										else if (num2 >= new Decimal(3) && num2 <= new Decimal(4))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 116);
										else if (num2 >= new Decimal(5) && num2 <= new Decimal(6))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 117);
										else if (num2 >= new Decimal(7) && num2 <= new Decimal(8))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 118);
										else if (num2 >= new Decimal(9) && num2 <= new Decimal(10))
											assessmentNarrativeReport4 = await _appDbContext.AssessmentNarrativeReports.FirstOrDefaultAsync(x => x.Id == 119);
										narrativeDescriptionView1.AbstractScoreNarrativeDescription = new EnglishArabicView()
										{
											English = assessmentNarrativeReport4.TextEn,
											Arabic = assessmentNarrativeReport4.TextAr
										};
									}
								}
								individualToolReportView.profileCognitiveNarrativeDescriptionView = narrativeDescriptionView1;
								break;
						}
					}
				}
				return new AssessmentResponse<AssessmentIndividualToolReportView>(individualToolReportView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentIndividualToolReportView>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentIndividualToolReportView>> ExportAssessmentReportPDF(int ProfileAssessmentReport, string language = "AR")
		{
			try
			{
				var individualProfileReport = (await GenerateIndividualProfileReport(ProfileAssessmentReport)).Data;
				var report = new AssessmentIndividualToolReportView();
				switch (individualProfileReport.AssessmnetToolCategoryID)
				{
					case 82001:
						report = GenerateIndividualPersonalityPDF(individualProfileReport, language);
						break;
					case 82003:
						report = GenerateIndividualWellBeingPDF(individualProfileReport, language);
						break;
					case 82002:
						report = GenerateIndividualEQPDF(individualProfileReport, language);
						break;
					case 82008:
						report = GenerateIndividualCognitivePDF(individualProfileReport, language);
						break;
					case 82005:
						report = GenerateIndividualCareerPDF(individualProfileReport, language);
						break;
					case 82006:
						report = GenerateIndividualLeadershipPDF(individualProfileReport, language);
						break;
					default:
						report = new AssessmentIndividualToolReportView();
						break;
				}
				return new AssessmentResponse<AssessmentIndividualToolReportView>(report);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentIndividualToolReportView>(e);
			}
		}

		public async Task<IAssessmentResponse<AssessmentGroupProfileView>> GetProfileDrillDownByGroup(int ProfileID, int GroupID)
		{
			try
			{
				AssessmentGroupProfileView profileViewModel = new AssessmentGroupProfileView();

				List<AssessmentToolReportView> source = new List<AssessmentToolReportView>();

				var assessmentIdsattacheViewGrp = new List<int>();
				var assessmentGroup = await _appDbContext.AssessmentGroups
					.Include(x => x.AssessmentGroupMembers)
					.Include(x => x.AssessmentToolMatrix)
					.ThenInclude(x => x.MatricesTool)
					.FirstOrDefaultAsync(e => e.Id == GroupID && e.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID));
				if (assessmentGroup != null)
				{
					assessmentIdsattacheViewGrp.AddRange(assessmentGroup.AssessmentToolMatrix.MatricesTool.Select(e => e.AssessmentToolId).ToList());
					DateTime dateFrom = assessmentGroup.DateFrom;
					DateTime dateTo = assessmentGroup.DateTo;

					var list1 = _appDbContext.AssessmentTools.Where(e => assessmentIdsattacheViewGrp.Contains(e.Id)).Select(a => new
					{
						ToolNameEN = a.NameEn,
						ToolNameAR = a.NameAr,
						ID = a.Id,
						Category = a.AssessmentToolCategory,
						ValidityRangeNumber = a.ValidityRangeNumber
					}).ToList();

					var data1 = _appDbContext.Users.Where(e => e.Id == ProfileID).Select(p => new
					{
						ProfileNameEN = p.NameEn,
						profileNameAR = p.NameAr,
						Email = p.Username
					}).FirstOrDefault();

					var list2 = _appDbContext.ProfileAssessmentToolScores.Select(p => new
					{
						IsCompleted = p.IsCompleted,
						IsFailed = p.IsFailed,
						IsProcessing = p.IsProcessing,
						Score = p.Score,
						ProfileID = p.ProfileId,
						Order = p.Order,
						AssessmentToolID = p.AssessmentToolId,
						Created = p.Created,
						ValidityRangeNumber = p.AssessmentTool.ValidityRangeNumber,
						ID = p.Id,
						ProfileGroupAssessment = p.ProfileGroupAssessments.Select(pg => new
						{
							GroupID = pg.GroupId,
							ProfileID = pg.ProfileId,
							ProfileAssessmenttoolID = pg.ProfileAssessmenttoolId
						})
					}).Where(e => e.ProfileID == ProfileID && assessmentIdsattacheViewGrp.Contains(e.AssessmentToolID) &&
					e.ProfileGroupAssessment.Any(pg => pg.GroupID == GroupID && pg.ProfileID == ProfileID)).ToList();

					foreach (var data2 in list1)
					{
						var tool = data2;
						bool flag1 = false;
						bool flag2 = false;
						bool flag3 = false;
						int num = 0;
						var data3 = list2.FirstOrDefault();
						if (data3 != null)
						{
							if ((data3.IsCompleted != null && data3.IsCompleted.Value) || data3.IsProcessing)
							{
								if (data3.IsFailed.HasValue && data3.IsFailed.Value)
									flag2 = true;
								flag1 = true;
							}
							else
								flag3 = true;
							num = data3.Order;
						}

						source.Add(new AssessmentToolReportView()
						{
							IsFailed = flag2,
							IsCompleted = flag1,
							ProfileAssessmentID = flag3 ? data3.ID : 0,
							AssessmentToolID = tool.ID,
							AssessmentToolName = StringExtensions.GetLanguage(tool.ToolNameEN, tool.ToolNameAR),
							CompletedDate = flag3 | flag1 ? data3.Created.ToShortDateString() : "",
							IsExisted = data3 != null,
							InProgress = flag3,
							ProfileAssessmentIDEncrypted = data3 != null ? this._encryptor.Encrypt(data3.ID.ToString()) : "",
							IsCareer = tool.Category == 82005,
							IsPersonality = tool.Category == 82001,
							Order = num,
							AssessmneToolCategoryID = tool.Category,
							Groups = GetAssessmentGroupsview(tool.ID, ProfileID)
						});
					}
					profileViewModel.Reports = source.OrderBy(e => e.IsExisted).ToList();
					// ISSUE: reference to a compiler-generated field
					profileViewModel.GroupID = GroupID;
					// ISSUE: reference to a compiler-generated field
					profileViewModel.ProfileID = ProfileID;
					profileViewModel.ProfileName = data1 != null ? StringExtensions.GetLanguage(data1.ProfileNameEN, data1.profileNameAR) : "";
					profileViewModel.GroupName = StringExtensions.GetLanguage(assessmentGroup.NameEn, assessmentGroup.NameAr);
				}
				return new AssessmentResponse<AssessmentGroupProfileView>(profileViewModel);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<AssessmentGroupProfileView>(e);
			}
		}


		public AssessmentIndividualToolReportView GenerateIndividualWellBeingPDF(
		AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "WellBeingReport" + language.ToUpper() + ".cshtml";
			return list;
		}

		public AssessmentIndividualToolReportView GenerateIndividualCognitivePDF(
		  AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "CognitiveReportIndividual" + language.ToUpper() + ".cshtml";
			return list;
		}

		public AssessmentIndividualToolReportView GenerateIndividualEQPDF(
		  AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "EmoIntelReport" + language.ToUpper() + ".cshtml";
			return list;
		}

		public AssessmentIndividualToolReportView GenerateIndividualLeadershipPDF(
		  AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "ReportLeadership" + language.ToUpper() + ".cshtml";
			return list;
		}

		public AssessmentIndividualToolReportView GenerateIndividualPersonalityPDF(
		  AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "KnowYourselfIndividual" + language.ToUpper() + ".cshtml";
			return list;
		}

		public AssessmentIndividualToolReportView GenerateIndividualCareerPDF(
		  AssessmentIndividualToolReportView list, string language = "AR")
		{
			list.FileName = "-" + list.ProfileName.Current + "-" + list.AssessmentToolName.Current;
			list.ViewPath = "CareerReportIndividual" + language.ToUpper() + ".cshtml";
			return list;
		}

		public FileViewModel ExportGroupAssessmentToolsReport(int GroupID, string language)
		{
			DataTable dataTable = new DataTable("Group Sheet Report");
			dataTable.Columns.Add("Rank", typeof(int));
			dataTable.Columns.Add("User name");
			dataTable.Columns.Add("Assessment Completion");
			dataTable.Columns.Add("Total score", typeof(Decimal));
			dataTable.Columns.Add("Know yourself (Personality Traits)", typeof(int));
			dataTable.Columns.Add("Know your Well-Being (Happiness Level)", typeof(int));
			dataTable.Columns.Add("Know your Emotional Intelligence", typeof(int));
			dataTable.Columns.Add("Leadership Judgement Test", typeof(int));
			dataTable.Columns.Add("Career Interests", typeof(int));
			dataTable.Columns.Add("English Language Test", typeof(int));
			dataTable.Columns.Add("Cognitive", typeof(int));

			var members = _appDbContext.AssessmentGroups
				.Include(x => x.AssessmentGroupMembers)
				.FirstOrDefault(p => p.Id == GroupID).AssessmentGroupMembers
				.OrderByDescending(m => m.Rank).ThenByDescending(m => m.AssessmentCompletionPercentage).ToList();

			var profiles = new List<ProfileView>();
			foreach (var member in members)
			{
				var user = _appDbContext.Users.FirstOrDefault(x => x.Id == member.ProfileId);
				profiles.Add(new ProfileView
				{
					NameEN = user.NameEn,
					NameAR = user.NameAr,
					IsCoordinator = member.IsCoordinator,
					Id = member.ProfileId,
					AssessmentCompletionPercentage = member.AssessmentCompletionPercentage,
					Rank = member.Rank,
					AssessmentTotalCount = member.TotalCount,
					AssessmentCompletionCount = member.CompletedCount,
					TotalScore = new Decimal?(member.ProfileWeightedPercentage)
				});
			}

			foreach (var profile in profiles)
			{
				var list = _appDbContext.ProfileGroupAssessments
					.Include(x => x.ProfileAssessmenttool)
					.ThenInclude(x => x.AssessmentTool)
					.Where(e => e.GroupId == GroupID && e.ProfileId == profile.Id).ToList();
				var name = (language == "en") ? profile.NameEN : profile.NameAR;
				dataTable.Rows.Add(
					(object)profile.Rank,
					(object)name,
					(object)(profile.AssessmentCompletionCount.ToString() + "/" + (object)profile.AssessmentTotalCount),
					(object)profile.TotalScore.Value.ToString("0.#"),
					(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82001)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82003)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82002)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82006)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82005)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82007)?.ProfileAssessmenttool.Score,
				(object)list.FirstOrDefault(e => e.ProfileAssessmenttool.AssessmentTool.AssessmentToolCategory == 82005)?.ProfileAssessmenttool.Score);
			}
			var file = new FileViewModel();
			using (var xlWorkbook = new XLWorkbook())
			{
				xlWorkbook.Worksheets.Add(dataTable);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					xlWorkbook.SaveAs((Stream)memoryStream);
					string str = "GroupAssessmentToolsSheetReport";
					file.NameWithExtension = str + ".xlsx";
					file.Bytes = memoryStream.ToArray();
				}
			}
			return file;
		}



		private async Task AssignScorePerEachBlock(int ProfileID, int AssessmentToolID, int Order)
		{
			var source1 = _appDbContext.ProfileQuestionItemScores
				.Include(x => x.AssessmentTool)
				.Include(x => x.QuestionItem)
				.ThenInclude(x => x.Questionhead)
				.ThenInclude(x => x.Assessmentblock)
				.ThenInclude(x => x.SubAssessmenttool)
				.Where(e => e.AssessmentTool.AssessmentToolType.Value == 66004 && e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.Order == Order);

			foreach (var source2 in source1.Include(e => e.QuestionItem).ToList().GroupBy(e => new
			{
				SubAssessmenttoolID = e.QuestionItem.Questionhead.Assessmentblock.SubAssessmenttoolId,
				SubAssessmenttool = e.QuestionItem.Questionhead.Assessmentblock.SubAssessmenttool
			}))
			{
				Decimal num1 = source2.Sum(e => e.Score);
				source2.Count();
				int SubID = source2.Key.SubAssessmenttoolID;
				int profileID = ProfileID;
				int AssessmentID = AssessmentToolID;
				var subAssessmentToolScores = await _appDbContext.ProfileSubAssessmentToolScores.FirstOrDefaultAsync(e => e.ProfileId == profileID && e.Order == Order && e.SubAssessmenttoolId == SubID && e.AssessmentToolId == AssessmentID);
				if (subAssessmentToolScores == null)
				{
					try
					{
						Decimal num2 = num1;
						Decimal? nullable1 = source2.Key.SubAssessmenttool.Mean;
						Decimal? nullable2 = nullable1.HasValue ? new Decimal?(num2 - nullable1.GetValueOrDefault()) : new Decimal?();
						Decimal? standardDeviation = source2.Key.SubAssessmenttool.StandardDeviation;
						Decimal? nullable3;
						if (!(nullable2.HasValue & standardDeviation.HasValue))
						{
							nullable1 = new Decimal?();
							nullable3 = nullable1;
						}
						else
							nullable3 = new Decimal?(nullable2.GetValueOrDefault() / standardDeviation.GetValueOrDefault());
						Decimal? nullable4 = nullable3;
						ProfileSubAssessmentToolScore entity = new ProfileSubAssessmentToolScore
						{
							AssessmentToolId = AssessmentID,
							ProfileId = profileID,
							Score = num1,
							Order = Order,
							SubAssessmenttoolId = SubID,
							StandardScore = nullable4,
							Created = DateTime.Now,
							CreatedBy = "APP",
							Modified = DateTime.Now,
							ModifiedBy = "APP",
						};
						Decimal? nullable5 = nullable4;
						Decimal num3 = (Decimal)10;
						Decimal? nullable6;
						if (!nullable5.HasValue)
						{
							nullable1 = new Decimal?();
							nullable6 = nullable1;
						}
						else
							nullable6 = new Decimal?(nullable5.GetValueOrDefault() * num3);
						Decimal? nullable7 = nullable6;
						Decimal num4 = (Decimal)50;
						Decimal? nullable8;
						if (!nullable7.HasValue)
						{
							nullable5 = new Decimal?();
							nullable8 = nullable5;
						}
						else
							nullable8 = new Decimal?(nullable7.GetValueOrDefault() + num4);
						entity.Tscore = nullable8;
						nullable7 = nullable4;
						Decimal num5 = (Decimal)2;
						Decimal? nullable9;
						if (!nullable7.HasValue)
						{
							nullable5 = new Decimal?();
							nullable9 = nullable5;
						}
						else
							nullable9 = new Decimal?(nullable7.GetValueOrDefault() * num5 + new Decimal(55, 0, 0, false, (byte)1));
						entity.Stenscore = nullable9;
						entity.Mean = source2.Key.SubAssessmenttool.Mean;
						entity.StandardDeviation = source2.Key.SubAssessmenttool.StandardDeviation;
						_appDbContext.ProfileSubAssessmentToolScores.Add(entity);
						_appDbContext.SaveChanges();
					}
					catch (Exception e)
					{
					}
				}
			}
		}

		private async Task<List<ProfileView>> GetAssessmentGroupMembers(int assessmentGroupId)
		{
			try
			{
				var group = await _appDbContext.AssessmentGroups.FirstOrDefaultAsync(p => p.Id == assessmentGroupId);

				var members = _appDbContext.AssessmentGroupMembers.Where(p => p.AssessmentGroupId == assessmentGroupId)
					.OrderByDescending(m => m.Rank)
					.ThenByDescending(m => m.AssessmentCompletionPercentage).ToList();

				var profiles = new List<ProfileView>();
				foreach (var member in members)
				{
					var profile = await _appDbContext.Profiles.Include(x => x.IdNavigation).FirstOrDefaultAsync(x => x.Id == member.ProfileId);
					profiles.Add(new ProfileView()
					{
						Id = member.ProfileId,
						NameEN = member.Profile.IdNavigation.NameEn,
						NameAR = member.Profile.IdNavigation.NameAr,
						Email = member.Profile.IdNavigation.Username,
						IDEncrypted = this._encryptor.Encrypt(member.ProfileId.ToString()),
						IsCoordinator = member.IsCoordinator,
						AssessmentCompletionPercentage = member.AssessmentCompletionPercentage,
						Rank = member.Rank,
						AssessmentTotalCount = member.TotalCount,
						AssessmentCompletionCount = member.CompletedCount,
						UserImageFileId = profile.IdNavigation?.OriginalImageFileId ?? 0
					});
				}

				return profiles.ToList();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private int GetProfileAssessmentTool(int aId, int pId)
		{
			try
			{
				var profileAssessment = _appDbContext.ProfileAssessmentToolScores.FirstOrDefault(x => x.AssessmentToolId == aId && x.ProfileId == pId);

				if (profileAssessment != null) return profileAssessment.Id;
				return 0;
			}
			catch (Exception)
			{
				return 0;
			}
		}

		private int GetQuestionCountByAssessment(int AssessmentToolID)
		{
			ProfileAssessmentQuestionView profilequestitemVM = new ProfileAssessmentQuestionView();
			var assessmenttool = _appDbContext.AssessmentTools.Include(x => x.QuestionItems).FirstOrDefault(x => x.Id == AssessmentToolID);
			bool HasQuestionDirect = assessmenttool.HasQuestionDirect;
			bool hasSubScale = assessmenttool.HasSubscale;
			bool hasQuestionHead = assessmenttool.HasQuestionHead;
			IEnumerable<QuestionItem> questionitems;
			List<QuestionItem> QItems = new List<QuestionItem>();

			if (HasQuestionDirect)
			{
				questionitems = _appDbContext.QuestionItems.Where(e => e.AssessmentToolId == AssessmentToolID).ToList();
			}
			else if (hasQuestionHead)
			{
				//AssessmentTool assessmentTool = _appDbContext.AssessmentTools
				//								.Include(x => x.SubAssessmentTools)
				//								.ThenInclude(x => x.AssessmentBlocks)
				//								.ThenInclude(x => x.QuestionHeads)
				//								.ThenInclude(x => x.AssessmentTool)
				//								.ThenInclude(x => x.QuestionItems)
				//								.FirstOrDefault(e => e.Id == AssessmentToolID);
				//foreach (SubAssessmentTool subAssessmentTool in assessmentTool.SubAssessmentTools)
				//{
				//	List<AssessmentNavigationObjectView> randomlist = new List<AssessmentNavigationObjectView>();
				//	int? nullable = subAssessmentTool.SubAssessmentToolTypeId;
				//	int num1 = 84003;
				//		foreach (var item in subAssessmentTool.AssessmentBlocks)
				//		{
				//			var qhead = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).Include(x => x.AssessmentTool).Where(e => e.AssessmentblockId == item.Id).ToList();
				//			foreach (QuestionHead questionHead in qhead)
				//			{
				//				QuestionHead qh = questionHead;

				//				//List<int> list = qh.QuestionItems;
				//				if (qh.QuestionItems.Count != 0)
				//					qh.QuestionItems.ForEach((o => QItems.Add(o)));
				//			}
				//		}
				//}
				//questionitems = QItems;
				return 12;
			}
			else
			{
				if (hasSubScale)
				{
					questionitems = _appDbContext.QuestionItems
						.Include(x => x.Scale)
						.ThenInclude(x => x.Factor)
						.Where(e => e.SubScale.Scale.Factor.AssessmentToolId == AssessmentToolID).ToList();
				}
				else
				{
					questionitems = _appDbContext.QuestionItems
						.Include(x => x.Scale)
						.ThenInclude(x => x.Factor)
						.Where(e => e.Scale.Factor.AssessmentToolId == AssessmentToolID).ToList();
				}

			}
			return questionitems.Count();
		}

		private bool CheckCompetencyExist(int id)
		{
			AssessmentTool assessmentTool = _appDbContext.AssessmentTools.Include(x => x.QuestionItems).FirstOrDefault(x => x.Id == id);
			if (assessmentTool.HasSubscale)
			{
				var list = _appDbContext.CompetencySubscales.Include(x => x.Subscale)
					.ThenInclude(x => x.Scale).ThenInclude(x => x.Factor).Where(e => e.Subscale.Scale.Factor.AssessmentToolId == id)
					.Select(e => e.CompetencyId).Distinct().ToList();
				if (list != null && list.Count > 0 && (list.Count != 1 || list.FirstOrDefault().HasValue))
					return true;
			}
			else if (assessmentTool.HasQuestionDirect)
			{
				var list = assessmentTool.QuestionItems.Select(e => e.CompetencyId).Distinct().ToList();
				if (list != null && list.Count > 0 && (list.Count != 1 || list.FirstOrDefault<int?>().HasValue))
					return true;
			}
			return false;
		}

		private bool IsCordinator(int profileId)
		{
			var member = _appDbContext.AssessmentGroupMembers.FirstOrDefault(x => x.ProfileId == profileId);
			if (member == null) return false;
			return member.IsCoordinator;
		}

		private bool IsFeedbackSubmited(int scoreId)
		{
			var feedback = _appDbContext.AssessmentReportFeedbacks.FirstOrDefault(x => x.ProfileAssesmentToolScoreId == scoreId);
			if (feedback == null) return false;
			return true;
		}

		private List<AssessmentGroup> GetAssessmentGroups(long assessmentToolId, int ProfileID)
		{
			try
			{
				if (ProfileID == 0)
					return _appDbContext.AssessmentGroups.Where(x => x.AssessmentToolMatrixId == assessmentToolId).ToList();

				return _appDbContext.AssessmentGroups
					.Include(x => x.AssessmentGroupMembers)
					.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID) && (b.AssessmentToolMatrixId == assessmentToolId)).ToList();
			}
			catch
			{
				return new List<AssessmentGroup>();
			}
		}
		private List<AssessmentGroupView> GetAssessmentGroupsview(long assessmentToolId, int ProfileID)
		{
			try
			{
				if (ProfileID == 0)
					return _appDbContext.AssessmentGroups.Where(x => x.AssessmentToolMatrixId == assessmentToolId)
														.Select(t => new AssessmentGroupView
														{
															ID = t.Id,
															NameEN = t.NameEn,
															NameAR = t.NameAr,
															Color = t.Color,
															LogoID = t.LogoId,
															DateFrom = t.DateFrom,
															DateTo = t.DateTo,
															AssessmentToolMatrixID = t.AssessmentToolMatrixId
														}).ToList();

				//return _appDbContext.AssessmentGroups
				//	.Include(x => x.AssessmentGroupMembers)
				//	.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID) && (b.AssessmentToolMatrixId == assessmentToolId)).ToList();

				var Groups = _appDbContext.AssessmentGroups
						.Include(x => x.AssessmentGroupMembers).Include(x=> x.AssessmentToolMatrix).ThenInclude(o=>o.MatricesTool)
						.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID) && 
						(b.AssessmentToolMatrix.MatricesTool.Any(t=>t.AssessmentToolId == assessmentToolId)))
						.Select(t => new AssessmentGroupView
						{
							ID = t.Id,
							NameEN = t.NameEn,
							NameAR = t.NameAr,
							Color = t.Color,
							LogoID = t.LogoId,
							DateFrom = t.DateFrom,
							DateTo = t.DateTo,
							AssessmentToolMatrixID = t.AssessmentToolMatrixId
						}).ToList();

				return Groups;
			}
			catch
			{
				return new List<AssessmentGroupView>();
			}
		}

		private List<AssessmentGroupView> GetAssessmentGroupsById(long assessmentToolId, int ProfileID,int Gid)
		{
			try
			{
				if (ProfileID == 0)
					return _appDbContext.AssessmentGroups.Where(x => x.AssessmentToolMatrixId == assessmentToolId)
														.Select(t => new AssessmentGroupView
														{
															ID = t.Id,
															NameEN = t.NameEn,
															NameAR = t.NameAr,
															Color = t.Color,
															LogoID = t.LogoId,
															DateFrom = t.DateFrom,
															DateTo = t.DateTo,
															AssessmentToolMatrixID = t.AssessmentToolMatrixId
														}).ToList();

				//return _appDbContext.AssessmentGroups
				//	.Include(x => x.AssessmentGroupMembers)
				//	.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID) && (b.AssessmentToolMatrixId == assessmentToolId)).ToList();

				var Groups = _appDbContext.AssessmentGroups
						.Include(x => x.AssessmentGroupMembers).Include(x => x.AssessmentToolMatrix).ThenInclude(o => o.MatricesTool)
						.Where(b => b.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID) &&
						(b.AssessmentToolMatrix.MatricesTool.Any(t => t.AssessmentToolId == assessmentToolId)) && b.Id == Gid)
						.Select(t => new AssessmentGroupView
						{
							ID = t.Id,
							NameEN = t.NameEn,
							NameAR = t.NameAr,
							Color = t.Color,
							LogoID = t.LogoId,
							DateFrom = t.DateFrom,
							DateTo = t.DateTo,
							AssessmentToolMatrixID = t.AssessmentToolMatrixId
						}).ToList();

				return Groups;
			}
			catch
			{
				return new List<AssessmentGroupView>();
			}
		}
		private AssessmentTool GetAssessment(long assessmentToolId)
		{
			try
			{
				return _appDbContext.AssessmentTools.FirstOrDefault(x => x.Id == assessmentToolId);
			}
			catch
			{
				return null;
			}
		}

		private EnglishArabicView GetAssessmentName(long assessmentToolId)
		{
			try
			{
				var assessment = _appDbContext.AssessmentTools.FirstOrDefault(x => x.Id == assessmentToolId);
				return new EnglishArabicView()
				{
					Arabic = assessment.NameAr,
					English = assessment.NameEn
				};
			}
			catch
			{
				return new EnglishArabicView();
			}
		}


		private async Task AssignQuestionItemScores(List<QuestionScoreView> QuestionItemScores, UserInfo user, bool HasQuestionDirect, bool HasQuestionHead, int AssessmentToolID, int Order)
		{
			try
			{
				var profileId = user.UserId;
				var questionItemScoreList1 = new List<ProfileQuestionItemScore>();
				if (HasQuestionDirect)
				{
					var givenScoresID = QuestionItemScores.Select(e => e.Score).ToList();
					var list = _appDbContext.Qanswers.Where(e => givenScoresID.Contains(e.Id)).Select(p => new
					{
						ID = p.Id,
						Score = p.Score,
						IsCorrectAnswer = p.IsCorrectAnswer
					}).ToList();
					foreach (var questionItemScore1 in QuestionItemScores)
					{
						var quest = questionItemScore1;
						List<ProfileQuestionItemScore> questionItemScoreList2 = questionItemScoreList1;
						var questionItemScore2 = new ProfileQuestionItemScore
						{
							ProfileId = profileId,
							AssessmentToolId = AssessmentToolID,
							QuestionItemId = quest.ID,
							Created = DateTime.UtcNow,
							Modified = DateTime.UtcNow,
							CreatedBy = user.Email,
							ModifiedBy = user.Email
						};

						var data = list.FirstOrDefault(e => e.ID == quest.Score);
						if (data != null)
						{
							int? score = data.Score;
							int num;
							if (!score.HasValue)
							{
								num = data.IsCorrectAnswer ? 1 : 0;
							}
							else
							{
								score = data.Score;
								num = score.Value;
							}
							questionItemScore2.Score = (Decimal)num;
						}
						//questionItemScore2.AnswerId = new int?(quest.Score);
						questionItemScore2.Order = Order;
						questionItemScoreList2.Add(questionItemScore2);
					}
					_appDbContext.ProfileQuestionItemScores.AddRange(questionItemScoreList1);
				}
				else if (HasQuestionHead)
				{

					var givenScoresID = QuestionItemScores.Select(e => e.Score).ToList();
					var list = _appDbContext.Qanswers.Where(e => givenScoresID.Contains(e.Id)).Select(p => new
					{
						ID = p.Id,
						Score = p.Score,
						IsCorrectAnswer = p.IsCorrectAnswer
					}).ToList();
					foreach (var questionItemScore in QuestionItemScores)
					{
						var quest = questionItemScore;
						var data = list.FirstOrDefault(e => e.ID == quest.Score);
						var item = _appDbContext.ProfileQuestionItemScores.FirstOrDefault(a => a.ProfileId == profileId && a.AssessmentToolId == AssessmentToolID
						&& a.QuestionItemId == quest.ID);
						if (item == null)
						{
							questionItemScoreList1.Add(new ProfileQuestionItemScore()
							{
								ProfileId = profileId,
								AssessmentToolId = AssessmentToolID,
								QuestionItemId = quest.ID,
								Score = quest.Score == 0 ? Decimal.Zero : (Decimal)this.CalculateScore(quest.Score, quest.ID, quest.TimeTaken, data.IsCorrectAnswer),
								//AnswerId = quest.Score != 0 ? new int?(quest.Score) : new int?(),
								Order = Order,
								TimeAnswered = quest.TimeTaken,
								Created = DateTime.UtcNow,
								Modified = DateTime.UtcNow,
								CreatedBy = user.Email,
								ModifiedBy = user.Email
							});
						}
					}
					_appDbContext.ProfileQuestionItemScores.AddRange(questionItemScoreList1);
				}
				else
				{
					var QuestionsIDs = QuestionItemScores.Select(e => e.ID).ToList();
					var source = _appDbContext.QuestionItems.Where(e => QuestionsIDs.Contains(e.Id)).Select(e => new
					{
						ID = e.Id,
						IsNegativeDirection = e.IsNegativeDirection
					});
					foreach (QuestionScoreView questionItemScore in QuestionItemScores)
					{
						QuestionScoreView quest = questionItemScore;
						var data = source.FirstOrDefault(e => e.ID == quest.ID);
						questionItemScoreList1.Add(new ProfileQuestionItemScore
						{
							ProfileId = profileId,
							AssessmentToolId = AssessmentToolID,
							QuestionItemId = quest.ID,
							Score = (Decimal)(data.IsNegativeDirection ? SwitchScore(quest.Score) : quest.Score),
							Order = Order,
							Created = DateTime.UtcNow,
							Modified = DateTime.UtcNow,
							CreatedBy = user.Email,
							ModifiedBy = user.Email
						});
					}
					_appDbContext.ProfileQuestionItemScores.AddRange(questionItemScoreList1);
				}
				
				await _appDbContext.SaveChangesAsync();
			}

			catch (Exception e)
			{
				throw e;
			}
		}

		public async Task<IAssessmentResponse<bool>> IsCoordinator(int profileID)
		{
			try
			{
				var member = await _appDbContext.AssessmentGroupMembers.FirstOrDefaultAsync(x => x.ProfileId == profileID && x.IsCoordinator);
				return new AssessmentResponse<bool>((member != null && member.IsCoordinator == true));
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<bool>> MakeCoordinator(int profileID)
		{
			try
			{
				var member = await _appDbContext.AssessmentGroupMembers.FirstOrDefaultAsync(x => x.ProfileId == profileID);
				member.IsCoordinator = true;
				await _appDbContext.SaveChangesAsync();

				var groupName = await _appDbContext.AssessmentGroups.Where(x => x.Id == member.AssessmentGroupId).Select(x => x.NameEn).FirstOrDefaultAsync();
				var email = await _appDbContext.UserInfos.Where(k => k.UserId == member.ProfileId).Select(k => k.Email).FirstOrDefaultAsync();
				var firstName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.FirstNameEn).FirstOrDefaultAsync();
				var lastName = await _appDbContext.Profiles.Where(k => k.Id == member.ProfileId).Select(k => k.LastNameEn).FirstOrDefaultAsync();
				var userName = firstName + " " + lastName;
				var userId = await _appDbContext.UserInfos.Where(x => x.Email == member.CreatedBy).Select(x => x.UserId).FirstOrDefaultAsync();
				var notifyText = "You have been selected to be a coordinator in" + groupName;
				var customNotificationData = await _appDbContext.CustomNotifications.Where(x => x.ProfileID == member.ProfileId && x.CategoryID == (int)CategoryType.Assessments).FirstOrDefaultAsync();
				if (customNotificationData?.isEnabled == true || customNotificationData == null)
				{
					await AddNotificationAsync(member.ProfileId, ActionType.AddNewItem, member.Id, ParentType.AssessmentGroup, userId, notifyText);
				}

				await _emailService.SendAssessmentCoordinatorReminderAsync(email, groupName, userName);
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<bool>> RemoveCoordinator(int profileID)
		{
			try
			{
				var member = await _appDbContext.AssessmentGroupMembers.FirstOrDefaultAsync(x => x.ProfileId == profileID);
				member.IsCoordinator = false;
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<bool>> ToggleCoordintor(int memberID, int groupID)
		{
			try
			{
				AssessmentGroup assessmentGroup = _appDbContext.AssessmentGroups.Include(X => X.AssessmentGroupMembers).FirstOrDefault(G => G.Id == groupID);
				AssessmentGroupMember assessmentGroupMember = assessmentGroup.AssessmentGroupMembers.FirstOrDefault(m => m.ProfileId == memberID);
				assessmentGroupMember.IsCoordinator = !assessmentGroupMember.IsCoordinator;
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(e);
			}
		}

		public async Task<IAssessmentResponse<QuestionWithAnswerView>> GetQuestionsWithAnswers(int paidID, int userID)
		{
			try
			{
				QuestionWithAnswerView questionWithAnswerView = new QuestionWithAnswerView
				{
					ProfielID = userID,
					ProfileAssessmentToolID = paidID
				};
				ProfileAssessmentToolScore assessmentToolScore = await _appDbContext.ProfileAssessmentToolScores.Include(x => x.AssessmentTool).FirstOrDefaultAsync(p => p.Id == paidID);
				var num1 = assessmentToolScore.AssessmentTool.AssessmentToolCategory switch
				{
					82001 => AssessmentPersonalityIndividualReportQuestionGroup,
					82005 => AssessmentCareerInterestIndividualReportQuestionGroup,
					_ => 0,
				};
				int num2 = num1;
				var questionGroups = _appDbContext.QuestionGroups
					.Include(x => x.QuestionGroupsQuestions)
					.ThenInclude(x => x.Question).FirstOrDefault(x => x.Id == num2);


				questionWithAnswerView.Questions = questionGroups.QuestionGroupsQuestions.OrderBy(q => q.QuestionOrder).Select(q => new QuestionView()
				{
					TextAR = q.Question.TextAr,
					ID = q.Question.Id,
					TextEN = q.Question.TextEn,
					WordCount = new int?(q.Question.WordCount),
					QuestionTypeItemID = q.Question.QuestionTypeItemId,
					Options = q.Question.Options.Select(o => new OptionView()
					{
						ID = o.Id,
						QuestionID = o.QuestionId.Value,
						TextAR = o.TextAr,
						TextEN = o.TextEn,
						Value = o.Id
					}).ToList()
				}).ToList();

				var assessmentReportFeedback = assessmentToolScore.AssessmentReportFeedbacks.FirstOrDefault();
				if (assessmentReportFeedback != null)
				{
					List<QuestionAnswerView> answers = assessmentReportFeedback.QuestionAnswers.Select(m => new QuestionAnswerView()
					{
						QuestionId = m.QuestionId,
						Text = m.Text,
						Id = m.Id,
						SelectedOptionId = m.SelectedOptionId,
						YnquestionAnswer = m.YnquestionAnswer,
						Scale = m.Scale,
						AnswerFileId = m.AnswerFileId,
						QuestionTypeItemID = m.Question.QuestionTypeItemId
					}).ToList();

					if (answers != null)
					{
						questionWithAnswerView.Participated = true;
						var orderedAnswers = new List<QuestionAnswerView>();
						questionWithAnswerView.Questions.ForEach((Action<QuestionView>)(q => orderedAnswers.Add(answers.FirstOrDefault(o => o.QuestionId == q.ID) == null ? new QuestionAnswerView() : answers.FirstOrDefault(o => o.QuestionId == q.ID))));
						questionWithAnswerView.QuestionsAnswer = orderedAnswers.Select(an => new QuestionAnswerView()
						{
							ProfileId = userID,
							QuestionId = an.QuestionId,
							Text = an.Text,
							Id = an.Id,
							SelectedOptionId = new int?(an.SelectedOptionId.HasValue ? an.SelectedOptionId.Value : 0),
							YnquestionAnswer = an.YnquestionAnswer,
							Scale = an.Scale,
							AnswerFileId = an.AnswerFileId,
							QuestionTypeItemID = an.QuestionTypeItemID
						}).ToList();
					}
				}
				else
					questionWithAnswerView.QuestionsAnswer = new List<QuestionAnswerView>();
				while (questionWithAnswerView.QuestionsAnswer.Count < questionWithAnswerView.Questions.Count)
					questionWithAnswerView.QuestionsAnswer.Add(new QuestionAnswerView());
				if (questionWithAnswerView.QuestionsAnswer.Count <= 0)
					questionWithAnswerView.QuestionsAnswer = new List<QuestionAnswerView>(new QuestionAnswerView[questionWithAnswerView.Questions.Count]);
				return new AssessmentResponse<QuestionWithAnswerView>(questionWithAnswerView);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<QuestionWithAnswerView>(e);
			}
		}

		public async Task<IAssessmentResponse<bool>> AnswerFeedBackQuestions(QuestionWithAnswerView_New questionWithAnswerView, int userID)
		{
			try
			{
				var user = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == userID);
				var questionsAnswer = questionWithAnswerView.QuestionsAnswer;
				var paidID = questionWithAnswerView.ProfileAssessmentToolID;

				if (_appDbContext.AssessmentReportFeedbacks.Any(a => a.ProfileAssesmentToolScoreId == paidID && a.CreatedUserId == userID))
					return new AssessmentResponse<bool>(true);
				var assessmentReportFeedback = new AssessmentReportFeedback()
				{
					CreatedUserId = userID,
					IsAdmin = false,
					ProfileAssesmentToolScoreId = paidID,
					Created = DateTime.Now,
					Modified = DateTime.Now,
					CreatedBy = user.Email,
					ModifiedBy = user.Email
				};
				_appDbContext.AssessmentReportFeedbacks.Add(assessmentReportFeedback);
				foreach (var questionAnswerView in questionsAnswer)
				{
					QuestionAnswer entity = new QuestionAnswer
					{
						ProfileId = userID,
						QuestionId = questionAnswerView.QuestionId,
						AssessmentReportFeedback = assessmentReportFeedback,
						Text = questionAnswerView.Text,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = user.Email,
						ModifiedBy = user.Email
					};
					int? nullable1 = questionAnswerView.SelectedOptionId;
					int? nullable2;
					if (!nullable1.HasValue)
					{
						nullable1 = new int?();
						nullable2 = nullable1;
					}
					else
						nullable2 = questionAnswerView.SelectedOptionId;
					entity.SelectedOptionId = nullable2;
					nullable1 = questionAnswerView.Scale;
					int? nullable3;
					if (!nullable1.HasValue)
					{
						nullable1 = new int?();
						nullable3 = nullable1;
					}
					else
						nullable3 = questionAnswerView.Scale;
					entity.Scale = nullable3;
					int? nullable4;
					if (questionAnswerView.QuestionTypeItemID != 57005)
					{
						nullable1 = new int?();
						nullable4 = nullable1;
					}
					else
						nullable4 = null;
					entity.AnswerFileId = nullable4;
					entity.YnquestionAnswer = questionAnswerView.YnquestionAnswer;
					_appDbContext.QuestionAnswers.Add(entity);
				}
				await _appDbContext.SaveChangesAsync();
				return new AssessmentResponse<bool>(true);
			}
			catch (Exception e)
			{
				return new AssessmentResponse<bool>(false);
			}
		}


		private async Task<ProfileAssessmentHeadQuestionView> GetQuestionsByTest(int AssessmentToolID, int ProfileID, LanguageType lang = LanguageType.AR)
		{
			var questionViewModel1 = new ProfileAssessmentHeadQuestionView();
			List<AssessmentNavigationObjectView> navigationList = new List<AssessmentNavigationObjectView>();
			int? Order = new int?();
			if (_appDbContext.ProfileAssessmentToolScores.Any(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID))
			{
				var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == false).OrderByDescending(e => e.Order).FirstOrDefault();
				if (assessmentToolScore != null)
					Order = new int?(assessmentToolScore.Order);
			}
			if (Order.HasValue)
			{
				if (_appDbContext.ProfileQuestionItemScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.Order == Order.Value).ToList().Count > 0)
				{
					var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.FirstOrDefault(e => e.AssessmentToolId == AssessmentToolID && e.ProfileId == ProfileID && e.Order == Order.Value);
					if (assessmentToolScore != null)
					{
						assessmentToolScore.Score = Decimal.Zero;
						assessmentToolScore.IsCompleted = true;
						assessmentToolScore.IsFailed = new bool?(true);
						await _appDbContext.SaveChangesAsync();
						questionViewModel1.IsFailed = true;
						return questionViewModel1;
					}
				}
			}
			var assessmentTool = _appDbContext.AssessmentTools
				.Include(x => x.SubAssessmentTools)
				.Include(x => x.AssessmentBlocks)
				.ThenInclude(x => x.QuestionHeads)
				.ThenInclude(x => x.QuestionItems)
				.FirstOrDefault(e => e.Id == AssessmentToolID);
			foreach (var subAssessmentTool in assessmentTool.SubAssessmentTools)
			{
				if (subAssessmentTool.SubAssessmentToolTypeId.GetValueOrDefault() == (int)SubAssessmentToolType.Abstract & subAssessmentTool.SubAssessmentToolTypeId.HasValue)
				{
					foreach (var assessmentblock in subAssessmentTool.AssessmentBlocks)
					{
						var currentQh = assessmentblock.QuestionHeads.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
						questionViewModel1.TotalTestCount += currentQh.QuestionItems.Count;
						var questionViewModel2 = questionViewModel1;
						subAssessmentTool.SubAssessmentToolTypeId = currentQh.AssessmentTool.AssessmentToolType;
						int num2 = subAssessmentTool.SubAssessmentToolTypeId.Value;
						questionViewModel2.AssessmentToolType = num2;
						List<int> list = currentQh.QuestionItems.Select(e => e.Id).ToList();
						if (list.Count != 0)
							list.ForEach((Action<int>)(o => navigationList.Add(new AssessmentNavigationObjectView(currentQh.AssessmentblockId, currentQh.Assessmentblock.SubAssessmenttoolId, currentQh.Id, o))));
					}
				}
				else
				{
					var currentblock = subAssessmentTool.AssessmentBlocks.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
					var questionHeads = _appDbContext.QuestionHeads
						.Include(x => x.AssessmentTool)
						.Include(x => x.QuestionItems)
						.Where(e => e.AssessmentblockId == currentblock.Id).ToList();
					foreach (var questionHead in questionHeads)
					{
						var qh = questionHead;
						questionViewModel1.TotalTestCount += qh.QuestionItems.Count;
						var questionViewModel2 = questionViewModel1;
						subAssessmentTool.SubAssessmentToolTypeId = qh.AssessmentTool.AssessmentToolType;
						questionViewModel2.AssessmentToolType = subAssessmentTool.SubAssessmentToolTypeId.Value;
						var list = qh.QuestionItems.Select(e => e.Id).ToList();
						if (list.Count != 0)
							list.ForEach((Action<int>)(o => navigationList.Add(new AssessmentNavigationObjectView(qh.AssessmentblockId, qh.Assessmentblock.SubAssessmenttoolId, qh.Id, o))));
					}
				}
			}
			var currentItem = navigationList.FirstOrDefault();
			if (currentItem != null)
			{
				var questionHead1 = _appDbContext.QuestionHeads
					.Include(x => x.Assessmentblock)
					.ThenInclude(x => x.SubAssessmenttool)
					.Include(x => x.QuestionItems)
					.ThenInclude(qa => qa.Qanswers)
					.FirstOrDefault(e => e.Id == currentItem.QuestionHeadID);
				questionViewModel1.QuestionsHead = new QuickHeadTestView()
				{
					ID = questionHead1.Id,
					Name = new EnglishArabicView()
					{
						English = questionHead1.NameEn,
						Arabic = questionHead1.NameAr
					},
					//QHImage = questionHead1.ImageId.HasValue ? _fileService.GetFile(questionHead1.ImageId.Value) : new FileViewModel(),
					QHImageId = questionHead1.ImageId,
					QHMobImageId = questionHead1.MobImageID,
					BlockName = (lang == LanguageType.EN) ? questionHead1.Assessmentblock.NameEn : questionHead1.Assessmentblock.NameAr,
					SubAssessmentToolName = (lang == LanguageType.EN) ? questionHead1.Assessmentblock.SubAssessmenttool.NameEn : questionHead1.Assessmentblock.SubAssessmenttool.NameAr,
					SubAssessementToolTypeID = questionHead1.Assessmentblock.SubAssessmenttool.SubAssessmentToolTypeId.Value,
					Questions = questionHead1.QuestionItems.Where(q => q.Id == currentItem.QuestionID).Select(q => new QuestionItemView()
					{
						ID = q.Id,
						Name = new EnglishArabicView()
						{
							English = q.NameEn,
							Arabic = q.NameAr
						},
						//Image = q.ImageId.HasValue ? _fileService.GetFile(q.ImageId.Value) : new FileViewModel(),
						ImageID = q.ImageId,
						//MobImageID = q.MobImageId,
						TimeTaken = (q.TimeTaken != null) ? q.TimeTaken + 30 : q.TimeTaken,
						Answers = q.Qanswers.OrderBy(t => Guid.NewGuid()).Select(a => new QAnswerView()
						{
							ID = a.Id,
							Name = new EnglishArabicView()
							{
								Arabic = a.NameAr,
								English = a.NameEn
							},
							IsCorrectAnswer = a.IsCorrectAnswer,
							IDEncrypted = this._encryptor.Encrypt(a.Id.ToString()),
							//Image = a.ImageId.HasValue ? _fileService.GetFile(a.ImageId.Value) : new FileViewModel()
							MobImageID = a.MobImageID,
							ImageID = a.ImageId,
						}).ToList()
					}).FirstOrDefault()
				};
			}
			questionViewModel1.AssessmentToolID = AssessmentToolID;
			questionViewModel1.AssessmentToolName = (lang == LanguageType.EN) ? assessmentTool.NameEn : assessmentTool.NameAr;

			navigationList.Remove(currentItem);
			if (navigationList.Count == 0)
				questionViewModel1.IsLastCount = true;
			questionViewModel1._assessmentNavigationObject = navigationList;
			questionViewModel1.TotalAnsweredCount = questionViewModel1.TotalTestCount - navigationList.Count;
			var assessmentToolScore1 = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true).OrderByDescending(e => e.Order).FirstOrDefault();
			questionViewModel1.Order = assessmentToolScore1 != null ? assessmentToolScore1.Order + 1 : 1;
			return questionViewModel1;
		}

		private async Task<ProfileAssessmentQuestionView> GetQuestionsByTest(int AssessmentToolID, int ProfileID, bool isCompleted)
		{
			ProfileAssessmentQuestionView profilequestitemVM = new ProfileAssessmentQuestionView();
			var assessmenttool = (await GetAssessmentDetails(AssessmentToolID)).Data;
			bool HasQuestionDirect = assessmenttool.HasQuestionDirect;
			bool hasSubScale = assessmenttool.HasSubScale;
			profilequestitemVM.HasSubScale = hasSubScale;
			int? Order = null;
			int SkippedCounts = 0;
			if (!isCompleted)
			{
				IEnumerable<QuestionItemView> questionitems;

				if (HasQuestionDirect)
				{

					questionitems = _appDbContext.QuestionItems.Where(e => e.AssessmentToolId == AssessmentToolID).Select(e => new QuestionItemView()
					{
						ID = e.Id,
						IDEncrypted = _encryptor.Encrypt(e.Id.ToString()),
						Name = new EnglishArabicView()
						{
							Arabic = e.NameAr,
							English = e.NameEn
						},
						AssessmentToolID = e.AssessmentToolId,
						LevelID = e.LevelId,
						Answers = e.Qanswers.OrderBy(q => Guid.NewGuid()).Select(a => new QAnswerView()
						{
							ID = a.Id,
							Name = new EnglishArabicView()
							{
								Arabic = a.NameAr,
								English = a.NameEn
							},
							IsCorrectAnswer = a.IsCorrectAnswer,
							IDEncrypted = _encryptor.Encrypt(a.Id.ToString()),
						}).ToList(),

					}).OrderBy(e => e.LevelID);
				}
				else
				{
					if (hasSubScale)
					{
						questionitems = _appDbContext.QuestionItems
							.Include(x => x.SubScale)
							.ThenInclude(x => x.Scale)
							.ThenInclude(x => x.Factor)
							.OrderBy(q => Guid.NewGuid()).Where(e => e.SubScale.Scale.Factor.AssessmentToolId == AssessmentToolID)
							.Select(e => new QuestionItemView()
							{
								ID = e.Id,
								IDEncrypted = _encryptor.Encrypt(e.Id.ToString()),
								Name = new EnglishArabicView()
								{
									Arabic = e.NameAr,
									English = e.NameEn
								},
								SubScaleID = e.SubScaleId,
								ScaleID = e.SubScaleId,
								QuestionDirection = e.IsNegativeDirection ? QuestionItemDirection.N : QuestionItemDirection.P
							}).ToList();
					}
					else
					{
						questionitems = _appDbContext.QuestionItems
							.Include(x => x.Scale)
							.ThenInclude(x => x.Factor)
							.OrderBy(q => Guid.NewGuid())
							.Where(e => e.Scale.Factor.AssessmentToolId == AssessmentToolID)
							.Select(e => new QuestionItemView()
							{
								ID = e.Id,
								IDEncrypted = _encryptor.Encrypt(e.Id.ToString()),
								Name = new EnglishArabicView()
								{
									Arabic = e.NameAr,
									English = e.NameEn
								},
								SubScaleID = e.SubScaleId,
								ScaleID = e.SubScaleId,
								QuestionDirection = e.IsNegativeDirection ? QuestionItemDirection.N : QuestionItemDirection.P
							}).ToList();
					}

				}
				var profAssessmentObj = _appDbContext.ProfileAssessmentToolScores
					.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true)
					.OrderByDescending(e => e.Modified).FirstOrDefault();

				if (profAssessmentObj != null)
					Order = profAssessmentObj.Order;

				var QuestionIDs = Order.HasValue ? _appDbContext.ProfileQuestionItemScores
					.Where(e => e.ProfileId == ProfileID && e.Order > Order && e.AssessmentToolId == AssessmentToolID)
														  .Select(e => e.QuestionItemId).Distinct().ToList() :
														  _appDbContext.ProfileQuestionItemScores
														  .Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID)
														  .Select(e => e.QuestionItemId).Distinct().ToList();

				var _questionitemscores = new List<QuestionItemScoreView>();

				if (QuestionIDs != null && QuestionIDs.Count() > 0)
				{
					SkippedCounts = questionitems.Where(q => QuestionIDs.Contains(q.ID)).Count();
					_questionitemscores.AddRange(questionitems.Where(e => QuestionIDs.Contains(e.ID))
					  .Select(q => new QuestionItemScoreView()
					  {
						  ID = q.ID,
						  Question = q.Name,
						  Score = 0,
						  QuestionDirection = q.QuestionDirection,
						  Answers = q.Answers,
					  }
				  ));

					_questionitemscores.AddRange(questionitems.Where(e => !QuestionIDs.Contains(e.ID))
						.Select(q => new QuestionItemScoreView()
						{
							ID = q.ID,
							Question = q.Name,
							Score = 0,
							QuestionDirection = q.QuestionDirection,
							Answers = q.Answers,
						}
					));

				}
				else
				{
					_questionitemscores.AddRange(questionitems
						.Select(q => new QuestionItemScoreView()
						{
							ID = q.ID,
							Question = q.Name,
							Score = 0,
							QuestionDirection = q.QuestionDirection,
							Answers = q.Answers,
						}
					));

				}
				profilequestitemVM.TotalTestCount = _questionitemscores.Count;
				profilequestitemVM.QuestionIDs = string.Join(",", _questionitemscores.Select(e => e.ID).ToList());


				if (SkippedCounts != 0 && _questionitemscores.Count == 0)
				{
					profilequestitemVM.IsCompleted = true;
				}

				profilequestitemVM.QuestionItemScores = _questionitemscores;
			}
			else
			{
				profilequestitemVM.IsCompleted = true;
				profilequestitemVM.QuestionItemScores = new List<QuestionItemScoreView>();
			}

			profilequestitemVM.ProfileID = ProfileID;
			profilequestitemVM.AssessmentToolID = AssessmentToolID;
			profilequestitemVM.SkippedCount = SkippedCounts;
			profilequestitemVM.HasQuestionDirect = HasQuestionDirect;
			profilequestitemVM.AssessmentToolName = assessmenttool.Name.Current;
			profilequestitemVM.AssessmentToolTypeID = assessmenttool.AssessmentToolTypeID.Value;
			profilequestitemVM.AssessmentToolCategoryID = assessmenttool.AssessmentToolCategID;
			var profAssessment = _appDbContext.ProfileAssessmentToolScores
				.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true)
				.OrderByDescending(e => e.Order).FirstOrDefault();
			profilequestitemVM.Order = profAssessment != null ? profAssessment.Order + 1 : 1;
			return profilequestitemVM;
		}


		private async Task<ProfileAssessmentHeadQuestionView> GetQuestionsHeadByTest_New(int AssessmentToolID, int ProfileID)
		{
			ProfileAssessmentHeadQuestionView questionViewModel1 = new ProfileAssessmentHeadQuestionView();
			List<AssessmentNavigationObjectView> navigationList = new List<AssessmentNavigationObjectView>();
			int? Order = new int?();
			if (_appDbContext.ProfileAssessmentToolScores.Any(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID))
			{
				var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && !(e.IsCompleted == true)).OrderByDescending(e => e.Order).FirstOrDefault();
				if (assessmentToolScore != null)
					Order = new int?(assessmentToolScore.Order);
			}
			if (Order.HasValue)
			{
				if (_appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.Order == Order.Value).ToList().Count > 0)
				{
					ProfileAssessmentToolScore assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.FirstOrDefault(e => e.AssessmentToolId == AssessmentToolID && e.ProfileId == ProfileID && e.Order == Order.Value);
					if (assessmentToolScore != null)
					{
						assessmentToolScore.Score = Decimal.Zero;
						assessmentToolScore.IsCompleted = true;
						assessmentToolScore.IsFailed = new bool?(true);
						await _appDbContext.SaveChangesAsync();
						questionViewModel1.IsFailed = true;
						return questionViewModel1;
					}
				}
			}
			AssessmentTool assessmentTool = _appDbContext.AssessmentTools
				.Include(x => x.SubAssessmentTools)
				.ThenInclude(x => x.AssessmentBlocks)
				.ThenInclude(x => x.QuestionHeads)
				.ThenInclude(x => x.AssessmentTool)
				.ThenInclude(x => x.QuestionItems)
				.FirstOrDefault(e => e.Id == AssessmentToolID);
			foreach (SubAssessmentTool subAssessmentTool in assessmentTool.SubAssessmentTools)
			{
				List<AssessmentNavigationObjectView> randomlist = new List<AssessmentNavigationObjectView>();
				int? nullable = subAssessmentTool.SubAssessmentToolTypeId;
				int num1 = 84003;
				if (nullable.GetValueOrDefault() == num1 & nullable.HasValue)
				{
					
					foreach (var item in subAssessmentTool.AssessmentBlocks)
					{
						var qhead = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).Include(x => x.AssessmentTool).Where(e => e.AssessmentblockId == item.Id).ToList();
						foreach (QuestionHead questionHead in qhead)
						{
							QuestionHead qh = questionHead;
							List<int> list = qh.QuestionItems.Select(e => e.Id).ToList();
							if (list.Count != 0)
								list.ForEach((Action<int>)(o => randomlist.Add(new AssessmentNavigationObjectView(qh.AssessmentblockId, qh.Assessmentblock.SubAssessmenttoolId, qh.Id, o))));
						}
					}

					
					var abslist = randomlist.OrderBy(q => Guid.NewGuid()).Take(4).ToList();
					questionViewModel1.TotalTestCount += abslist.Count;
					foreach (var item1 in abslist)
					{
						navigationList.Add(item1);
					}

				}
				else
				{
					AssessmentBlock currentblock = subAssessmentTool.AssessmentBlocks.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
					var qhead = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).Include(x => x.AssessmentTool).Where(e => e.AssessmentblockId == currentblock.Id).ToList();
					foreach (QuestionHead questionHead in qhead)
					{
						QuestionHead qh = questionHead;
						questionViewModel1.TotalTestCount += qh.QuestionItems.Count;
						ProfileAssessmentHeadQuestionView questionViewModel2 = questionViewModel1;
						nullable = qh.AssessmentTool.AssessmentToolType;
						int num2 = nullable.Value;
						questionViewModel2.AssessmentToolType = num2;
						List<int> list = qh.QuestionItems.Select(e => e.Id).ToList();
						if (list.Count != 0)
							list.ForEach((Action<int>)(o => navigationList.Add(new AssessmentNavigationObjectView(qh.AssessmentblockId, qh.Assessmentblock.SubAssessmenttoolId, qh.Id, o))));
					}
				}
			}
			var currentItem = navigationList.FirstOrDefault();
			QuestionHead questionHead1 = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).ThenInclude(x => x.Qanswers).FirstOrDefault(e => e.Id == currentItem.QuestionHeadID);
			questionViewModel1.AssessmentToolID = AssessmentToolID;
			questionViewModel1.AssessmentToolName = StringExtensions.GetLanguage(assessmentTool.NameEn, assessmentTool.NameAr);
			questionViewModel1.QuestionsHead = new QuickHeadTestView()
			{
				ID = questionHead1.Id,
				Name = new EnglishArabicView()
				{
					English = questionHead1.NameEn,
					Arabic = questionHead1.NameAr
				},
				QHImageId = questionHead1.ImageId,
				QHMobImageId = questionHead1.MobImageID,
				BlockName = StringExtensions.GetLanguage(questionHead1.Assessmentblock.NameEn, questionHead1.Assessmentblock.NameAr),
				SubAssessmentToolName = StringExtensions.GetLanguage(questionHead1.Assessmentblock.SubAssessmenttool.NameEn, questionHead1.Assessmentblock.SubAssessmenttool.NameAr),
				SubAssessementToolTypeID = questionHead1.Assessmentblock.SubAssessmenttool.SubAssessmentToolTypeId.Value,
				Questions = questionHead1.QuestionItems.Where(q => q.Id == currentItem.QuestionID).Select(q => new QuestionItemView()
				{
					ID = q.Id,
					Name = new EnglishArabicView()
					{
						English = q.NameEn,
						Arabic = q.NameAr
					},
					ImageID = q.ImageId,
					//MobImageID = q.MobImageId,
					TimeTaken = (q.TimeTaken != null) ? q.TimeTaken + 30 : q.TimeTaken,
					Answers = q.Qanswers.OrderBy(t => Guid.NewGuid()).Select(a => new QAnswerView()
					{
						ID = a.Id,
						Name = new EnglishArabicView()
						{
							Arabic = a.NameAr,
							English = a.NameEn
						},
						IsCorrectAnswer = a.IsCorrectAnswer,
						IDEncrypted = this._encryptor.Encrypt(a.Id.ToString()),
						ImageID = a.ImageId,
						MobImageID = a.MobImageID
					}).ToList()
				}).FirstOrDefault()
			};
			navigationList.Remove(currentItem);
			if (navigationList.Count == 0)
				questionViewModel1.IsLastCount = true;
			questionViewModel1._assessmentNavigationObject = navigationList;
			questionViewModel1.TotalAnsweredCount = questionViewModel1.TotalTestCount - navigationList.Count;
			ProfileAssessmentToolScore assessmentToolScore1 = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true).OrderByDescending(e => e.Order).FirstOrDefault();
			questionViewModel1.Order = assessmentToolScore1 != null ? assessmentToolScore1.Order + 1 : 1;
			return questionViewModel1;
		}
		private async Task<ProfileAssessmentHeadQuestionView> GetQuestionsHeadByTest(int AssessmentToolID, int ProfileID)
		{
			ProfileAssessmentHeadQuestionView questionViewModel1 = new ProfileAssessmentHeadQuestionView();
			List<AssessmentNavigationObjectView> navigationList = new List<AssessmentNavigationObjectView>();
			int? Order = new int?();
			if (_appDbContext.ProfileAssessmentToolScores.Any(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID))
			{
				var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && !(e.IsCompleted == true)).OrderByDescending(e => e.Order).FirstOrDefault();
				if (assessmentToolScore != null)
					Order = new int?(assessmentToolScore.Order);
			}
			if (Order.HasValue)
			{
				if (_appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.Order == Order.Value).ToList().Count > 0)
				{
					ProfileAssessmentToolScore assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.FirstOrDefault(e => e.AssessmentToolId == AssessmentToolID && e.ProfileId == ProfileID && e.Order == Order.Value);
					if (assessmentToolScore != null)
					{
						assessmentToolScore.Score = Decimal.Zero;
						assessmentToolScore.IsCompleted = true;
						assessmentToolScore.IsFailed = new bool?(true);
						await _appDbContext.SaveChangesAsync();
						questionViewModel1.IsFailed = true;
						return questionViewModel1;
					}
				}
			}
			AssessmentTool assessmentTool = _appDbContext.AssessmentTools
				.Include(x => x.SubAssessmentTools)
				.ThenInclude(x => x.AssessmentBlocks)
				.ThenInclude(x => x.QuestionHeads)
				.ThenInclude(x => x.AssessmentTool)
				.ThenInclude(x => x.QuestionItems)
				.FirstOrDefault(e => e.Id == AssessmentToolID);
			foreach (SubAssessmentTool subAssessmentTool in assessmentTool.SubAssessmentTools)
			{
				int? nullable = subAssessmentTool.SubAssessmentToolTypeId;
				int num1 = 84003;
				if (nullable.GetValueOrDefault() == num1 & nullable.HasValue)
				{
					foreach (AssessmentBlock assessmentblock in subAssessmentTool.AssessmentBlocks)
					{
						QuestionHead currentQh = assessmentblock.QuestionHeads.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
						questionViewModel1.TotalTestCount += currentQh.QuestionItems.Count;
						ProfileAssessmentHeadQuestionView questionViewModel2 = questionViewModel1;
						nullable = currentQh.AssessmentTool.AssessmentToolType;
						int num2 = nullable.Value;
						questionViewModel2.AssessmentToolType = num2;
						List<int> list = currentQh.QuestionItems.Select(e => e.Id).ToList();
						if (list.Count != 0)
							list.ForEach((Action<int>)(o => navigationList.Add(new AssessmentNavigationObjectView(currentQh.AssessmentblockId, currentQh.Assessmentblock.SubAssessmenttoolId, currentQh.Id, o))));
					}

					

				}
				else
				{
					AssessmentBlock currentblock = subAssessmentTool.AssessmentBlocks.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
					var qhead = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).Include(x => x.AssessmentTool).Where(e => e.AssessmentblockId == currentblock.Id).ToList();
					foreach (QuestionHead questionHead in qhead)
					{
						QuestionHead qh = questionHead;
						questionViewModel1.TotalTestCount += qh.QuestionItems.Count;
						ProfileAssessmentHeadQuestionView questionViewModel2 = questionViewModel1;
						nullable = qh.AssessmentTool.AssessmentToolType;
						int num2 = nullable.Value;
						questionViewModel2.AssessmentToolType = num2;
						List<int> list = qh.QuestionItems.Select(e => e.Id).ToList();
						if (list.Count != 0)
							list.ForEach((Action<int>)(o => navigationList.Add(new AssessmentNavigationObjectView(qh.AssessmentblockId, qh.Assessmentblock.SubAssessmenttoolId, qh.Id, o))));
					}
				}
			}
			var currentItem = navigationList.FirstOrDefault();
			QuestionHead questionHead1 = _appDbContext.QuestionHeads.Include(x => x.QuestionItems).ThenInclude(x => x.Qanswers).FirstOrDefault(e => e.Id == currentItem.QuestionHeadID);
			questionViewModel1.AssessmentToolID = AssessmentToolID;
			questionViewModel1.AssessmentToolName = StringExtensions.GetLanguage(assessmentTool.NameEn, assessmentTool.NameAr);
			questionViewModel1.QuestionsHead = new QuickHeadTestView()
			{
				ID = questionHead1.Id,
				Name = new EnglishArabicView()
				{
					English = questionHead1.NameEn,
					Arabic = questionHead1.NameAr
				},
				QHImageId = questionHead1.ImageId,
				QHMobImageId = questionHead1.MobImageID,
				BlockName = StringExtensions.GetLanguage(questionHead1.Assessmentblock.NameEn, questionHead1.Assessmentblock.NameAr),
				SubAssessmentToolName = StringExtensions.GetLanguage(questionHead1.Assessmentblock.SubAssessmenttool.NameEn, questionHead1.Assessmentblock.SubAssessmenttool.NameAr),
				SubAssessementToolTypeID = questionHead1.Assessmentblock.SubAssessmenttool.SubAssessmentToolTypeId.Value,
				Questions = questionHead1.QuestionItems.Where(q => q.Id == currentItem.QuestionID).Select(q => new QuestionItemView()
				{
					ID = q.Id,
					Name = new EnglishArabicView()
					{
						English = q.NameEn,
						Arabic = q.NameAr
					},
					ImageID = q.ImageId,
					//MobImageID = q.MobImageId,
					TimeTaken = (q.TimeTaken != null) ? q.TimeTaken + 30 : q.TimeTaken,
					Answers = q.Qanswers.OrderBy(t => Guid.NewGuid()).Select(a => new QAnswerView()
					{
						ID = a.Id,
						Name = new EnglishArabicView()
						{
							Arabic = a.NameAr,
							English = a.NameEn
						},
						IsCorrectAnswer = a.IsCorrectAnswer,
						IDEncrypted = this._encryptor.Encrypt(a.Id.ToString()),
						ImageID = a.ImageId,
						MobImageID = a.MobImageID
					}).ToList()
				}).FirstOrDefault()
			};
			navigationList.Remove(currentItem);
			if (navigationList.Count == 0)
				questionViewModel1.IsLastCount = true;
			questionViewModel1._assessmentNavigationObject = navigationList;
			questionViewModel1.TotalAnsweredCount = questionViewModel1.TotalTestCount - navigationList.Count;
			ProfileAssessmentToolScore assessmentToolScore1 = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true).OrderByDescending(e => e.Order).FirstOrDefault();
			questionViewModel1.Order = assessmentToolScore1 != null ? assessmentToolScore1.Order + 1 : 1;
			return questionViewModel1;
		}

		private async Task<ProfileAssessmentQuestionView> GetByTestFiltered(int AssessmentToolID, int ProfileID, bool hasSubScale, bool HasQuestionDirect, int Skip, string QuestionIDs)
		{
			var questionViewModel = new ProfileAssessmentQuestionView();
			List<int> ExistedQuestionIDs = QuestionIDs.Split(',').Select(x => Convert.ToInt32(x)).ToList();
			questionViewModel.TotalTestCount = ExistedQuestionIDs.Count;
			questionViewModel.HasQuestionDirect = HasQuestionDirect;
			if (ExistedQuestionIDs.Skip<int>(Skip + 5).Count<int>() <= 5)
				questionViewModel.IsLastCount = true;
			ExistedQuestionIDs = ExistedQuestionIDs.Skip(Skip + 5).Take(5).ToList();
			IEnumerable<QuestionItemView> source1;
			if (HasQuestionDirect)
				source1 = _appDbContext.QuestionItems.Where(e => ExistedQuestionIDs.Contains(e.Id)).Select(e => new QuestionItemView()
				{
					ID = e.Id,
					IDEncrypted = this._encryptor.Encrypt(e.Id.ToString()),
					Name = new EnglishArabicView()
					{
						Arabic = e.NameAr,
						English = e.NameEn
					},
					AssessmentToolID = e.AssessmentToolId,
					LevelID = e.LevelId,
					Answers = e.Qanswers.Select(a => new QAnswerView()
					{
						ID = a.Id,
						Name = new EnglishArabicView()
						{
							Arabic = a.NameAr,
							English = a.NameEn
						},
						IsCorrectAnswer = a.IsCorrectAnswer,
						IDEncrypted = this._encryptor.Encrypt(a.Id.ToString())
					}).OrderBy(q => Guid.NewGuid()).ToList()
				});
			else
				source1 = _appDbContext.QuestionItems.Where(e => ExistedQuestionIDs.Contains(e.Id)).Select(e => new QuestionItemView()
				{
					ID = e.Id,
					IDEncrypted = this._encryptor.Encrypt(e.Id.ToString()),
					Name = new EnglishArabicView()
					{
						Arabic = e.NameAr,
						English = e.NameEn
					},
					SubScaleID = e.SubScaleId,
					ScaleID = e.SubScaleId,
					QuestionDirection = e.IsNegativeDirection ? QuestionItemDirection.N : QuestionItemDirection.P
				});
			var source2 = new List<QuestionItemScoreView>();
			source2.AddRange(source1.Select(q => new QuestionItemScoreView()
			{
				ID = q.ID,
				Question = q.Name,
				Score = 0,
				QuestionDirection = q.QuestionDirection,
				Answers = q.Answers
			}));

			questionViewModel.QuestionItemScores = source2.ToList();
			questionViewModel.ProfileID = ProfileID;
			questionViewModel.AssessmentToolID = AssessmentToolID;
			questionViewModel.SkippedCount = Skip + 5;
			var typeAndCurrentName = GetToolTypeAndCurrentName(AssessmentToolID);
			questionViewModel.AssessmentToolName = typeAndCurrentName.Name.Current;
			questionViewModel.AssessmentToolTypeID = typeAndCurrentName.AssessmentToolTypeID.Value;
			questionViewModel.AssessmentToolCategoryID = typeAndCurrentName.AssessmentToolCategID;
			var assessmentToolScore = await _appDbContext.ProfileAssessmentToolScores
				.Where(e => e.ProfileId == ProfileID && e.AssessmentToolId == AssessmentToolID && e.IsCompleted == true)
				.OrderByDescending(e => e.Order).FirstOrDefaultAsync();
			questionViewModel.Order = assessmentToolScore != null ? assessmentToolScore.Order + 1 : 1;
			return questionViewModel;
		}

		private async Task<ProfileAssessmentHeadQuestionView> GetByTestFiltered(List<AssessmentNavigationObjectView> _AssessmentNavigationObjectView, int TotalTestCount, int Order)
		{
			try
			{
				ProfileAssessmentHeadQuestionView questionViewModel = new ProfileAssessmentHeadQuestionView();
				AssessmentNavigationObjectView currentItem = _AssessmentNavigationObjectView.FirstOrDefault();
				QuestionHead questionHead = await _appDbContext.QuestionHeads
					.Include(x => x.AssessmentTool)
					.Include(x => x.Assessmentblock)
					.ThenInclude(x => x.SubAssessmenttool)
					.Include(x => x.QuestionItems)
					.ThenInclude(x => x.Qanswers)
					.FirstOrDefaultAsync(e => e.Id == currentItem.QuestionHeadID);
				questionViewModel.QuestionsHead = new QuickHeadTestView()
				{
					ID = questionHead.Id,
					SubAssessmentToolName = StringExtensions.GetLanguage(questionHead.Assessmentblock.SubAssessmenttool.NameEn, questionHead.Assessmentblock.SubAssessmenttool.NameAr),
					BlockName = StringExtensions.GetLanguage(questionHead.Assessmentblock.NameEn, questionHead.Assessmentblock.NameAr),
					SubAssessementToolTypeID = questionHead.Assessmentblock.SubAssessmenttool.SubAssessmentToolTypeId.Value,
					QHImageId = questionHead.ImageId,
					QHMobImageId = questionHead.MobImageID,
					Name = new EnglishArabicView()
					{
						English = questionHead.NameEn,
						Arabic = questionHead.NameAr
					},
					Questions = questionHead.QuestionItems.Where(q => q.Id == currentItem.QuestionID).Select(q => new QuestionItemView()
					{
						ID = q.Id,
						Name = new EnglishArabicView()
						{
							English = q.NameEn,
							Arabic = q.NameAr
						},
						ImageID = q.ImageId,
						//MobImageID = q.MobImageId,
						TimeTaken = (q.TimeTaken != null) ? q.TimeTaken + 30 : q.TimeTaken,
						Answers = q.Qanswers.OrderBy(t => Guid.NewGuid()).Select(a => new QAnswerView()
						{
							ID = a.Id,
							Name = new EnglishArabicView()
							{
								Arabic = a.NameAr,
								English = a.NameEn
							},
							IsCorrectAnswer = a.IsCorrectAnswer,
							IDEncrypted = _encryptor.Encrypt(a.Id.ToString()),
							ImageID = a.ImageId,
							MobImageID = a.MobImageID
						}).ToList()
					}).FirstOrDefault()
				};

				_AssessmentNavigationObjectView.Remove(currentItem);

				if (_AssessmentNavigationObjectView.Count == 0)
					questionViewModel.IsLastCount = true;
				questionViewModel._assessmentNavigationObject = _AssessmentNavigationObjectView;
				questionViewModel.TotalTestCount = TotalTestCount;
				questionViewModel.TotalAnsweredCount = TotalTestCount - _AssessmentNavigationObjectView.Count;
				questionViewModel.AssessmentToolID = questionHead.AssessmentToolId;
				questionViewModel.Order = Order;
				questionViewModel.AssessmentToolType = questionHead.AssessmentTool.AssessmentToolType.Value;
				return questionViewModel;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private AssessmentToolView GetToolTypeAndCurrentName(int ID)
		{
			AssessmentToolView assessmentToolView = new AssessmentToolView();
			var data = _appDbContext.AssessmentTools.Where(e => e.Id == ID).Select(e => new
			{
				AssessmentToolType = e.AssessmentToolType,
				NameEN = e.NameEn,
				NameAR = e.NameAr,
				AssessmentToolCategory = e.AssessmentToolCategory
			}).FirstOrDefault();
			if (data == null)
				throw new ArgumentException("Invalid assessmenttoolID: " + (object)ID);
			assessmentToolView.AssessmentToolTypeID = data.AssessmentToolType;
			assessmentToolView.Name = new EnglishArabicView();
			assessmentToolView.Name.English = data.NameEN;
			assessmentToolView.AssessmentToolCategID = data.AssessmentToolCategory;
			assessmentToolView.Name.Arabic = data.NameAR;
			return assessmentToolView;
		}

		private int SwitchScore(int Score)
		{
			switch (Score)
			{
				case 1:
					return 5;
				case 2:
					return 4;
				case 3:
					return 3;
				case 4:
					return 2;
				case 5:
					return 1;
				default:
					return 0;
			}
		}

		private double CalculateScore(int AnswerID, int DefaultQuestionTimeInSec, int? timeTaken, bool isCorrectAnswer)
		{
			int? nullable1 = timeTaken;
			int num1 = DefaultQuestionTimeInSec;
			if (nullable1.GetValueOrDefault() >= num1 & nullable1.HasValue && AnswerID == 0)
				return 0.0;
			int? nullable2 = timeTaken;
			int num2 = 60;
			if (nullable2.GetValueOrDefault() < num2 & nullable2.HasValue)
				return !isCorrectAnswer ? 0.0 : 1.2;
			nullable2 = timeTaken;
			int num3 = 60;
			if (!(nullable2.GetValueOrDefault() >= num3 & nullable2.HasValue))
			{
				nullable2 = timeTaken;
				int num4 = 89;
				if (!(nullable2.GetValueOrDefault() <= num4 & nullable2.HasValue))
				{
					nullable2 = timeTaken;
					int num5 = 90;
					if (!(nullable2.GetValueOrDefault() >= num5 & nullable2.HasValue))
					{
						nullable2 = timeTaken;
						int num6 = 120;
						if (!(nullable2.GetValueOrDefault() <= num6 & nullable2.HasValue))
							return 1.0;
					}
					return !isCorrectAnswer ? 0.0 : 0.98;
				}
			}
			return isCorrectAnswer ? 1.0 : 0.0;
		}

		private async Task<ProfileSectionsPercentageDistributionVM> ReCalculateCompleteness(int profileID)
		{
			var percentageDistributionVm = new ProfileSectionsPercentageDistributionVM();
			foreach (KeyValuePair<ProfileSectionPercentage, int> sectionPercentage in ProfileSectionPercentages)
				percentageDistributionVm.SectionsWithPercentage.Add(new ProfileSectionsPercentageDistributionView()
				{
					Section = sectionPercentage.Key,
					Percentage = (Decimal)sectionPercentage.Value,
					Done = false
				});
			percentageDistributionVm.Percentage = Decimal.Zero;
			var profile = _appDbContext.Profiles.FirstOrDefault(p => p.Id == profileID);
			var user = _appDbContext.Users.FirstOrDefault(x => x.Id == profileID);
			var userInfo = _appDbContext.UserInfos.FirstOrDefault(x => x.UserId == profileID);
			if (profile.FirstNameEn != null && profile.SecondNameEn != null && (profile.ThirdNameEn != null && profile.LastNameEn != null) &&
				(profile.FirstNameAr != null && profile.SecondNameAr != null && (profile.ThirdNameAr != null && profile.LastNameAr != null)) &&
				(profile.ResidenceCountryId.HasValue && profile.Address != null && profile.PassportNumber != null) &&
				(profile.PassportIssueEmirateItemId.HasValue && (profile.BirthDate.HasValue && userInfo.Mobile != null && profile.Eid != null)))
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.BasicInfoPrcange).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.BasicInfoPrcange).Done = true;
			}
			if (user.LargeImageFileId.HasValue && user.SmallImageFileId.HasValue && user.OriginalImageFileId.HasValue)
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.profileImg).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.profileImg).Done = true;
			}
			if (profile.ProfileEducations.Count > 0)
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.EducationPrcange).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.EducationPrcange).Done = true;
			}
			if (profile.ProfileWorkExperiences.Count > 0)
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.WorkExpeincePrcange).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.WorkExpeincePrcange).Done = true;
			}
			if (profile.ProfileTrainings.Count > 0)
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.TrainingPrcange).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.TrainingPrcange).Done = true;
			}
			if (profile.ProfileAchievements.Count > 0)
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.AchievementPrcange).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.AchievementPrcange).Done = true;
			}
			AssessmentTool assessmentTool1 = _appDbContext.AssessmentTools.FirstOrDefault(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Personality);
			AssessmentTool assessmentTool2 = _appDbContext.AssessmentTools.FirstOrDefault(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.EQ);
			AssessmentTool assessmentTool3 = _appDbContext.AssessmentTools.FirstOrDefault(ass => ass.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing);
			if (assessmentTool1 != null && (profile.ProfileAssessmentToolScores.OrderByDescending(ass => ass.Order).FirstOrDefault(a => a.AssessmentTool.AssessmentToolCategory == (int)AssessmentToolCategory.Personality && a.IsCompleted == true)) != null && !CheckAssessmentValidity(assessmentTool1.Id, profileID, assessmentTool1.ValidityRangeNumber))
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.PersonalityAssessment).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.PersonalityAssessment).Done = true;
			}
			if (assessmentTool2 != null && (profile.ProfileAssessmentToolScores.OrderByDescending(ass => ass.Order).FirstOrDefault(a => a.AssessmentTool.AssessmentToolCategory == (int)AssessmentToolCategory.EQ && a.IsCompleted == true)) != null && !CheckAssessmentValidity(assessmentTool2.Id, profileID, assessmentTool2.ValidityRangeNumber))
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.EQAssessment).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.EQAssessment).Done = true;
			}
			if (assessmentTool3 != null && (profile.ProfileAssessmentToolScores.OrderByDescending(ass => ass.Order).FirstOrDefault(a => a.AssessmentTool.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing && a.IsCompleted == true)) != null && !CheckAssessmentValidity(assessmentTool3.Id, profileID, assessmentTool3.ValidityRangeNumber))
			{
				percentageDistributionVm.Percentage += percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.WellbeingAssessment).Percentage;
				percentageDistributionVm.SectionsWithPercentage.FirstOrDefault(s => s.Section == ProfileSectionPercentage.WellbeingAssessment).Done = true;
			}
			profile.CompletenessPercentage = percentageDistributionVm.Percentage;
			await _appDbContext.SaveChangesAsync();

			return percentageDistributionVm;
		}

		private async Task UpdateProfileCompletionAfterSubmission(int ProfileID, int AssessmentToolID, LanguageType lang)
		{
			await UpdateProfileApplicationAsessmentTool(ProfileID, AssessmentToolID, lang);
			await UpdateProfileCompletionforGroups(ProfileID);
			await UpdateProfileCompletionforAssignedAssessments(ProfileID);
			await UpdateProfileCompletionforOverAll(ProfileID);
		}

		private async Task UpdateProfileCompletionforGroups(int ProfileID)
		{
			foreach (AssessmentGroup assessmentGroup in _appDbContext.AssessmentGroups
				.Include(x => x.AssessmentGroupMembers)
				.Include(x => x.AssessmentToolMatrix)
				.ThenInclude(x => x.MatricesTool)
				.Where(e => e.AssessmentGroupMembers.Any(m => m.ProfileId == ProfileID)).ToList())
				await UpdateProfileCompletionbyGroup(ProfileID, assessmentGroup);
		}

		private async Task UpdateProfileCompletionforAssignedAssessments(int ProfileID)
		{
			foreach (AssignedAssessment assignedAssessment in _appDbContext.AssignedAssessments
				.Include(x => x.AssessmentToolMatrix)
				.ThenInclude(x => x.MatricesTool)
				.Where(e => e.ProfileId == ProfileID).ToList())
				await UpdateProfileCompletionforAssignedAssessmentsforAssignedAssessment(ProfileID, assignedAssessment);
		}

		private async Task UpdateProfileCompletionforAssignedAssessmentsforAssignedAssessment(int ProfileID, AssignedAssessment assignedAssessment)
		{
			var assessmentIdsattacheViewAssignedAssessments = new List<int>();
			assessmentIdsattacheViewAssignedAssessments.AddRange(assignedAssessment.AssessmentToolMatrix.MatricesTool
				.Select(e => e.AssessmentToolId).ToList());
			DateTime startDate = assignedAssessment.DateFrom;
			DateTime endDate = assignedAssessment.DateTo;
			var list = _appDbContext.ProfileAssessmentToolScores.Where(e => e.ProfileId == ProfileID && assessmentIdsattacheViewAssignedAssessments.Contains(e.AssessmentToolId) && e.Created <= endDate).Select(p => new
			{
				IsCompleted = p.IsCompleted,
				IsFailed = p.IsFailed,
				IsProcessing = p.IsProcessing,
				Score = p.Score,
				ProfileID = p.ProfileId,
				Order = p.Order,
				AssessmentToolID = p.AssessmentToolId,
				Created = p.Created,
				ValidityRangeNumber = p.AssessmentTool.ValidityRangeNumber,
				ID = p.Id
			}).ToList();
			var num1 = new Decimal();
			List<Decimal> source = new List<Decimal>();
			int num2 = 0;
			List<bool> boolList = new List<bool>();
			List<int> ProfileAssessmentIDs = new List<int>();
			if (list.Count > 0)
			{
				foreach (int num3 in assessmentIdsattacheViewAssignedAssessments)
				{
					int assessmentId = num3;
					var data = list.Where(e => e.AssessmentToolID == assessmentId && e.Created >= startDate.AddMonths(-e.ValidityRangeNumber)).OrderByDescending(e => e.Order).FirstOrDefault();
					if (data != null)
					{
						if (data.IsCompleted == true || data.IsProcessing)
						{
							++num2;
							boolList.Add(true);
							ProfileAssessmentIDs.Add(data.ID);
						}
						else
							boolList.Add(false);
						if (data.IsCompleted == true)
						{
							int? weight = assignedAssessment.AssessmentToolMatrix.MatricesTool.FirstOrDefault(e => e.AssessmentToolId == assessmentId)?.Weight;
							Decimal? nullable = weight.HasValue ? new Decimal?((Decimal)weight.GetValueOrDefault()) : new Decimal?();
							if (nullable.HasValue)
							{
								Decimal num4 = data.Score * nullable.Value / new Decimal(100);
								source.Add(num4);
							}
						}
					}
				}
			}
			Decimal num5 = (Decimal)num2 / (Decimal)assessmentIdsattacheViewAssignedAssessments.Count * new Decimal(100);
			if (!boolList.Contains(false) && boolList.Count == assessmentIdsattacheViewAssignedAssessments.Count)
			{
				assignedAssessment.IsAssessmentCompleted = true;
				assignedAssessment.AssessmentCompletionDatetime = new DateTime?(DateTime.Now);
				assignedAssessment.ProfileWeightedPercentage = source.Sum() * new Decimal(10);
			}
			else
			{
				assignedAssessment.IsAssessmentCompleted = false;
				assignedAssessment.AssessmentCompletionDatetime = new DateTime?();
				assignedAssessment.ProfileWeightedPercentage = Decimal.Zero;
			}
			await AddProfileAssignedAssessments(ProfileID, assignedAssessment.Id, ProfileAssessmentIDs);
			assignedAssessment.AssessmentCompletionPercentage = num5;
			await _appDbContext.SaveChangesAsync();
		}

		private async Task AddProfileAssignedAssessments(int ProfileID, int AssignedAssessmentID, List<int> ProfileAssessmentIDs)
		{
			foreach (int profileAssessmentId in ProfileAssessmentIDs)
			{
				int profileAssessmentID = profileAssessmentId;
				if (_appDbContext.ProfileAssignedAssessments.FirstOrDefault(e => e.ProfileId == ProfileID && e.AssignedAssessmentId == AssignedAssessmentID && e.ProfileAssessmenttoolId == profileAssessmentID) == null)
					_appDbContext.ProfileAssignedAssessments.Add(new ProfileAssignedAssessment()
					{
						ProfileId = ProfileID,
						AssignedAssessmentId = AssignedAssessmentID,
						ProfileAssessmenttoolId = profileAssessmentID
					});
			}
			await _appDbContext.SaveChangesAsync();
		}

		private async Task UpdateProfileCompletionforOverAll(int ProfileID)
		{
			var matrix = _appDbContext.AssessmentToolMatrices
				.Include(x => x.MatricesTool)
				.ThenInclude(x => x.AssessmentTool)
				.FirstOrDefault(e => e.IsOverall);
			if (matrix == null)
				return;
			List<int> list1 = matrix.MatricesTool.Select(e => e.AssessmentTool.AssessmentToolCategory).ToList();
			bool flag = false;
			List<Decimal> source1 = new List<Decimal>();
			foreach (int num1 in list1)
			{
				int assessmentCateg = num1;
				var data = _appDbContext.ProfileAssessmentToolScores
					.Include(x => x.AssessmentTool)
					.Where(e => e.AssessmentTool.AssessmentToolCategory == assessmentCateg && e.ProfileId == ProfileID && e.IsCompleted == true)
					.OrderByDescending(e => e.Created).Select(e => new
					{
						e.Score,
						e.AssessmentTool
					}).FirstOrDefault();
				if (data != null)
				{
					flag = true;
					Decimal num2 = data.Score * GetAssessmentWeightOverall(data.AssessmentTool.AssessmentToolCategory, matrix) / new Decimal(100);
					source1.Add(num2);
				}
				else
				{
					flag = false;
					break;
				}
			}
			if (!flag)
				return;
			var rankingPerBatchMatrix1 = _appDbContext.ReportAssessmentPercentageRankingPerBatchMatrices
				.Where(e => e.ProfileId == ProfileID && e.IsOverView).FirstOrDefault();
			if (rankingPerBatchMatrix1 == null)
				_appDbContext.ReportAssessmentPercentageRankingPerBatchMatrices.Add(new ReportAssessmentPercentageRankingPerBatchMatrix()
				{
					ProfileId = ProfileID,
					ProfilePercentage = source1.Sum() * new Decimal(10),
					IsOverView = true
				});
			else
				rankingPerBatchMatrix1.ProfilePercentage = source1.Sum() * new Decimal(10);
			await _appDbContext.SaveChangesAsync();
			var list2 = _appDbContext.ReportAssessmentPercentageRankingPerBatchMatrices.Where(e => e.IsOverView).OrderByDescending(e => e.ProfilePercentage).ToList();
			for (int index = 0; index < list2.Count<ReportAssessmentPercentageRankingPerBatchMatrix>(); ++index)
				list2[index].Rank = index + 1;
			foreach (var source2 in list2.GroupBy(m => m.ProfilePercentage).ToList())
			{
				if (source2.Count() > 1)
				{
					int rank = source2.FirstOrDefault().Rank;
					foreach (ReportAssessmentPercentageRankingPerBatchMatrix rankingPerBatchMatrix2 in source2.ToList())
						rankingPerBatchMatrix2.Rank = rank;
				}
			}
			await _appDbContext.SaveChangesAsync();
			var list3 = _appDbContext.ReportAssessmentPercentageRankingPerBatchMatrices
				.Where(e => e.IsOverView).Select(e => e.ProfilePercentage).ToList();
			var percentageOverview = _appDbContext.ReportAssessmentPercentageOverviews
				.Where(e => e.TabItemId == 97004).FirstOrDefault();
			if (percentageOverview == null)
			{
				_appDbContext.ReportAssessmentPercentageOverviews.Add(new ReportAssessmentPercentageOverview()
				{
					BestScorePercentage = list3.Count > 0 ? list3.Max() : Decimal.Zero,
					AverageScorePercentage = list3.Count > 0 ? list3.Sum() / (Decimal)list3.Count : Decimal.Zero,
					TabItemId = 97004
				});
			}
			else
			{
				percentageOverview.BestScorePercentage = list3.Count > 0 ? list3.Max() : Decimal.Zero;
				percentageOverview.AverageScorePercentage = list3.Count > 0 ? list3.Sum() / (Decimal)list3.Count : Decimal.Zero;
			}
			await _appDbContext.SaveChangesAsync();
		}

		private Decimal GetAssessmentWeightOverall(int AssessmentCategoryID, AssessmentToolMatrix matrix)
		{
			switch (AssessmentCategoryID)
			{
				case (int)AssessmentToolCategory.Personality:
				case (int)AssessmentToolCategory.EQ:
				case (int)AssessmentToolCategory.Wellbeing:
				case (int)AssessmentToolCategory.Leadership:
				case (int)AssessmentToolCategory.EnglishLanguage:
				case (int)AssessmentToolCategory.Cognitive:
					return (Decimal)matrix.MatricesTool.FirstOrDefault(e => e.AssessmentTool.AssessmentToolCategory == AssessmentCategoryID).Weight;
				default:
					return Decimal.Zero;
			}
		}

		private async Task UpdateProfileCompletionbyGroup(int ProfileID, AssessmentGroup assessmentGroup)
		{
			List<int> assessmentIdsattacheViewGrp = new List<int>();
			var assessmentGroupMember = assessmentGroup.AssessmentGroupMembers.FirstOrDefault(e => e.ProfileId == ProfileID);
			assessmentIdsattacheViewGrp.AddRange(assessmentGroup.AssessmentToolMatrix.MatricesTool.Select(e => e.AssessmentToolId).ToList());
			DateTime startDate = assessmentGroup.DateFrom;
			DateTime endDate = assessmentGroup.DateTo;
			var list = _appDbContext.ProfileAssessmentToolScores
				.Where(e => e.ProfileId == ProfileID && assessmentIdsattacheViewGrp.Contains(e.AssessmentToolId) && e.Created <= endDate)
				.Select(p => new
				{
					IsCompleted = p.IsCompleted,
					IsFailed = p.IsFailed,
					IsProcessing = p.IsProcessing,
					Score = p.Score,
					ProfileID = p.ProfileId,
					Order = p.Order,
					AssessmentToolID = p.AssessmentToolId,
					Created = p.Created,
					ValidityRangeNumber = p.AssessmentTool.ValidityRangeNumber,
					ID = p.Id
				}).ToList();
			Decimal num1 = new Decimal();
			List<Decimal> source = new List<Decimal>();
			int num2 = 0;
			List<bool> boolList = new List<bool>();
			List<int> ProfileAssessmentIDs = new List<int>();
			if (list.Count() > 0)
			{
				foreach (int num3 in assessmentIdsattacheViewGrp)
				{
					int assessmentId = num3;
					var data = list.Where(e => e.AssessmentToolID == assessmentId && e.Created >= startDate.AddMonths(-e.ValidityRangeNumber))
						.OrderByDescending(e => e.Order).FirstOrDefault();
					if (data != null)
					{
						if (data.IsCompleted == true || data.IsProcessing)
						{
							++num2;
							boolList.Add(true);
							ProfileAssessmentIDs.Add(data.ID);
						}
						else
							boolList.Add(false);
						if (data.IsCompleted == true)
						{
							int? weight = assessmentGroup.AssessmentToolMatrix.MatricesTool.FirstOrDefault(e => e.AssessmentToolId == assessmentId)?.Weight;
							Decimal? nullable = weight.HasValue ? new Decimal?((Decimal)weight.GetValueOrDefault()) : new Decimal?();
							if (nullable.HasValue)
							{
								Decimal num4 = data.Score * nullable.Value / new Decimal(100);
								source.Add(num4);
							}
						}
					}
				}
			}
			Decimal num5 = (Decimal)num2 / (Decimal)assessmentIdsattacheViewGrp.Count * new Decimal(100);
			if (!boolList.Contains(false) && boolList.Count == assessmentIdsattacheViewGrp.Count)
			{
				assessmentGroupMember.IsAssessmentCompleted = true;
				assessmentGroupMember.AssessmentCompletionDatetime = new DateTime?(DateTime.Now);
				assessmentGroupMember.ProfileWeightedPercentage = source.Sum() * new Decimal(10);
			}
			else
			{
				assessmentGroupMember.IsAssessmentCompleted = false;
				assessmentGroupMember.AssessmentCompletionDatetime = new DateTime?();
				assessmentGroupMember.ProfileWeightedPercentage = Decimal.Zero;
			}
			await this.AddProfileGroupAssessments(ProfileID, assessmentGroup.Id, ProfileAssessmentIDs);
			assessmentGroupMember.AssessmentCompletionPercentage = num5;
			assessmentGroupMember.CompletedCount = num2;
			assessmentGroupMember.TotalCount = assessmentIdsattacheViewGrp.Count;
			await _appDbContext.SaveChangesAsync();
			await this.UpdateAssessmentGroupRanking(assessmentGroup);
		}

		private async Task AddProfileGroupAssessments(int ProfileID, int GroupID, List<int> ProfileAssessmentIDs)
		{
			foreach (int profileAssessmentId in ProfileAssessmentIDs)
			{
				int profileAssessmentID = profileAssessmentId;
				if (_appDbContext.ProfileGroupAssessments
					.FirstOrDefault(e => e.ProfileId == ProfileID && e.GroupId == GroupID && e.ProfileAssessmenttoolId == profileAssessmentID) == null)
					_appDbContext.ProfileGroupAssessments.Add(new ProfileGroupAssessment()
					{
						ProfileId = ProfileID,
						GroupId = GroupID,
						ProfileAssessmenttoolId = profileAssessmentID
					});
			}
			await _appDbContext.SaveChangesAsync();
		}

		private async Task UpdateAssessmentGroupRanking(AssessmentGroup AssesmentGroup)
		{
			List<AssessmentGroupMember> list = AssesmentGroup.AssessmentGroupMembers.Where(e => e.IsAssessmentCompleted)
				.OrderByDescending(e => e.ProfileWeightedPercentage).ToList();
			for (int index = 0; index < list.Count<AssessmentGroupMember>(); ++index)
				list[index].Rank = index + 1;
			foreach (var source in list.GroupBy(m => m.ProfileWeightedPercentage).ToList())
			{
				if (source.Count<AssessmentGroupMember>() > 1)
				{
					int rank = source.FirstOrDefault<AssessmentGroupMember>().Rank;
					foreach (AssessmentGroupMember assessmentGroupMember in source.ToList<AssessmentGroupMember>())
						assessmentGroupMember.Rank = rank;
				}
			}
			if (list.Count != 0)
			{
				AssesmentGroup.AverageAssessmentScore = new Decimal?(list.Select<AssessmentGroupMember, Decimal>((Func<AssessmentGroupMember, Decimal>)(e => e.ProfileWeightedPercentage)).Sum() / (Decimal)list.Count);
				AssesmentGroup.BestAssessmentScore = new Decimal?(list.Select<AssessmentGroupMember, Decimal>((Func<AssessmentGroupMember, Decimal>)(e => e.ProfileWeightedPercentage)).Max());
			}
			await _appDbContext.SaveChangesAsync();
		}

		private async Task UpdateProfileApplicationAsessmentTool(int ProfileID, int AssessemntToolID, LanguageType lang)
		{
			await UpdateProfileScoring(new ProfileScore?(ProfileScore.Assessment), ProfileID);
			if (_appDbContext.AssessmentTools.Where(e => e.Id == AssessemntToolID).Select(x => x.BatchAssessmentTools).FirstOrDefault() != null)
				await UpdateOpenedAppsPercentgeByCommonData(63009, ProfileID, lang);
			await UpdateApplicationProfileAssessments(ProfileID);
		}

		private async Task UpdateApplicationProfileAssessments(int ProfileID)
		{
			var list1 = _appDbContext.Applications
				.Include(x => x.Batch)
				.ThenInclude(x => x.BatchAssessmentTools)
				.ThenInclude(x => x.AssessmentTool)
				.ThenInclude(x => x.ProfileAssessmentToolScores)
				.Where(e => e.ProfileId == ProfileID && e.StatusItemId.HasValue && e.StatusItemId.Value == 59004).ToList();
			if (list1.Count() <= 0)
				return;
			foreach (Application application1 in list1)
			{
				var boolList = new List<bool>();
				var ProfileAssessmentIDs = new List<int>();
				if (application1.Batch.BatchAssessmentTools != null)
				{
					List<AssessmentTool> list2 = application1.Batch.BatchAssessmentTools.Select(e => e.AssessmentTool).ToList();
					DateTime? startDate = application1.Batch.AssessmentStartDate;
					foreach (AssessmentTool assessmentTool in list2)
					{
						DateTime ValidDate = DateTime.Now.AddMonths(-assessmentTool.ValidityRangeNumber);
						ProfileAssessmentToolScore assessmentToolScore = assessmentTool.ProfileAssessmentToolScores
							.OrderByDescending(e => e.Order).FirstOrDefault(e =>
						{
							if (e.ProfileId != ProfileID)
								return false;
							DateTime created = e.Created;
							DateTime? nullable = startDate;
							return (nullable.HasValue ? (created >= nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0 || e.Created >= ValidDate;
						});
						if (assessmentToolScore != null)
						{
							boolList.Add(assessmentToolScore.IsCompleted == true);
							ProfileAssessmentIDs.Add(assessmentToolScore.Id);
						}
					}
					if (!boolList.Contains(false) && boolList.Count == list2.Count)
					{
						application1.AssessmentItemId = new int?(62002);
						application1.AssessmentCompletionDatetime = new DateTime?(DateTime.Now);
						await _appDbContext.SaveChangesAsync();
						await AddProfileBatchAssessments(application1.ProfileId, application1.BatchId, application1.Id, ProfileAssessmentIDs);
					}
					else
					{
						int? nullable1 = application1.AssessmentItemId;
						int num = 62002;
						if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
						{
							Application application2 = application1;
							nullable1 = new int?();
							int? nullable2 = nullable1;
							application2.AssessmentItemId = nullable2;
							application1.AssessmentCompletionDatetime = new DateTime?();
							await _appDbContext.SaveChangesAsync();
						}
					}
					await this.UpdateApplicationsByBatchRanking(application1.BatchId);
				}
			}
		}

		private async Task UpdateApplicationsByBatchRanking(int BatchID, string lang = "ar")
		{
			var batch1 = _appDbContext.Batches
				.Include(x => x.AssessmentMatrix)
				.ThenInclude(x => x.MatricesTool)
				.Include(x => x.BatchAssessmentTools)
				.ThenInclude(x => x.AssessmentTool)
				.FirstOrDefault(b => b.Id == BatchID);
			var assessmentToolsReport = new BatchAssessmentToolsReportView();
			assessmentToolsReport.BatchAssessmentToolsProfiles.Data = new List<BatchAssessmentToolsProfilesPV>();
			if (batch1 == null)
				throw new ArgumentException("Invalid BatchID: " + (object)BatchID);
			List<AssessmentToolCategoryView> assessmentToolCategoryViewList = new List<AssessmentToolCategoryView>();
			if (batch1.BatchAssessmentTools.Count > 0)
				assessmentToolCategoryViewList = batch1.BatchAssessmentTools
					.Select(x => x.AssessmentTool).Select(e => new AssessmentToolCategoryView()
					{
						AssessmentToolID = e.Id,
						AssessmentToolCategoryID = e.AssessmentToolCategory,
						AssessmentToolImageID = e.ImageId.ToString(),
						AssessmentToolMobImageID = e.MobImageId.ToString(),
						AssessmentToolName = (lang == "en") ? e.NameEn : e.NameAr,
						AssessmentToolWeigth = (Decimal)(e.AssessmentToolCategory == (int)AssessmentToolCategory.Personality ? assessmentPersonalityWeight : (e.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing ? assessmentWellbeingWeight : assessmentEQWeight))
					}).ToList();
			if (batch1.AssessmentMatrix != null)
			{
				foreach (var item in batch1.AssessmentMatrix.MatricesTool)
				{
					assessmentToolCategoryViewList.Add(new AssessmentToolCategoryView()
					{
						AssessmentToolID = item.AssessmentTool.Id,
						AssessmentToolCategoryID = item.AssessmentTool.AssessmentToolCategory,
						AssessmentToolWeigth = (Decimal)item.Weight,
						AssessmentToolImageID = item.AssessmentTool.ImageId.ToString(),
						AssessmentToolMobImageID = item.AssessmentTool.MobImageId.ToString(),
						AssessmentToolName = (lang != "ar") ? item.AssessmentTool.NameEn : item.AssessmentTool.NameAr
					});
				}
			}
			assessmentToolsReport.BatchAssessmentToolsProfiles.Data = new List<BatchAssessmentToolsProfilesPV>();
			var list1 = batch1.Applications.Where(e =>
			{
				int? statusItemId = e.StatusItemId;
				int num1 = 59007;
				if (statusItemId.GetValueOrDefault() == num1 & statusItemId.HasValue)
					return false;
				int? assessmentItemId = e.AssessmentItemId;
				int num2 = 62002;
				return assessmentItemId.GetValueOrDefault() == num2 & assessmentItemId.HasValue;
			}).ToList();

			if (list1.Count <= 0)
				return;

			foreach (Application application1 in list1)
			{
				Application application = application1;
				BatchAssessmentToolsProfilesPV assessmentToolsProfilesPv = new BatchAssessmentToolsProfilesPV();
				foreach (AssessmentToolCategoryView assessmentToolCategoryView1 in assessmentToolCategoryViewList)
				{
					AssessmentToolCategoryView _assCateg = assessmentToolCategoryView1;
					AssessmentToolCategoryView assessmentToolCategoryView2 = new AssessmentToolCategoryView();
					ProfileAssessmentToolScore assessmentToolScore = _appDbContext.ProfileBatchAssessments
						.Include(x => x.ProfileAssessmenttool)
						.Where(e => e.BatchId == BatchID && e.ApplicationId == application.Id && e.ProfileId == application.ProfileId && e.ProfileAssessmenttool.AssessmentToolId == _assCateg.AssessmentToolID)
						.Select(e => e.ProfileAssessmenttool).FirstOrDefault();
					if (assessmentToolScore != null)
						assessmentToolCategoryView2.ProfileWeigthedScore = assessmentToolScore.Score * _assCateg.AssessmentToolWeigth / new Decimal(100);
					if (_assCateg.AssessmentToolCategoryID != 82005)
						assessmentToolsProfilesPv.Profile.OverAllWeigthedScore += assessmentToolCategoryView2.ProfileWeigthedScore;
				}
				application.TotalAssessmentScore = new Decimal?(assessmentToolsProfilesPv.Profile.OverAllWeigthedScore * new Decimal(10));
				application.IsRecordUpdated = true;
			}
			var list2 = list1.OrderByDescending(e => e.TotalAssessmentScore).ToList();
			for (int index = 0; index < list2.Count; ++index)
				list2[index].BatchRank = new int?(index + 1);
			foreach (IGrouping<Decimal?, Application> source in list1.GroupBy(m => m.TotalAssessmentScore))
			{
				if (source.Count<Application>() > 1)
				{
					int? batchRank = source.FirstOrDefault<Application>().BatchRank;
					foreach (Application application in source)
						application.BatchRank = batchRank;
				}
			}
			Batch batch2 = batch1;
			Decimal? nullable1 = list1.Select(a => a.TotalAssessmentScore).Sum();
			Decimal count = (Decimal)list1.Count;
			Decimal? nullable2 = nullable1.HasValue ? new Decimal?(nullable1.GetValueOrDefault() / count) : new Decimal?();
			batch2.AverageAssessmentScore = nullable2;
			batch1.BestAssessmentScore = list1.Select(a => a.TotalAssessmentScore).Max();
			await _appDbContext.SaveChangesAsync();
		}

		private async Task<HashSet<batchPrec>> UpdateOpenedAppsPercentgeByCommonData(int sectionID, int profileID, LanguageType lang)
		{
			try
			{
				var list = _appDbContext.Applications
				.Where(a => a.ProfileId == profileID && a.StatusItemId == (int?)59001 && (!a.Batch.IsClosed && a.Batch.DateRegFrom <= DateTime.Now && a.Batch.DateRegTo >= DateTime.Now)).ToList();
				var batchPrecSet1 = new HashSet<batchPrec>();
				foreach (var app in list)
				{
					if (_appDbContext.Batches.FirstOrDefault(b => b.Id == app.BatchId) != null)
					{
						HashSet<batchPrec> batchPrecSet2 = batchPrecSet1;
						batchPrec batchPrec1 = new batchPrec
						{
							ProgrammeID = _appDbContext.Batches.FirstOrDefault(b => b.Id == app.BatchId).ProgrammeId,
							Percentage = await UpdateSpasifcStatus(app.Id, sectionID, profileID, lang)
						};
						batchPrec batchPrec2 = batchPrec1;
						batchPrecSet2.Add(batchPrec2);
					}
				}
				return batchPrecSet1;
			}
			catch (Exception e)
			{

				throw e;
			}
		}

		private async Task<int> UpdateSpasifcStatus(int applicationID, int sectionID, int ProfileID, LanguageType lang)
		{
			await UpdateProgress(applicationID, ProfileID, lang);
			await _appDbContext.SaveChangesAsync();
			return await GetAppProgressPercentage(applicationID);
		}

		private async Task<int> GetAppProgressPercentage(int applicationID)
		{
			int num = await _appDbContext.Applications.Where(e => e.Id == applicationID).Select(p => p.CompletionPercentage).FirstOrDefaultAsync();
			return num != 0 ? num : 0;
		}

		private bool CheckAssessmentValidity(int AssessmentToolID, int ProfileID, int ValidityRangeNumber)
		{
			var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores
				.Where(p => p.AssessmentToolId == AssessmentToolID && p.ProfileId == ProfileID && p.IsCompleted == true)
				.OrderByDescending(e => e.Order).FirstOrDefault();
			return assessmentToolScore == null || assessmentToolScore.Created.AddMonths(ValidityRangeNumber) <= DateTime.Now;
		}

		private async Task AddProfileBatchAssessments(int ProfileID, int BatchID, int ApplicationID, List<int> ProfileAssessmentIDs)
		{
			foreach (int profileAssessmentId in ProfileAssessmentIDs)
			{
				int profileAssessmentID = profileAssessmentId;
				if (_appDbContext.ProfileBatchAssessments.Where(e => e.ProfileId == ProfileID && e.BatchId == BatchID && e.ApplicationId == ApplicationID && e.ProfileAssessmenttoolId == profileAssessmentID) == null)
					_appDbContext.ProfileBatchAssessments.Add(new Models.ProfileBatchAssessment()
					{
						ProfileId = ProfileID,
						BatchId = BatchID,
						ApplicationId = ApplicationID,
						ProfileAssessmenttoolId = profileAssessmentID
					});
			}
			await _appDbContext.SaveChangesAsync();
		}

		private async Task<ApplicationSectionCountsView> GetApplicationAndProfileSections(int appID)
		{
			var model = _appDbContext.Applications
				.Include(x => x.ApplicationTrainings)
				.Include(x => x.ApplicationAchievements)
				.Include(x => x.QuestionAnswers)
				.Include(x => x.Batch)
				.ThenInclude(x => x.QuestionGroup)
				.ThenInclude(x => x.QuestionGroupsQuestions)
				.Include(x => x.Profile)
				.ThenInclude(x => x.ProfileEducations)
				.Include(x => x.Profile)
				.ThenInclude(x => x.ProfileWorkExperiences)
				.Include(x => x.Profile)
				.ThenInclude(x => x.ProfileMemberships)
				.Include(x => x.Profile)
				.ThenInclude(x => x.ProfileTrainings)
				.Include(x => x.Profile)
				.ThenInclude(x => x.ProfileAchievements)
				.Where(a => a.Id == appID).FirstOrDefault();

			var sectionCountsView = new ApplicationSectionCountsView();
			if (model != null)
			{
				bool flag = true;
				var assessmentTools = _appDbContext.Batches
					.Include(x => x.BatchAssessmentTools)
					.ThenInclude(x => x.AssessmentTool)
					.ThenInclude(x => x.ProfileAssessmentToolScores)
					.FirstOrDefault(x => x.Id == model.BatchId).BatchAssessmentTools;
				if (assessmentTools != null)
				{
					List<int> ProfileAssessmentIDs = new List<int>();
					foreach (var assessmentTool in assessmentTools)
					{
						var assessmentToolScore = assessmentTool.AssessmentTool.ProfileAssessmentToolScores
							.Where(e => e.ProfileId == model.ProfileId)
							.OrderByDescending(o => o.Order).FirstOrDefault();
						if (assessmentToolScore == null || assessmentToolScore.IsCompleted == false || CheckAssessmentValidity(assessmentToolScore.AssessmentTool.Id, model.ProfileId, assessmentToolScore.AssessmentTool.ValidityRangeNumber))
						{
							flag = false;
							ProfileAssessmentIDs = new List<int>();
							break;
						}
						ProfileAssessmentIDs.Add(assessmentToolScore.Id);
					}
					if (ProfileAssessmentIDs.Count > 0)
						await AddProfileBatchAssessments(model.ProfileId, model.BatchId, model.Id, ProfileAssessmentIDs);
				}

				sectionCountsView.EducationCount = model.Profile.ProfileEducations.Count;
				sectionCountsView.WorkExperienceCount = model.Profile.ProfileWorkExperiences.Count;
				sectionCountsView.MembershipCount = model.Profile.ProfileMemberships.Count;
				int? statusItemId = model.StatusItemId;
				int num = 59001;
				if (statusItemId.GetValueOrDefault() == num & statusItemId.HasValue)
				{
					sectionCountsView.TrainingCount = model.Profile.ProfileTrainings.Count;
					sectionCountsView.AchievementCount = model.Profile.ProfileAchievements.Count;
				}
				else
				{
					sectionCountsView.TrainingCount = model.ApplicationTrainings.Count;
					sectionCountsView.AchievementCount = model.ApplicationAchievements.Count;
				}
				sectionCountsView.AttachmentCount = 0;
				sectionCountsView.ProgramDetailsCount = model.QuestionAnswers.Count != model.Batch.QuestionGroup.QuestionGroupsQuestions.Count ? 0 : 1;
				int? nullable = model.Profile.PassportFileId;
				if (nullable.HasValue)
					++sectionCountsView.AttachmentCount;
				nullable = model.Profile.UaeidfileId;
				if (nullable.HasValue)
					++sectionCountsView.AttachmentCount;
				nullable = model.Profile.LastEducationCertificateFileId;
				if (nullable.HasValue)
					++sectionCountsView.AttachmentCount;
				nullable = model.Profile.CvfileId;
				if (nullable.HasValue)
					++sectionCountsView.AttachmentCount;
				nullable = model.Profile.FamilyBookFileId;
				if (nullable.HasValue)
					++sectionCountsView.AttachmentCount;
				int? smallImageFileId = _appDbContext.Users.FirstOrDefault(x => x.Id == model.Profile.Id).SmallImageFileId;
				if (model.Profile.FirstNameAr != null && model.Profile.FirstNameEn != null && (model.Profile.SecondNameAr != null && model.Profile.SecondNameEn != null) && (model.Profile.ThirdNameAr != null && model.Profile.ThirdNameEn != null && (model.Profile.LastNameAr != null && model.Profile.LastNameEn != null)) && (model.Profile.Address != null && model.Profile.PassportNumber != null))
				{
					nullable = model.Profile.PassportIssueEmirateItemId;
					if (nullable.HasValue && model.Profile.BirthDate.HasValue)
					{
						if (_appDbContext.UserInfos.FirstOrDefault(u => u.UserId == model.Profile.Id).Mobile != null && model.Profile.Eid != null)
						{
							nullable = _appDbContext.Users.FirstOrDefault(u => u.Id == model.Profile.Id).SmallImageFileId;
							if (nullable.HasValue)
								sectionCountsView.CandidateInformationCompleted = true;
						}
					}
				}
				if (flag)
					sectionCountsView.AssessmentToolDone = true;
			}
			return sectionCountsView;
		}

		private async Task<List<ApplicationProgressView>> UpdateProgress(int applicationID, int ProfileID, LanguageType lang)
		{
			var list = _appDbContext.ApplicationProgresses.Where(m => m.ApplicationId == applicationID).ToList();
			var andProfileSections = await GetApplicationAndProfileSections(applicationID);
			var profile = _appDbContext.Profiles
					.Include(x => x.Applications)
					.Include(x => x.ProfileTrainings)
					.Include(x => x.ProfileAchievements)
				.FirstOrDefault(x => x.Id == ProfileID);
			var application = _appDbContext.Applications.FirstOrDefault(x => x.Id == applicationID);
			application.CompletionPercentage = 10;
			if (andProfileSections.CandidateInformationCompleted)
			{
				list.Where(p => p.ApplicationSectionItemId == 63004).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63004).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			if (andProfileSections.EducationCount > 0)
			{
				list.Where(p => p.ApplicationSectionItemId == 63001).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63001).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			if (andProfileSections.WorkExperienceCount > 0)
			{
				list.Where(p => p.ApplicationSectionItemId == 63002).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63002).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			if (andProfileSections.MembershipCount > 0)
			{
				list.Where(p => p.ApplicationSectionItemId == 63003).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63003).FirstOrDefault().ApplicationSectionStatusItemId = 64002;
			int? statusItemId;
			if (andProfileSections.TrainingCount > 0)
			{
				list.Where(p => p.ApplicationSectionItemId == 63005).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
			{
				statusItemId = profile.Applications.Where(ap => ap.Id == applicationID).FirstOrDefault().StatusItemId;
				int num = 59001;
				if (statusItemId.GetValueOrDefault() == num & statusItemId.HasValue && profile.ProfileTrainings.Count > 0)
				{
					list.Where(p => p.ApplicationSectionItemId == 63005).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
					application.CompletionPercentage += 10;
				}
				else
					list.Where(p => p.ApplicationSectionItemId == 63005).FirstOrDefault().ApplicationSectionStatusItemId = 64002;
			}
			if (andProfileSections.AchievementCount > 0)
			{
				list.Where(p => p.ApplicationSectionItemId == 63006).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
			{
				statusItemId = profile.Applications.Where(ap => ap.Id == applicationID).FirstOrDefault().StatusItemId;
				int num = 59001;
				if (statusItemId.GetValueOrDefault() == num & statusItemId.HasValue && profile.ProfileAchievements.Count > 0)
				{
					list.Where(p => p.ApplicationSectionItemId == 63006).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
					application.CompletionPercentage += 10;
				}
				else
					list.Where(p => p.ApplicationSectionItemId == 63006).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			}
			if (andProfileSections.ProgramDetailsCount == 1)
			{
				list.Where(p => p.ApplicationSectionItemId == 63007).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63007).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			if (andProfileSections.AttachmentCount < 5)
			{
				list.Where(p => p.ApplicationSectionItemId == 63008).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			}
			else
			{
				list.Where(p => p.ApplicationSectionItemId == 63008).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			if (andProfileSections.AssessmentToolDone)
			{
				list.Where(p => p.ApplicationSectionItemId == 63009).FirstOrDefault().ApplicationSectionStatusItemId = 64001;
				application.CompletionPercentage += 10;
			}
			else
				list.Where(p => p.ApplicationSectionItemId == 63009).FirstOrDefault().ApplicationSectionStatusItemId = 64003;
			await _appDbContext.SaveChangesAsync();
			return GetAppProgress(list, 0, lang);
		}

		private List<ApplicationProgressView> GetAppProgress(List<ApplicationProgress> appProg, int applicationID, LanguageType lang)
		{
			if (appProg == null && applicationID == 0)
				return new List<ApplicationProgressView>();
			if (applicationID != 0)
				appProg = _appDbContext.ApplicationProgresses.Where(m => m.ApplicationId == applicationID).ToList();
			return appProg.Select(p => new ApplicationProgressView()
			{
				ApplicationSection = p.ApplicationSectionItem.Id == 63008 ? "Documents" : (lang == LanguageType.EN) ? p.ApplicationSectionStatusItem.NameEn : p.ApplicationSectionItem.NameAr,
				ApplicationSectionStatus = (lang == LanguageType.EN) ? p.ApplicationSectionStatusItem.NameEn : p.ApplicationSectionStatusItem.NameAr,
				ApplicationSectionItemID = p.ApplicationSectionItemId,
				ApplicationSectionStatusItemID = p.ApplicationSectionStatusItemId
			}).ToList();
		}

		private async Task UpdateProfileScoring(ProfileScore? profileScore, int profileID)
		{
			try
			{
				var user = _appDbContext.Users.FirstOrDefault(x => x.Id == profileID);
				var profile = _appDbContext.Profiles.Include(x => x.ScoringProfile).FirstOrDefault(x => x.Id == profileID);
				var data = _appDbContext.Users.Where(e => e.Id == profileID).Select(e => new
				{
					e.Username,
					ID = e.Id
				}).FirstOrDefault();
				if (profile.ScoringProfile == null)
				{
					profile.ScoringProfile = new Models.ScoringProfile
					{
						Created = DateTime.Now,
						CreatedBy = user.Username,
						Modified = DateTime.Now,
						ModifiedBy = user.Username
					};
				}
				else
				{
					profile = this.ResetScores(profileScore, profile);
					profile.ScoringProfile.Modified = DateTime.Now;
					profile.ScoringProfile.ModifiedBy = user.Username;
				}
				ProfileScore? nullable;
				if (profileScore.HasValue)
				{
					nullable = profileScore;
					ProfileScore profileScore1 = ProfileScore.Education;
					if (nullable.GetValueOrDefault() == profileScore1 & nullable.HasValue)
					{
						UpdateEducationScoring(profile);
					}
					else
					{
						nullable = profileScore;
						ProfileScore profileScore2 = ProfileScore.WorkExperince;
						if (nullable.GetValueOrDefault() == profileScore2 & nullable.HasValue)
						{
							UpdateWorkExperinceScoring(profile);
						}
						else
						{
							nullable = profileScore;
							ProfileScore profileScore3 = ProfileScore.Assessment;
							if (nullable.GetValueOrDefault() == profileScore3 & nullable.HasValue)
								UpdateAssessmentScoring(profile);
						}
					}
				}
				else
				{
					UpdateEducationScoring(profile);
					this.UpdateWorkExperinceScoring(profile);
					this.UpdateAssessmentScoring(profile);
				}
				nullable = profileScore;
				ProfileScore profileScore4 = ProfileScore.Education;
				if (!(nullable.GetValueOrDefault() == profileScore4 & nullable.HasValue))
				{
					nullable = profileScore;
					ProfileScore profileScore1 = ProfileScore.WorkExperince;
					if (!(nullable.GetValueOrDefault() == profileScore1 & nullable.HasValue))
					{
						await _appDbContext.SaveChangesAsync();
						await this.UpdateAllApplicationScoring(profileID);
						return;
					}
				}
				_appDbContext.Profiles.FirstOrDefault(x => x.Id == data.ID).ProfileLastModified = DateTime.Now;
				await _appDbContext.SaveChangesAsync();
				await this.UpdateAllApplicationScoring(profileID);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private async Task UpdateAllApplicationScoring(int profileID)
		{
			foreach (Application application in _appDbContext.Profiles
				.Include(x => x.Applications)
				.ThenInclude(x => x.ScoringApplication)
				.ThenInclude(x => x.Profile)
				.ThenInclude(x => x.ScoringProfile).FirstOrDefault(x => x.Id == profileID).Applications)
			{
				if (application.ScoringApplication != null && application.Profile.ScoringProfile != null)
					application.TotalScore = application.ScoringApplication.ApplcationTotalScore.Value + application.Profile.ScoringProfile.ProfileTotalScore.Value;
				else if (application.Profile.ScoringProfile != null)
					application.TotalScore = application.Profile.ScoringProfile.ProfileTotalScore.Value;
			}
			await _appDbContext.SaveChangesAsync();
		}

		private void UpdateEducationScoring(Models.Profile profile)
		{
			var profileEducations = profile.ProfileEducations;
			if (profileEducations.Any(ed => ed.DegreeItemId == 56004 && ed.Finshed))
			{
				profile.ScoringProfile.EducationPhdholderScore = educationPHDHolderScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56004 && ed.Finshed && ed.Organization != null && ed.Organization.IsFromTop200Qsrank))
					return;
				profile.ScoringProfile.EducationPhdholderFromTop200QsrankScore = educationPHDHolderFromTop200QSRankScore;
			}
			else if (profileEducations.Any(ed => ed.DegreeItemId == 56004 && !ed.Finshed))
			{
				profile.ScoringProfile.EducationStudyingPhdscore = educationStudyingPHDScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56003 && ed.Finshed))
					return;
				profile.ScoringProfile.EducationMasterHolderScore = educationMasterHolderScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56003 && ed.Finshed && ed.Organization != null && ed.Organization.IsFromTop200Qsrank))
					return;
				profile.ScoringProfile.EducationMasterHolderFromTop200QsrankScore = educationMasterHolderFromTop200QSRankScore;
			}
			else if (profileEducations.Any(ed => ed.DegreeItemId == 56003 && ed.Finshed))
			{
				profile.ScoringProfile.EducationMasterHolderScore = educationMasterHolderScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56003 && ed.Finshed && ed.Organization != null && ed.Organization.IsFromTop200Qsrank))
					return;
				profile.ScoringProfile.EducationMasterHolderFromTop200QsrankScore = 5;
			}
			else if (profileEducations.Any(ed => ed.DegreeItemId == 56003 && !ed.Finshed))
			{
				profile.ScoringProfile.EducationStudyingMasterScore = educationStudyingMasterScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56002 && ed.Finshed))
					return;
				profile.ScoringProfile.EducationBachelorScore = educationBachelorScore;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56002 && ed.Finshed && ed.Organization != null && ed.Organization.IsFromTop200Qsrank))
					return;
				profile.ScoringProfile.EducationBachelorFromTop200QsrankScore = educationBachelorFromTop200QSRankScore;
			}
			else if (profileEducations.Any(ed => ed.DegreeItemId == 56002))
			{
				profile.ScoringProfile.EducationBachelorScore = 3;
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56002 && ed.Organization != null && ed.Organization.IsFromTop200Qsrank))
					return;
				profile.ScoringProfile.EducationBachelorFromTop200QsrankScore = educationBachelorFromTop200QSRankScore;
			}
			else if (profileEducations.Any(ed => ed.DegreeItemId == 56001))
			{
				profile.ScoringProfile.EducationHigherDiplomaScore = educationHigherDiplomaScore;
			}
			else
			{
				if (!profileEducations.Any(ed => ed.DegreeItemId == 56007))
					return;
				profile.ScoringProfile.EducationDiplomaScore = educationDiplomaScore;
			}
		}

		private void UpdateWorkExperinceScoring(Models.Profile profile)
		{
			profile.TotalYearsOfExperinceWriteOnly = new Decimal?(CalculateTotalNumberOfExpe(profile, (List<int>)null));
			if (profile.ProfileWorkExperiences.Any(w =>
			{
				if (w.Organization == null)
					return false;
				int? organizationScaleItemId = w.Organization.OrganizationScaleItemId;
				int num = 54002;
				return organizationScaleItemId.GetValueOrDefault() == num & organizationScaleItemId.HasValue;
			}))
				profile.ScoringProfile.ExperienceInMulinationalOrganizationScore = (Decimal)this.experienceInMulinationalOrganizationScore;
			if (profile.ProfileWorkExperiences.Any(w =>
			{
				if (w.Organization == null)
					return false;
				int? sectorTypeItemId = w.Organization.OrganizationSectorTypeItemId;
				int num = 53001;
				return sectorTypeItemId.GetValueOrDefault() == num & sectorTypeItemId.HasValue;
			}) && profile.ProfileWorkExperiences.Any(w =>
			{
				int? sectorTypeItemId = w.Organization.OrganizationSectorTypeItemId;
				int num = 53002;
				return sectorTypeItemId.GetValueOrDefault() == num & sectorTypeItemId.HasValue;
			}))
				profile.ScoringProfile.ExperienceInBothPublicAndPrivateScore = (Decimal)this.experienceInBothPublicAndPrivateScore;
			if (!profile.ProfileWorkExperiences.Any(w => w.Organization != null && w.Organization.IsUaepriority))
				return;
			profile.ScoringProfile.ExperienceInUaeprioritySectorScore = (Decimal)this.experienceInUAEPrioritySectorScore;
		}

		private void UpdateAssessmentScoring(Models.Profile profile)
		{
			var source1 = profile.ProfileAssessmentToolScores.Where(a => a.IsCompleted == true && a.AssessmentTool?.AssessmentToolCategory == (int)AssessmentToolCategory.Personality);
			var source2 = profile.ProfileAssessmentToolScores.Where(a => a.IsCompleted == true && a.AssessmentTool?.AssessmentToolCategory == (int)AssessmentToolCategory.EQ);
			var source3 = profile.ProfileAssessmentToolScores.Where(a => a.IsCompleted == true && a.AssessmentTool?.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing);
			if (source1 != null)
				profile.ScoringProfile.AssessmentPersonalityScore = source1.OrderByDescending(a => a.Order).FirstOrDefault() != null ? source1.OrderByDescending(a => a.Order).FirstOrDefault().Score * new Decimal(20) / new Decimal(100) * new Decimal(10) : Decimal.Zero;
			if (source2 != null)
				profile.ScoringProfile.AssessmentEmotionalIntelligenceScore = source2.OrderByDescending(a => a.Order).FirstOrDefault() != null ? source2.OrderByDescending(a => a.Order).FirstOrDefault().Score * new Decimal(15) / new Decimal(100) * new Decimal(10) : Decimal.Zero;
			if (source3 == null)
				return;
			profile.ScoringProfile.AssessmentWelbeingScore = source3.OrderByDescending(a => a.Order).FirstOrDefault() != null ? source3.OrderByDescending(a => a.Order).FirstOrDefault().Score * new Decimal(5) / new Decimal(100) * new Decimal(10) : Decimal.Zero;
		}

		private Models.Profile ResetScores(ProfileScore? profileScore, Models.Profile profile)
		{
			if (profileScore.HasValue)
			{
				ProfileScore? nullable = profileScore;
				if (nullable.HasValue)
				{
					switch (nullable.GetValueOrDefault())
					{
						case ProfileScore.Education:
							profile.ScoringProfile.EducationPhdholderScore = Decimal.Zero;
							profile.ScoringProfile.EducationPhdholderFromTop200QsrankScore = Decimal.Zero;
							profile.ScoringProfile.EducationStudyingPhdscore = Decimal.Zero;
							profile.ScoringProfile.EducationMasterHolderScore = Decimal.Zero;
							profile.ScoringProfile.EducationMasterHolderFromTop200QsrankScore = Decimal.Zero;
							profile.ScoringProfile.EducationStudyingMasterScore = Decimal.Zero;
							profile.ScoringProfile.EducationBachelorScore = Decimal.Zero;
							profile.ScoringProfile.EducationBachelorFromTop200QsrankScore = Decimal.Zero;
							profile.ScoringProfile.EducationHigherDiplomaScore = Decimal.Zero;
							profile.ScoringProfile.EducationDiplomaScore = Decimal.Zero;
							return profile;
						case ProfileScore.WorkExperince:
							profile.ScoringProfile.ExperienceInMulinationalOrganizationScore = Decimal.Zero;
							profile.ScoringProfile.ExperienceInBothPublicAndPrivateScore = Decimal.Zero;
							profile.ScoringProfile.ExperienceInUaeprioritySectorScore = Decimal.Zero;
							return profile;
						case ProfileScore.Assessment:
							profile.ScoringProfile.AssessmentPersonalityScore = Decimal.Zero;
							profile.ScoringProfile.AssessmentEmotionalIntelligenceScore = Decimal.Zero;
							profile.ScoringProfile.AssessmentWelbeingScore = Decimal.Zero;
							return profile;
					}
				}
				return profile;
			}
			profile.ScoringProfile.EducationPhdholderScore = Decimal.Zero;
			profile.ScoringProfile.EducationPhdholderFromTop200QsrankScore = Decimal.Zero;
			profile.ScoringProfile.EducationStudyingPhdscore = Decimal.Zero;
			profile.ScoringProfile.EducationMasterHolderScore = Decimal.Zero;
			profile.ScoringProfile.EducationMasterHolderFromTop200QsrankScore = Decimal.Zero;
			profile.ScoringProfile.EducationStudyingMasterScore = Decimal.Zero;
			profile.ScoringProfile.EducationBachelorScore = Decimal.Zero;
			profile.ScoringProfile.EducationBachelorFromTop200QsrankScore = Decimal.Zero;
			profile.ScoringProfile.EducationHigherDiplomaScore = Decimal.Zero;
			profile.ScoringProfile.EducationDiplomaScore = Decimal.Zero;
			profile.ScoringProfile.ExperienceInMulinationalOrganizationScore = Decimal.Zero;
			profile.ScoringProfile.ExperienceInBothPublicAndPrivateScore = Decimal.Zero;
			profile.ScoringProfile.ExperienceInUaeprioritySectorScore = Decimal.Zero;
			profile.ScoringProfile.AssessmentPersonalityScore = Decimal.Zero;
			profile.ScoringProfile.AssessmentEmotionalIntelligenceScore = Decimal.Zero;
			profile.ScoringProfile.AssessmentWelbeingScore = Decimal.Zero;
			return profile;
		}

		private Decimal CalculateTotalNumberOfExpe(Models.Profile profile, List<int> filters = null)
		{
			var array = profile.ProfileWorkExperiences.OrderBy(m => m.DateFrom).ToList();
			if (filters != null)
				array = profile.ProfileWorkExperiences.Where(ex => ex.Organization.OrganizationSectorTypeItemId.HasValue && filters.Any(f => f == ex.Organization.OrganizationSectorTypeItemId.Value)).OrderBy(m => m.DateFrom).ToList();
			Decimal num1 = new Decimal();
			DateTime? dateTo1;
			TimeSpan? nullable1;
			TimeSpan timeSpan;
			for (int index = 0; index < ((IEnumerable<ProfileWorkExperience>)array).Count<ProfileWorkExperience>(); ++index)
			{
				if (index == 0)
				{
					dateTo1 = array[index].DateTo;
					if (dateTo1.HasValue)
					{
						Decimal num2 = num1;
						dateTo1 = array[index].DateTo;
						DateTime dateFrom = array[index].DateFrom;
						TimeSpan? nullable2;
						if (!dateTo1.HasValue)
						{
							nullable1 = new TimeSpan?();
							nullable2 = nullable1;
						}
						else
							nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateFrom);
						nullable1 = nullable2;
						timeSpan = nullable1.Value;
						Decimal days = (Decimal)timeSpan.Days;
						num1 = num2 + days;
					}
					else
					{
						Decimal num2 = num1;
						timeSpan = DateTime.Now.Subtract(array[index].DateFrom);
						Decimal days = (Decimal)timeSpan.Days;
						num1 = num2 + days;
					}
				}
				else
				{
					dateTo1 = array[index - 1].DateTo;
					if (!dateTo1.HasValue)
					{
						dateTo1 = array[index].DateTo;
						if (!dateTo1.HasValue)
						{
							dateTo1 = array[index - 1].DateTo;
							DateTime dateFrom = array[index].DateFrom;
							if ((dateTo1.HasValue ? (dateTo1.GetValueOrDefault() <= dateFrom ? 1 : 0) : 0) != 0)
								continue;
						}
					}
					DateTime dateFrom1 = array[index].DateFrom;
					dateTo1 = array[index - 1].DateTo;
					if ((dateTo1.HasValue ? (dateFrom1 >= dateTo1.GetValueOrDefault() ? 1 : 0) : 0) != 0)
					{
						dateTo1 = array[index].DateTo;
						if (dateTo1.HasValue)
						{
							Decimal num2 = num1;
							dateTo1 = array[index].DateTo;
							DateTime dateFrom2 = array[index].DateFrom;
							TimeSpan? nullable2;
							if (!dateTo1.HasValue)
							{
								nullable1 = new TimeSpan?();
								nullable2 = nullable1;
							}
							else
								nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateFrom2);
							nullable1 = nullable2;
							timeSpan = nullable1.Value;
							Decimal days = (Decimal)timeSpan.Days;
							num1 = num2 + days;
						}
						else
						{
							dateTo1 = array[index].DateTo;
							if (!dateTo1.HasValue)
							{
								Decimal num2 = num1;
								timeSpan = DateTime.Now.Subtract(array[index].DateFrom);
								Decimal days = (Decimal)timeSpan.Days;
								num1 = num2 + days;
							}
						}
					}
					else
					{
						dateTo1 = array[index - 1].DateTo;
						if (dateTo1.HasValue)
						{
							dateTo1 = array[index].DateTo;
							if (!dateTo1.HasValue)
							{
								Decimal num2 = num1;
								DateTime now = DateTime.Now;
								ref DateTime local = ref now;
								dateTo1 = array[index - 1].DateTo;
								DateTime dateTime = dateTo1.Value;
								timeSpan = local.Subtract(dateTime);
								Decimal days = (Decimal)timeSpan.Days;
								num1 = num2 + days;
								continue;
							}
						}
						dateTo1 = array[index - 1].DateTo;
						if (dateTo1.HasValue)
						{
							dateTo1 = array[index].DateTo;
							if (dateTo1.HasValue)
							{
								Decimal num2 = num1;
								dateTo1 = array[index].DateTo;
								DateTime? dateTo2 = array[index - 1].DateTo;
								TimeSpan? nullable2;
								if (!(dateTo1.HasValue & dateTo2.HasValue))
								{
									nullable1 = new TimeSpan?();
									nullable2 = nullable1;
								}
								else
									nullable2 = new TimeSpan?(dateTo1.GetValueOrDefault() - dateTo2.GetValueOrDefault());
								nullable1 = nullable2;
								timeSpan = nullable1.Value;
								Decimal days = (Decimal)timeSpan.Days;
								num1 = num2 + days;
							}
						}
					}
				}
			}
			return Math.Round(Decimal.Parse(Math.Truncate(num1 / new Decimal(365)).ToString() + "." + (object)Math.Truncate(num1 % new Decimal(365) / new Decimal(30))), 1);
		}

		private async Task ExecuteCalcalution(int AssessmentToolID, UserInfo user, bool HasQuestDirect, int Order, LanguageType lang)
		{
			try
			{
				var profileId = user.UserId;
				var subscaleAndCategory = GetHasSubscaleAndCategory(AssessmentToolID);
				if (!HasQuestDirect)
				{
					if (subscaleAndCategory.HasSubScale)
					{
						await AssignSubScaleScore(AssessmentToolID, profileId, Order, true);
						if (subscaleAndCategory.AssessmentToolCategID == (int)AssessmentToolCategory.Personality)
							await AssignCompetnciesScore(AssessmentToolID, profileId, Order, true);
					}
					await AssignScaleScore(AssessmentToolID, profileId, Order, true, subscaleAndCategory.HasSubScale);
					await AssignFactorScore(AssessmentToolID, profileId, Order, true);
				}
				if (subscaleAndCategory.AssessmentToolCategID == (int)AssessmentToolCategory.Leadership)
					await AssignLeaderShipCompetencies(AssessmentToolID, profileId, Order);
				await AssignAssessmentToolScore(AssessmentToolID, user, HasQuestDirect, false, Order, true);
				await UpdateProfileCompletionAfterSubmission(profileId, AssessmentToolID, lang);
			}
			catch (Exception e)
			{

				throw e;
			}
		}

		private async Task AssignAssessmentToolScore(int AssessmentToolID, UserInfo user, bool HasQuestionDirect, bool HasQuestionHead, int Order, bool IsCompleted)
		{
			var profileId = user.UserId;
			var assessmenttool = _appDbContext.AssessmentTools
				.Include(x => x.Factors)
				.Include(x => x.QuestionItems)
				.Include(x => x.ProfileQuestionItemScores)
				.Include(x => x.BatchAssessmentTools)
				.Select(e => new
				{
					ID = e.Id,
					AssessmentToolCategory = e.AssessmentToolCategory,
					Factors = e.Factors,
					Questions = e.QuestionItems,
					Mean = e.Mean,
					StandardDeviation = e.StandardDeviation,
					Batches = e.BatchAssessmentTools
				}).Where(e => e.ID == AssessmentToolID).FirstOrDefault();

			if (assessmenttool == null) throw new ArgumentException($"Assignment does not exist with the ID {AssessmentToolID}");

			Decimal num1 = new Decimal();
			Decimal num2 = new Decimal();
			Decimal num3 = new Decimal();
			Decimal num4 = new Decimal();
			Decimal num5 = new Decimal();
			Decimal num6 = new Decimal();
			Decimal num7 = new Decimal();
			Decimal num8 = new Decimal();
			ProfileAssessmentToolScore assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.FirstOrDefault(e => e.ProfileId == profileId && e.AssessmentToolId == assessmenttool.ID && e.Order == Order);
			if (IsCompleted)
			{
				if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.Personality)
				{
					List<ProfileCompetencyScore> list = _appDbContext.ProfileCompetencyScores.Where(e => e.AssessmenttoolId == AssessmentToolID && e.Order == Order && e.ProfileId == profileId).ToList();
					int count = list.Count;
					Decimal num9 = list.Sum(e => e.Stenscore.GetValueOrDefault());
					if (count > 0)
					{

						Decimal num10 = num9 / (Decimal)count;
						num6 = num9 / (Decimal)count;
					}
					Decimal num11 = new Decimal();
					if (assessmenttool.Factors.Count > 0)
					{
						foreach (Factor factor in assessmenttool.Factors)
						{
							if (factor.ProfileFactorScores.Count > 0)
								num11 += factor.ProfileFactorScores.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order).Score;
						}

					}
					num2 = num11;
					if (assessmenttool.Mean.HasValue)
					{
						Decimal? nullable = assessmenttool.StandardDeviation;
						if (nullable.HasValue)
						{
							nullable = assessmenttool.Mean;
							Decimal valueOrDefault1 = nullable.GetValueOrDefault();
							nullable = assessmenttool.StandardDeviation;
							Decimal valueOrDefault2 = nullable.GetValueOrDefault();
							Decimal num12 = (num11 - valueOrDefault1) / valueOrDefault2;
							Decimal num13 = num12 * new Decimal(10) + new Decimal(50);
							Decimal d = num12 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
							num4 = num12;
							num3 = d;
							num5 = num13;
							num1 = !(d < Decimal.One) ? Math.Floor(d) : Decimal.One;
							goto label_67;
						}
					}
					if (count > 0)
						num1 = Math.Floor(num9 / (Decimal)count);
				}
				else if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.Wellbeing)
				{
					Decimal? nullable1 = new Decimal?(new Decimal());
					Decimal? score1 = _appDbContext.ProfileScaleScores
						.Include(x => x.Scale)
						.ThenInclude(x => x.Factor)
						.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order && e.Scale.Factor.AssessmentToolId == AssessmentToolID && e.ScaleId == WellBeingHappinessPositiveID)?.Score;
					Decimal? score2 = _appDbContext.ProfileScaleScores
						.Include(x => x.Scale)
						.ThenInclude(x => x.Factor)
						.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order && e.Scale.Factor.AssessmentToolId == AssessmentToolID && e.ScaleId == WellBeingHappinessNegativeID)?.Score;
					Decimal? nullable2 = score1;
					Decimal? nullable3 = score2;
					Decimal? nullable4 = nullable2.HasValue & nullable3.HasValue ? new Decimal?(nullable2.GetValueOrDefault() - nullable3.GetValueOrDefault()) : new Decimal?();
					Decimal? nullable5 = nullable4;
					Decimal affectiveBalanceMean = (Decimal)AffectiveBalanceMean;
					Decimal? nullable6 = nullable5.HasValue ? new Decimal?(nullable5.GetValueOrDefault() - affectiveBalanceMean) : new Decimal?();
					Decimal standardDeviation = (Decimal)AffectiveBalanceStandardDeviation;
					Decimal? nullable7;
					Decimal? nullable8 = nullable7 = nullable6.HasValue ? new Decimal?(nullable6.GetValueOrDefault() / standardDeviation) : new Decimal?();
					Decimal num9 = (Decimal)10;
					Decimal? nullable9 = nullable8.HasValue ? new Decimal?(nullable8.GetValueOrDefault() * num9) : new Decimal?();
					Decimal num10 = (Decimal)50;
					if (nullable9.HasValue)
					{
						Decimal num11 = nullable9.GetValueOrDefault() + num10;
					}
					nullable9 = nullable7;
					Decimal num12 = (Decimal)2;
					if (nullable9.HasValue)
					{
						Decimal num13 = nullable9.GetValueOrDefault() * num12 + new Decimal(55, 0, 0, false, (byte)1);
					}
					var dat = _appDbContext.ProfileScaleScores
						.Include(x => x.Scale)
						.ThenInclude(x => x.Factor)
						.Where(e => e.ProfileId == profileId && e.Order == Order && e.Scale.Factor.AssessmentToolId == AssessmentToolID && e.ScaleId != WellBeingHappinessPositiveID && e.ScaleId != WellBeingHappinessNegativeID)
						.ToList();
					var list = dat.GroupBy(e => e.ScaleId).ToList();
					int count = list.Count;
					Decimal? nullable10 = new Decimal?(new Decimal());
					Decimal num14 = new Decimal();
					foreach (IGrouping<int, ProfileScaleScore> source in list)
					{
						nullable9 = nullable10;
						Decimal? nullable11 = source.Sum(e => e.Stenscore);
						nullable10 = nullable9.HasValue & nullable11.HasValue ? new Decimal?(nullable9.GetValueOrDefault() + nullable11.GetValueOrDefault()) : new Decimal?();
						num14 += source.Sum<ProfileScaleScore>((Func<ProfileScaleScore, Decimal>)(e => e.Score));
					}
					Decimal num15;
					num2 = num15 = num14 + nullable4.GetValueOrDefault();
					Decimal? nullable12 = assessmenttool.Mean;
					Decimal valueOrDefault1 = nullable12.GetValueOrDefault();
					nullable12 = assessmenttool.StandardDeviation;
					Decimal valueOrDefault2 = nullable12.GetValueOrDefault();
					Decimal num16 = valueOrDefault1;
					Decimal num17 = (num15 - num16) / valueOrDefault2;
					Decimal num18 = num17 * new Decimal(10) + new Decimal(50);
					Decimal d = num17 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num4 = num17;
					num3 = d;
					num5 = num18;
					num1 = Math.Floor(d);
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
				else if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.EQ)
				{
					Decimal num9 = new Decimal();
					foreach (Factor factor in assessmenttool.Factors)
						num9 += factor.ProfileFactorScores.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order).Score;
					num2 = num9;
					Decimal valueOrDefault1 = assessmenttool.Mean.GetValueOrDefault();
					Decimal valueOrDefault2 = assessmenttool.StandardDeviation.GetValueOrDefault();
					Decimal num10 = (num9 - valueOrDefault1) / valueOrDefault2;
					Decimal num11 = num10 * new Decimal(10) + new Decimal(50);
					Decimal d = num10 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num4 = num10;
					num3 = d;
					num5 = num11;
					num1 = Math.Floor(d);
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
				else if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.Cognitive)
				{
					Decimal num9 = new Decimal();
					var profilequestDal = _appDbContext.ProfileQuestionItemScores;
					foreach (ProfileQuestionItemScore questionItemScore in profilequestDal
						.Where(e => e.AssessmentToolId == AssessmentToolID && e.ProfileId == profileId && e.Order == Order).ToList())
						num9 += questionItemScore.Score;
					Decimal? nullable = assessmenttool.Mean;
					Decimal valueOrDefault1 = nullable.GetValueOrDefault();
					nullable = assessmenttool.StandardDeviation;
					Decimal valueOrDefault2 = nullable.GetValueOrDefault();
					Decimal num10 = (num9 - valueOrDefault1) / valueOrDefault2;
					Decimal num11 = num10 * new Decimal(10) + new Decimal(50);
					Decimal d = num10 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num1 = Math.Floor(d);
					num2 = num9;
					num4 = num10;
					num3 = d;
					num5 = num11;
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
				else if (assessmenttool.AssessmentToolCategory == 82005)
				{
					Decimal num9 = new Decimal();
					foreach (Factor factor in assessmenttool.Factors)
						num9 += factor.ProfileFactorScores.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order).Score;
					num2 = num9;
					Decimal valueOrDefault1 = assessmenttool.Mean.GetValueOrDefault();
					Decimal valueOrDefault2 = assessmenttool.StandardDeviation.GetValueOrDefault();
					Decimal num10 = (num9 - valueOrDefault1) / valueOrDefault2;
					Decimal num11 = num10 * new Decimal(10) + new Decimal(50);
					Decimal d = num10 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num4 = num10;
					num3 = d;
					num5 = num11;
					num1 = Math.Floor(d);
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
				else if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.EnglishLanguage)
				{
					Decimal num9 = new Decimal();
					foreach (QuestionItem question in (IEnumerable<QuestionItem>)assessmenttool.Questions)
					{
						num9 += _appDbContext.ProfileQuestionItemScores.FirstOrDefault(e => e.QuestionItemId == question.Id 
								&& e.ProfileId == profileId && e.Order == Order).Score;
					}
						
					int num10 = (int)(num9 / (Decimal)assessmenttool.Questions.Count * new Decimal(100));
					Decimal? nullable = assessmenttool.Mean;
					Decimal valueOrDefault1 = nullable.GetValueOrDefault();
					nullable = assessmenttool.StandardDeviation;
					Decimal valueOrDefault2 = nullable.GetValueOrDefault();
					Decimal num11 = (num9 - valueOrDefault1) / valueOrDefault2;
					Decimal num12 = num11 * new Decimal(10) + new Decimal(50);
					Decimal d = num11 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num4 = num11;
					num3 = d;
					num5 = num12;
					num1 = Math.Floor(d);
					num2 = num9;
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
				else if (assessmenttool.AssessmentToolCategory == (int)AssessmentToolCategory.Leadership)
				{
					Decimal num9 = new Decimal();
					foreach (QuestionItem question in assessmenttool.Questions)
						num9 += question.ProfileQuestionItemScores.FirstOrDefault(e => e.ProfileId == profileId && e.Order == Order).Score;
					Decimal? nullable = assessmenttool.Mean;
					Decimal valueOrDefault1 = nullable.GetValueOrDefault();
					nullable = assessmenttool.StandardDeviation;
					Decimal valueOrDefault2 = nullable.GetValueOrDefault();
					Decimal num10 = (num9 - valueOrDefault1) / valueOrDefault2;
					Decimal num11 = num10 * new Decimal(10) + new Decimal(50);
					Decimal d = num10 * new Decimal(2) + new Decimal(55, 0, 0, false, (byte)1);
					num1 = Math.Floor(d);
					num4 = num10;
					num3 = d;
					num5 = num11;
					num2 = num9;
					num7 = valueOrDefault1;
					num8 = valueOrDefault2;
				}
			label_67:
				if (num1 < Decimal.One)
					num1 = Decimal.One;
				else if (num1 > new Decimal(10))
					num1 = new Decimal(10);
				if (assessmentToolScore == null)
				{
					_appDbContext.ProfileAssessmentToolScores.Add(new ProfileAssessmentToolScore
					{
						ProfileId = profileId,
						AssessmentToolId = assessmenttool.ID,
						Score = num1,
						Order = Order,
						IsCompleted = true,
						CompetencyTotalScore = new Decimal?(num6),
						Stenscore = new Decimal?(num3),
						Tscore = new Decimal?(num5),
						RawScore = new Decimal?(num2),
						StandardScore = new Decimal?(num4),
						Mean = new Decimal?(num7),
						StandardDeviation = new Decimal?(num8),
						IsProcessing = false,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = user.Email,
						ModifiedBy = user.Email
					});
					await _appDbContext.SaveChangesAsync();
				}
				else
				{
					assessmentToolScore.IsCompleted = true;
					assessmentToolScore.Score = num1;
					assessmentToolScore.CompetencyTotalScore = new Decimal?(num6);
					assessmentToolScore.Stenscore = new Decimal?(num3);
					assessmentToolScore.Tscore = new Decimal?(num5);
					assessmentToolScore.RawScore = new Decimal?(num2);
					assessmentToolScore.StandardScore = new Decimal?(num4);
					assessmentToolScore.Mean = new Decimal?(num7);
					assessmentToolScore.StandardDeviation = new Decimal?(num8);
					assessmentToolScore.IsProcessing = false;
					assessmentToolScore.Modified = DateTime.Now;
					assessmentToolScore.ModifiedBy = user.Email;
					await _appDbContext.SaveChangesAsync(); // no track
				}
			}
			else
			{
				if (assessmentToolScore == null)
					_appDbContext.ProfileAssessmentToolScores.Add(new ProfileAssessmentToolScore
					{
						ProfileId = profileId,
						AssessmentToolId = assessmenttool.ID,
						Score = num1,
						Order = Order,
						IsCompleted = false,
						Created = DateTime.UtcNow,
						Modified = DateTime.UtcNow,
						CreatedBy = user.Email,
						ModifiedBy = user.Email
					});
				await _appDbContext.SaveChangesAsync();
			}
		}

		private async Task AssignLeaderShipCompetencies(int AssessmenttoolID, int ProfileID, int Order)
		{
			List<ProfileQuestionItemScore> list = _appDbContext.ProfileQuestionItemScores
				.Where(e => e.AssessmentToolId == AssessmenttoolID && e.ProfileId == ProfileID && e.Order == Order)
				.Include(e => e.QuestionItem)
				.ThenInclude(e => e.Competency)
				.ToList();
			foreach (ProfileQuestionItemScore questionItemScore in list)
			{
				ProfileQuestionItemScore profQuestion = questionItemScore;
				if (_appDbContext.ProfileCompetencyScores.FirstOrDefault(e => e.CompetencyId == profQuestion.QuestionItem.CompetencyId.Value && e.AssessmenttoolId == AssessmenttoolID && e.Order == Order && e.ProfileId == ProfileID) == null)
					_appDbContext.ProfileCompetencyScores.Add(new ProfileCompetencyScore()
					{
						CompetencyId = profQuestion.QuestionItem.CompetencyId.Value,
						AssessmenttoolId = AssessmenttoolID,
						Order = Order,
						ProfileId = ProfileID,
						Score = profQuestion.Score,
						IsCompleted = new bool?(true),
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = "APP",
						ModifiedBy = "APP"
					});
			}
			foreach (var grouping in list.GroupBy(e => e.QuestionItem.Competency.PillarId))
			{
				var pillarGrp = grouping;
				PillarAssessmentTool pillarAssessmentTool = _appDbContext.PillarAssessmentTools.FirstOrDefault(e => e.AssessmentToolId == AssessmenttoolID && e.PillarId == pillarGrp.Key);
				Decimal num1 = pillarGrp.Sum(e => e.Score);
				Decimal num2 = num1;
				Decimal? nullable1 = (Decimal?)pillarAssessmentTool?.Mean;
				Decimal? nullable2 = nullable1.HasValue ? new Decimal?(num2 - nullable1.GetValueOrDefault()) : new Decimal?();
				Decimal? nullable3;
				if (pillarAssessmentTool == null)
				{
					nullable1 = new Decimal?();
					nullable3 = nullable1;
				}
				else
					nullable3 = pillarAssessmentTool.StandardDeviation;
				Decimal? nullable4 = nullable3;
				Decimal? nullable5;
				if (!(nullable2.HasValue & nullable4.HasValue))
				{
					nullable1 = new Decimal?();
					nullable5 = nullable1;
				}
				else
					nullable5 = new Decimal?(nullable2.GetValueOrDefault() / nullable4.GetValueOrDefault());
				Decimal? nullable6 = nullable5;
				if (_appDbContext.ProfilePillarScores.FirstOrDefault(e => e.PillarId == pillarGrp.Key && e.AssessmenttoolId == AssessmenttoolID && e.Order == Order && e.ProfileId == ProfileID) == null)
				{
					var profilePillarScoresDal = _appDbContext.ProfilePillarScores;
					ProfilePillarScore entity = new ProfilePillarScore
					{
						PillarId = pillarGrp.Key,
						AssessmenttoolId = AssessmenttoolID,
						Order = Order,
						ProfileId = ProfileID,
						Score = num1,
						StandardScore = nullable6,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = "APP",
						ModifiedBy = "APP"
					};
					nullable2 = nullable6;
					Decimal num3 = (Decimal)10;
					Decimal? nullable7;
					if (!nullable2.HasValue)
					{
						nullable1 = new Decimal?();
						nullable7 = nullable1;
					}
					else
						nullable7 = new Decimal?(nullable2.GetValueOrDefault() * num3);
					nullable4 = nullable7;
					Decimal num4 = (Decimal)50;
					Decimal? nullable8;
					if (!nullable4.HasValue)
					{
						nullable2 = new Decimal?();
						nullable8 = nullable2;
					}
					else
						nullable8 = new Decimal?(nullable4.GetValueOrDefault() + num4);
					entity.Tscore = nullable8;
					nullable4 = nullable6;
					Decimal num5 = (Decimal)2;
					Decimal? nullable9;
					if (!nullable4.HasValue)
					{
						nullable2 = new Decimal?();
						nullable9 = nullable2;
					}
					else
						nullable9 = new Decimal?(nullable4.GetValueOrDefault() * num5 + new Decimal(55, 0, 0, false, (byte)1));
					entity.Stenscore = nullable9;
					entity.IsCompleted = new bool?(true);
					Decimal? nullable10;
					if (pillarAssessmentTool == null)
					{
						nullable4 = new Decimal?();
						nullable10 = nullable4;
					}
					else
						nullable10 = pillarAssessmentTool.Mean;
					entity.Mean = nullable10;
					Decimal? nullable11;
					if (pillarAssessmentTool == null)
					{
						nullable4 = new Decimal?();
						nullable11 = nullable4;
					}
					else
						nullable11 = pillarAssessmentTool.StandardDeviation;
					entity.StandardDeviation = nullable11;
					profilePillarScoresDal.Add(entity);
				}
			}
			await _appDbContext.SaveChangesAsync();
		}

		private async Task AssignFactorScore(int AssessmentToolID, int ProfileID, int Order, bool IsCompleted)
		{
			bool flag = false;
			if (!IsCompleted)
				flag = _appDbContext.ProfileFactorScores.Any(e => e.ProfileId == ProfileID && e.Order == e.Order && e.Factor.AssessmentToolId == AssessmentToolID);
			if (((flag ? 0 : (!IsCompleted ? 1 : 0)) | (IsCompleted ? 1 : 0)) == 0)
				return;
			var source = _appDbContext.Factors
				.Where(e => e.AssessmentToolId == AssessmentToolID)
				.Select(e => new
				{
					ID = e.Id,
					Mean = e.Mean,
					StandardDeviation = e.StandardDeviation,
					Scales = e.Scales,
					ProfileFactorScores = e.ProfileFactorScores
				}).ToList();
			foreach (var data in source)
			{
				var factor = data;
				Decimal? nullable1 = new Decimal?(new Decimal());
				ProfileFactorScore profileFactorScore1 = _appDbContext.ProfileFactorScores.FirstOrDefault(e => e.Order == Order && e.ProfileId == ProfileID && e.FactorId == factor.ID);
				if (IsCompleted)
				{
					nullable1 = new Decimal?(Queryable.Sum(_appDbContext.ProfileScaleScores.Include(x => x.Scale).Where(p => p.ProfileId == ProfileID && p.Order == Order && p.Scale.FactorId == factor.ID).Select(e => e.Score)));
					Decimal? nullable2 = nullable1;
					Decimal? mean = factor.Mean;
					Decimal? nullable3 = nullable2.HasValue & mean.HasValue ? new Decimal?(nullable2.GetValueOrDefault() - mean.GetValueOrDefault()) : new Decimal?();
					Decimal? nullable4 = factor.StandardDeviation;
					Decimal? nullable5 = nullable3.HasValue & nullable4.HasValue ? new Decimal?(nullable3.GetValueOrDefault() / nullable4.GetValueOrDefault()) : new Decimal?();
					if (profileFactorScore1 == null)
					{
						var profilefactorDal = _appDbContext.ProfileFactorScores;
						ProfileFactorScore entity = new ProfileFactorScore
						{
							ProfileId = ProfileID,
							FactorId = factor.ID,
							Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero,
							Order = Order,
							IsCompleted = true,
							StandardScore = nullable5,
							Created = DateTime.Now,
							Modified = DateTime.Now,
							CreatedBy = "APP",
							ModifiedBy = "APP"
						};
						nullable3 = nullable5;
						Decimal num1 = (Decimal)10;
						nullable4 = nullable3.HasValue ? new Decimal?(nullable3.GetValueOrDefault() * num1) : new Decimal?();
						Decimal num2 = (Decimal)50;
						Decimal? nullable6;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable6 = nullable3;
						}
						else
							nullable6 = new Decimal?(nullable4.GetValueOrDefault() + num2);
						entity.Tscore = nullable6;
						nullable4 = nullable5;
						Decimal num3 = (Decimal)2;
						Decimal? nullable7;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable7 = nullable3;
						}
						else
							nullable7 = new Decimal?(nullable4.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
						entity.Stenscore = nullable7;
						profilefactorDal.Add(entity);
					}
					else
					{
						profileFactorScore1.IsCompleted = true;
						profileFactorScore1.Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero;
						profileFactorScore1.StandardScore = nullable5;
						profileFactorScore1.Modified = DateTime.Now;
						profileFactorScore1.ModifiedBy = "APP";
						ProfileFactorScore profileFactorScore2 = profileFactorScore1;
						nullable3 = nullable5;
						Decimal num1 = (Decimal)10;
						nullable4 = nullable3.HasValue ? new Decimal?(nullable3.GetValueOrDefault() * num1) : new Decimal?();
						Decimal num2 = (Decimal)50;
						Decimal? nullable6;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable6 = nullable3;
						}
						else
							nullable6 = new Decimal?(nullable4.GetValueOrDefault() + num2);
						profileFactorScore2.Tscore = nullable6;
						ProfileFactorScore profileFactorScore3 = profileFactorScore1;
						nullable4 = nullable5;
						Decimal num3 = (Decimal)2;
						Decimal? nullable7;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable7 = nullable3;
						}
						else
							nullable7 = new Decimal?(nullable4.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
						profileFactorScore3.Stenscore = nullable7;
					}
				}
				else if (profileFactorScore1 == null)
					_appDbContext.ProfileFactorScores.Add(new ProfileFactorScore()
					{
						ProfileId = ProfileID,
						FactorId = factor.ID,
						Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero,
						Order = Order,
						IsCompleted = false,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = "APP",
						ModifiedBy = "APP"
					});
				await _appDbContext.SaveChangesAsync();
			}
		}

		private async Task AssignScaleScore(int AssessmentToolID, int ProfileID, int Order, bool IsCompleted, bool HasSubscale)
		{
			bool flag = false;
			if (!IsCompleted)
				flag = _appDbContext.ProfileScaleScores.Include(x => x.Scale).ThenInclude(x => x.Factor).Any(e => e.ProfileId == ProfileID && e.Order == e.Order && e.Scale.Factor.AssessmentToolId == AssessmentToolID);
			if (((flag ? 0 : (!IsCompleted ? 1 : 0)) | (IsCompleted ? 1 : 0)) == 0)
				return;
			var source = _appDbContext.Scales.Where(e => e.Factor.AssessmentToolId == AssessmentToolID).Select(e => new
			{
				Mean = e.Mean,
				StandardDeviation = e.StandardDeviation,
				ID = e.Id
			}).ToList();

			foreach (var data in source)
			{
				var scale = data;
				Decimal? nullable1 = new Decimal?(new Decimal());
				ProfileScaleScore profileScaleScore1 = _appDbContext.ProfileScaleScores.Where(e => e.ScaleId == scale.ID && e.ProfileId == ProfileID && e.Order == Order).FirstOrDefault();
				if (IsCompleted)
				{
					if (HasSubscale)
					{
						nullable1 = new Decimal?(Queryable.Sum(_appDbContext.ProfileSubScaleScores.Include(x => x.SubScale).Where(p => p.ProfileId == ProfileID && p.Order == Order && p.SubScale.ScaleId == scale.ID).Select(e => e.Score)));
					}
					else
					{
						nullable1 = new Decimal?(Queryable.Sum(_appDbContext.ProfileQuestionItemScores.Include(x => x.QuestionItem).Where(p => p.ProfileId == ProfileID && p.Order == Order && p.QuestionItem.ScaleId == (int?)scale.ID).Select(p => p.Score)));
					}
					Decimal? nullable2 = nullable1;
					Decimal? mean = scale.Mean;
					Decimal? nullable3 = nullable2.HasValue & mean.HasValue ? new Decimal?(nullable2.GetValueOrDefault() - mean.GetValueOrDefault()) : new Decimal?();
					Decimal? nullable4 = scale.StandardDeviation;
					Decimal? nullable5 = nullable3.HasValue & nullable4.HasValue ? new Decimal?(nullable3.GetValueOrDefault() / nullable4.GetValueOrDefault()) : new Decimal?();
					if (profileScaleScore1 == null)
					{
						var profilescaleDal = _appDbContext.ProfileScaleScores;
						ProfileScaleScore entity = new ProfileScaleScore
						{
							ProfileId = ProfileID,
							ScaleId = scale.ID,
							Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero,
							Order = Order,
							IsCompleted = true,
							StandardScore = nullable5,
							Created = DateTime.Now,
							Modified = DateTime.Now,
							CreatedBy = "APP",
							ModifiedBy = "APP"
						};
						nullable3 = nullable5;
						Decimal num1 = (Decimal)10;
						nullable4 = nullable3.HasValue ? new Decimal?(nullable3.GetValueOrDefault() * num1) : new Decimal?();
						Decimal num2 = (Decimal)50;
						Decimal? nullable6;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable6 = nullable3;
						}
						else
							nullable6 = new Decimal?(nullable4.GetValueOrDefault() + num2);
						entity.Tscore = nullable6;
						nullable4 = nullable5;
						Decimal num3 = (Decimal)2;
						Decimal? nullable7;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable7 = nullable3;
						}
						else
							nullable7 = new Decimal?(nullable4.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
						entity.Stenscore = nullable7;
						entity.Mean = scale.Mean;
						entity.StandardDeviation = scale.StandardDeviation;
						profilescaleDal.Add(entity);
					}
					else
					{
						profileScaleScore1.StandardScore = nullable5;
						ProfileScaleScore profileScaleScore2 = profileScaleScore1;
						nullable3 = nullable5;
						Decimal num1 = (Decimal)10;
						nullable4 = nullable3.HasValue ? new Decimal?(nullable3.GetValueOrDefault() * num1) : new Decimal?();
						Decimal num2 = (Decimal)50;
						Decimal? nullable6;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable6 = nullable3;
						}
						else
							nullable6 = new Decimal?(nullable4.GetValueOrDefault() + num2);
						profileScaleScore2.Tscore = nullable6;
						ProfileScaleScore profileScaleScore3 = profileScaleScore1;
						nullable4 = nullable5;
						Decimal num3 = (Decimal)2;
						Decimal? nullable7;
						if (!nullable4.HasValue)
						{
							nullable3 = new Decimal?();
							nullable7 = nullable3;
						}
						else
							nullable7 = new Decimal?(nullable4.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
						profileScaleScore3.Stenscore = nullable7;
						profileScaleScore1.IsCompleted = true;
						profileScaleScore1.Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero;
						profileScaleScore1.Mean = scale.Mean;
						profileScaleScore1.StandardDeviation = scale.StandardDeviation;
						profileScaleScore1.Modified = DateTime.Now;
						profileScaleScore1.ModifiedBy = "APP";
					}
				}
				else if (profileScaleScore1 == null)
					_appDbContext.ProfileScaleScores.Add(new ProfileScaleScore()
					{
						ProfileId = ProfileID,
						ScaleId = scale.ID,
						Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero,
						Order = Order,
						IsCompleted = false,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						CreatedBy = "APP",
						ModifiedBy = "APP"
					});
				await _appDbContext.SaveChangesAsync();
			}
		}

		private async Task AssignSubScaleScore(int AssessmentToolID, int ProfileID, int Order, bool IsCompleted)
		{
			try
			{
				bool flag = false;
				if (!IsCompleted)
					flag = _appDbContext.ProfileSubScaleScores.Any(e => e.ProfileId == ProfileID && e.Order == e.Order && e.SubScale.Scale.Factor.AssessmentToolId == AssessmentToolID);
				if (((flag ? 0 : (!IsCompleted ? 1 : 0)) | (IsCompleted ? 1 : 0)) == 0)
					return;
				List<int> factorIDs = _appDbContext.Factors.Where(e => e.AssessmentToolId == AssessmentToolID).Select(e => e.Id).ToList();
				List<int> ScaleIDs = _appDbContext.Scales.Where(e => factorIDs.Contains(e.FactorId)).Select(e => e.Id).ToList();
				var source = _appDbContext.SubScales.AsNoTracking().Select(e => new
				{
					ID = e.Id,
					Mean = e.Mean,
					StandardDeviation = e.StandardDeviation,
					ScaleID = e.ScaleId
				}).ToList();

				foreach (var data in source.Where(e => ScaleIDs.Contains(e.ScaleID)).ToList())
				{
					var subscale = data;
					Decimal? nullable1 = new Decimal?(new Decimal());
					var profileSubScaleScore1 = _appDbContext.ProfileSubScaleScores
						.FirstOrDefault(e => e.SubScaleId == subscale.ID && e.ProfileId == ProfileID && e.Order == Order);
					if (IsCompleted)
					{
						nullable1 = new Decimal?(Queryable.Sum(_appDbContext.ProfileQuestionItemScores.Include(x => x.QuestionItem).Where(p => p.ProfileId == ProfileID && p.Order == Order && p.QuestionItem.SubScaleId == subscale.ID).Select(p => p.Score)));
						Decimal? nullable2 = nullable1;
						Decimal? nullable3 = subscale.Mean;
						Decimal? nullable4 = nullable2.HasValue & nullable3.HasValue ? new Decimal?(nullable2.GetValueOrDefault() - nullable3.GetValueOrDefault()) : new Decimal?();
						Decimal? nullable5 = subscale.StandardDeviation;
						Decimal? nullable6;
						if (!(nullable4.HasValue & nullable5.HasValue))
						{
							nullable3 = new Decimal?();
							nullable6 = nullable3;
						}
						else
							nullable6 = new Decimal?(nullable4.GetValueOrDefault() / nullable5.GetValueOrDefault());
						Decimal? nullable7 = nullable6;
						if (profileSubScaleScore1 != null)
						{
							profileSubScaleScore1.StandardScore = nullable7;
							ProfileSubScaleScore profileSubScaleScore2 = profileSubScaleScore1;
							nullable4 = nullable7;
							Decimal num1 = (Decimal)10;
							Decimal? nullable8;
							if (!nullable4.HasValue)
							{
								nullable3 = new Decimal?();
								nullable8 = nullable3;
							}
							else
								nullable8 = new Decimal?(nullable4.GetValueOrDefault() * num1);
							nullable5 = nullable8;
							Decimal num2 = (Decimal)50;
							Decimal? nullable9;
							if (!nullable5.HasValue)
							{
								nullable4 = new Decimal?();
								nullable9 = nullable4;
							}
							else
								nullable9 = new Decimal?(nullable5.GetValueOrDefault() + num2);
							profileSubScaleScore2.Tscore = nullable9;
							ProfileSubScaleScore profileSubScaleScore3 = profileSubScaleScore1;
							nullable5 = nullable7;
							Decimal num3 = (Decimal)2;
							Decimal? nullable10;
							if (!nullable5.HasValue)
							{
								nullable4 = new Decimal?();
								nullable10 = nullable4;
							}
							else
								nullable10 = new Decimal?(nullable5.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
							profileSubScaleScore3.Stenscore = nullable10;
							profileSubScaleScore1.IsCompleted = true;
							profileSubScaleScore1.Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero;
							profileSubScaleScore1.Mean = subscale.Mean;
							profileSubScaleScore1.StandardDeviation = subscale.StandardDeviation;
							profileSubScaleScore1.Modified = DateTime.Now;
							profileSubScaleScore1.ModifiedBy = "APP";
						}
						else
						{
							var profilesubscaleDal = _appDbContext.ProfileSubScaleScores;
							ProfileSubScaleScore entity = new ProfileSubScaleScore();
							entity.ProfileId = ProfileID;
							entity.SubScaleId = subscale.ID;
							entity.Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero;
							entity.Order = Order;
							entity.StandardScore = nullable7;
							nullable4 = nullable7;
							Decimal num1 = (Decimal)10;
							Decimal? nullable8;
							if (!nullable4.HasValue)
							{
								nullable3 = new Decimal?();
								nullable8 = nullable3;
							}
							else
								nullable8 = new Decimal?(nullable4.GetValueOrDefault() * num1);
							nullable5 = nullable8;
							Decimal num2 = (Decimal)50;
							Decimal? nullable9;
							if (!nullable5.HasValue)
							{
								nullable4 = new Decimal?();
								nullable9 = nullable4;
							}
							else
								nullable9 = new Decimal?(nullable5.GetValueOrDefault() + num2);
							entity.Tscore = nullable9;
							nullable5 = nullable7;
							Decimal num3 = (Decimal)2;
							Decimal? nullable10;
							if (!nullable5.HasValue)
							{
								nullable4 = new Decimal?();
								nullable10 = nullable4;
							}
							else
								nullable10 = new Decimal?(nullable5.GetValueOrDefault() * num3 + new Decimal(55, 0, 0, false, (byte)1));
							entity.Stenscore = nullable10;
							entity.IsCompleted = true;
							entity.Mean = subscale.Mean;
							entity.StandardDeviation = subscale.StandardDeviation;
							entity.Created = DateTime.Now;
							entity.Modified = DateTime.Now;
							entity.CreatedBy = "APP";
							entity.ModifiedBy = "APP";
							profilesubscaleDal.Add(entity);
						}
					}
					else if (profileSubScaleScore1 == null)
						_appDbContext.ProfileSubScaleScores.Add(new ProfileSubScaleScore()
						{
							ProfileId = ProfileID,
							SubScaleId = subscale.ID,
							Score = nullable1.HasValue ? nullable1.Value : Decimal.Zero,
							Order = Order,
							IsCompleted = false,
							Created = DateTime.Now,
							Modified = DateTime.Now,
							CreatedBy = "APP",
							ModifiedBy = "APP",
						});
					await _appDbContext.SaveChangesAsync();
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private async Task AssignCompetnciesScore(int AssessmentToolID, int ProfileID, int Order, bool IsCompleted)
		{
			var source = _appDbContext.CompetencySubscales.Include(x => x.Subscale).ThenInclude(x => x.Scale).ThenInclude(x => x.Factor).Where(e => e.Subscale.Scale.Factor.AssessmentToolId == AssessmentToolID).Select(e => new
			{
				CompetencyID = e.CompetencyId,
				profileSubscores = e.Subscale.ProfileSubScaleScores.Select(ss => new
				{
					ProfileID = ss.ProfileId,
					Order = ss.Order,
					AssessmentToolID = ss.SubScale.Scale.Factor.AssessmentToolId,
					Score = ss.Score
				}),
				Weight = e.Weight
			}).ToList();
			foreach (var grouping in source.GroupBy(e => e.CompetencyID).ToList())
			{
				var competency = grouping;
				int CompetencyID = competency.Key.GetValueOrDefault();
				Decimal num1 = new Decimal();
				foreach (var data1 in competency.ToList())
				{
					foreach (var data2 in data1.profileSubscores.Where(e => e.ProfileID == ProfileID && e.Order == Order && e.AssessmentToolID == AssessmentToolID).ToList())
						num1 += data2.Score * data1.Weight;
				}
				var data = _appDbContext.CompetencyAssessmentTools.Where(e => e.AssessmentToolId == AssessmentToolID && (int?)e.CompetencyId == competency.Key).Select(c => new
				{
					c.Mean,
					c.StandardDeviation
				}).FirstOrDefault();
				Decimal num2 = num1;
				Decimal? nullable1 = data.Mean;
				Decimal? nullable2 = nullable1.HasValue ? new Decimal?(num2 - nullable1.GetValueOrDefault()) : new Decimal?();
				Decimal? standardDeviation = data.StandardDeviation;
				Decimal? nullable3;
				if (!(nullable2.HasValue & standardDeviation.HasValue))
				{
					nullable1 = new Decimal?();
					nullable3 = nullable1;
				}
				else
					nullable3 = new Decimal?(nullable2.GetValueOrDefault() / standardDeviation.GetValueOrDefault());
				Decimal? nullable4 = nullable3;
				if (_appDbContext.ProfileCompetencyScores.FirstOrDefault(e => e.ProfileId == ProfileID && e.CompetencyId == CompetencyID && e.Order == Order && e.AssessmenttoolId == AssessmentToolID) == null)
				{
					var profilecompetencysubscalescoreDal = _appDbContext.ProfileCompetencyScores;
					ProfileCompetencyScore entity = new ProfileCompetencyScore
					{
						ProfileId = ProfileID,
						CompetencyId = CompetencyID,
						Order = Order,
						AssessmenttoolId = AssessmentToolID,
						IsCompleted = new bool?(true),
						Score = num1,
						StandardScore = nullable4
					};
					nullable2 = nullable4;
					Decimal num3 = (Decimal)10;
					Decimal? nullable5;
					if (!nullable2.HasValue)
					{
						nullable1 = new Decimal?();
						nullable5 = nullable1;
					}
					else
						nullable5 = new Decimal?(nullable2.GetValueOrDefault() * num3);
					Decimal? nullable6 = nullable5;
					Decimal num4 = (Decimal)50;
					Decimal? nullable7;
					if (!nullable6.HasValue)
					{
						nullable2 = new Decimal?();
						nullable7 = nullable2;
					}
					else
						nullable7 = new Decimal?(nullable6.GetValueOrDefault() + num4);
					entity.Tscore = nullable7;
					nullable6 = nullable4;
					Decimal num5 = (Decimal)2;
					Decimal? nullable8;
					if (!nullable6.HasValue)
					{
						nullable2 = new Decimal?();
						nullable8 = nullable2;
					}
					else
						nullable8 = new Decimal?(nullable6.GetValueOrDefault() * num5 + new Decimal(55, 0, 0, false, (byte)1));
					entity.Stenscore = nullable8;
					entity.Mean = data.Mean;
					entity.StandardDeviation = data.StandardDeviation;
					entity.Created = DateTime.Now;
					entity.Modified = DateTime.Now;
					entity.CreatedBy = "APP";
					entity.ModifiedBy = "APP";
					profilecompetencysubscalescoreDal.Add(entity);
				}
			}
			await _appDbContext.SaveChangesAsync();
		}

		private AssessmentToolView GetHasSubscaleAndCategory(int ID)
		{
			AssessmentToolView assessmentToolView = new AssessmentToolView();
			var data = _appDbContext.AssessmentTools.Where(e => e.Id == ID).Select(e => new
			{
				HasSubscale = e.HasSubscale,
				AssessmentToolCategory = e.AssessmentToolCategory,
				ID = e.Id
			}).FirstOrDefault();
			if (data == null)
				throw new ArgumentException("Invalid assessmenttoolID: " + (object)ID);
			assessmentToolView.AssessmentToolCategID = data.AssessmentToolCategory;
			assessmentToolView.HasSubScale = data.HasSubscale;
			return assessmentToolView;
		}

		private async Task UpdateTrialProcessing(int AssessmenttoolID, UserInfo user, int Order)
		{
			try
			{
				var profileId = user.UserId;
				var assessmentToolScore = await _appDbContext.ProfileAssessmentToolScores.FirstOrDefaultAsync(x => x.ProfileId == profileId && x.AssessmentToolId == AssessmenttoolID && x.Order == Order);
				if (assessmentToolScore != null)
					assessmentToolScore.IsProcessing = true;
				else
					_appDbContext.ProfileAssessmentToolScores.Add(new Models.ProfileAssessmentToolScore()
					{
						ProfileId = profileId,
						AssessmentToolId = AssessmenttoolID,
						Score = decimal.Zero,
						Order = Order,
						IsCompleted = false,
						IsProcessing = true,
						Created = DateTime.Now,
						Modified = DateTime.Now,
						ModifiedBy = user.Email,
						CreatedBy = user.Email
					});
				await _appDbContext.SaveChangesAsync();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private bool CheckProcessing(int id, int profileID)
		{
			return _appDbContext.ProfileAssessmentToolScores.Any(p => p.AssessmentToolId == id && p.ProfileId == profileID && p.IsProcessing);
		}

		private bool IsCompleted(int id, int profileID, int ValidityRangeNumber)
		{
			
			var isCompleted = _appDbContext.ProfileAssessmentToolScores.Any(p => p.AssessmentToolId == id && p.ProfileId == profileID && p.IsCompleted == true);
			var TimeLeft = CheckValidity(id, profileID, ValidityRangeNumber) ? new TimeDiffereceView() : this.GetMonthDifference(id, profileID, ValidityRangeNumber);

			if(TimeLeft.MonthsLeft == 0 && TimeLeft.DaysLeft == 0 && isCompleted == true)
            {
				return false;
            }
			return isCompleted;


		}

		private bool IsFailed(int id, int profileID, int ValidityRangeNumber)
		{
			var isFailed = _appDbContext.ProfileAssessmentToolScores.Any(p => p.AssessmentToolId == id && p.ProfileId == profileID && p.IsFailed == true);
			var TimeLeft = CheckValidity(id, profileID, ValidityRangeNumber) ? new TimeDiffereceView() : this.GetMonthDifference(id, profileID, ValidityRangeNumber);

			if (TimeLeft.MonthsLeft == 0 && TimeLeft.DaysLeft == 0 && isFailed == true)
			{
				return false;
			}
			return isFailed;
		}

		private bool CheckValidity(int AssessmentToolID, int ProfileID, int ValidityRangeNumber)
		{
			var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(p => p.AssessmentToolId == AssessmentToolID && p.ProfileId == ProfileID && p.IsCompleted == true).OrderByDescending(x => x.Order).FirstOrDefault();
			return assessmentToolScore == null || assessmentToolScore.Created.AddMonths(ValidityRangeNumber) <= DateTime.Now;
		}

		private TimeDiffereceView GetMonthDifference(int AssessmentToolID, int ProfileID, int ValidityRangeNumber)
		{
			TimeDiffereceView timeDiffereceView = new TimeDiffereceView();
			var assessmentToolScore = _appDbContext.ProfileAssessmentToolScores.Where(p => p.AssessmentToolId == AssessmentToolID && p.ProfileId == ProfileID && p.IsCompleted == true).OrderByDescending(e => e.Order).FirstOrDefault();
			if (assessmentToolScore == null)
				return new TimeDiffereceView();
			DateTime modified = assessmentToolScore.Modified;
			DateTime now = DateTime.Now;
			return Count(DateTime.Now, assessmentToolScore.Created.AddMonths(ValidityRangeNumber));
		}

		private TimeDiffereceView Count(DateTime Bday, DateTime Cday)
		{
			if (Cday.Year - Bday.Year <= 0 && (Cday.Year - Bday.Year != 0 || Bday.Month >= Cday.Month && (Bday.Month != Cday.Month || Bday.Day > Cday.Day)))
				throw new ArgumentException("Created date must be earlier than current date");
			int num1 = DateTime.DaysInMonth(Bday.Year, Bday.Month);
			int num2 = Cday.Day + (num1 - Bday.Day);
			int Months;
			int Days;
			if (Cday.Month > Bday.Month)
			{
				int year1 = Cday.Year;
				int year2 = Bday.Year;
				Months = Cday.Month - (Bday.Month + 1) + Math.Abs(num2 / num1);
				Days = (num2 % num1 + num1) % num1;
			}
			else if (Cday.Month == Bday.Month)
			{
				if (Cday.Day >= Bday.Day)
				{
					int year1 = Cday.Year;
					int year2 = Bday.Year;
					Months = 0;
					Days = Cday.Day - Bday.Day;
				}
				else
				{
					int year1 = Cday.Year;
					int year2 = Bday.Year;
					Months = 11;
					Days = DateTime.DaysInMonth(Bday.Year, Bday.Month) - (Bday.Day - Cday.Day);
				}
			}
			else
			{
				int year1 = Cday.Year;
				int year2 = Bday.Year;
				Months = Cday.Month + (11 - Bday.Month) + Math.Abs(num2 / num1);
				Days = (num2 % num1 + num1) % num1;
			}
			return new TimeDiffereceView(Months, Days);
		}

		private class batchPrec
		{
			public int ProgrammeID { get; set; }

			public int Percentage { get; set; }
		}
	}
}
