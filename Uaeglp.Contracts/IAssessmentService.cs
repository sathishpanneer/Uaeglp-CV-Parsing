using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Contracts
{
	public interface IAssessmentService
	{
		Task<IAssessmentResponse<AssessmentToolReportViewModel>> GetReportByProfile(int ProfileID, int? AssessmentToolID);
		Task<IAssessmentResponse<AssessmentToolReportViewModel>> GetProfileDrillDownByGroup_New(int ProfileID, int groupId, int? AssessmentToolID);
		
		Task<IAssessmentResponse<object>> GetListByProfile(int ProfileID);
		Task<IAssessmentResponse<List<AssessmentToolView>>> GetListByProfileAndGroupID(int ProfileID, int GroupID);
		Task<IAssessmentResponse<AssessmentToolView>> GetAssessmentDetails(int ID, LanguageType lang = LanguageType.AR);
		Task<IAssessmentResponse<AssessmentQuestionsView>> GetAssessmentQuestions(int AssessmentToolID, int profileID, LanguageType lang = LanguageType.AR);
		Task<IAssessmentResponse<SubmitedAssessmentAnswersView>> SubmitAssessmentAnswers(int assessmentId, int profileId, int skip, int order, AssessmentAnswersView assessmentAnswersView);
		Task<IAssessmentResponse<SubmitedAssessmentHeadAnswersView>> SubmitAssessmentHeadAnswers(int assessmentId, int profileId,
			List<QuestionScoreView> QuestionItemScores,
			  List<AssessmentNavigationObjectView> AssessmentNavigationObject,
			  int TotalTestCount,
			  int Order);
		Task<IAssessmentResponse<string>> SubmitAllScores(int assessmentId, int profileId, int order, AssessmentAnswersView assessmentAnswersView, LanguageType lang = LanguageType.AR);
		Task<IAssessmentResponse<string>> SubmitHeadAllScores(int assessmentId, int profileId, int order,
			List<QuestionScoreView> questionItemScores, LanguageType lang = LanguageType.AR);
		Task<IAssessmentResponse<decimal>> CalculatePercentge(int profileID);
		Task<IAssessmentResponse<List<AssessmentGroupView>>> GetAssessmentGroupsByUserId(int userId, bool isAdmin = false);
		Task<IAssessmentResponse<List<ProfileView>>> GetMembers(int assessmentGroupId);
		Task<IAssessmentResponse<AssessmentGroupMember>> AddMember(int profileId, int groupId);
		Task<IAssessmentResponse<bool>> DeleteMember(int memberID, int groupID);
		Task<IAssessmentResponse<AssessmentGroupView>> AddAssessmentGroup(AssessmentGroupView assessmentGroupView);
		Task<IAssessmentResponse<AssessmentGroupView>> UpdateAssessmentGroup(AssessmentGroupView assessmentGroupView);
		Task<IAssessmentResponse<bool>> DeleteAssessmentGroup(int id);
		Task<IAssessmentResponse<AssessmentGroupView>> GetAssessmentGroup(int id);
		Task<IAssessmentResponse<AssessmentIndividualToolReportView>> GenerateIndividualProfileReport(int ProfileAssessmentID);
		Task<IAssessmentResponse<AssessmentIndividualToolReportView>> ExportAssessmentReportPDF(int ProfileAssessmentReport, string language = "AR");
		Task<IAssessmentResponse<bool>> IsCoordinator(int profileID);
		Task<IAssessmentResponse<bool>> MakeCoordinator(int profileID);
		Task<IAssessmentResponse<bool>> RemoveCoordinator(int profileID);
		Task<IAssessmentResponse<bool>> ToggleCoordintor(int memberID, int groupID);
		Task<IAssessmentResponse<AssessmentGroupProfileView>> GetProfileDrillDownByGroup(int ProfileID, int GroupID);
		Task<IAssessmentResponse<QuestionWithAnswerView>> GetQuestionsWithAnswers(int paidID, int userID);
		Task<IAssessmentResponse<bool>> AnswerFeedBackQuestions(QuestionWithAnswerView_New questionWithAnswerView, int userID);
		FileViewModel ExportGroupAssessmentToolsReport(int GroupID, string language);
	}
}