using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.KnowledgeHub;

namespace Uaeglp.Contracts
{
	public interface IKnowledgeHubService
	{
		Task<IKnowledgeHubResponse> GetKnowledgeHubs(LanguageType language = LanguageType.AR);

		Task<IKnowledgeHubResponse> GetCategories();

		Task<IKnowledgeHubResponse> GetCoursesbyCategories(int categoryId, int profileid, int skip = 0, int take = 5);

		Task<IKnowledgeHubResponse> GetDetalis(int id, LanguageType language = LanguageType.AR);

		Task<IKnowledgeHubResponse> GetKnowledgeHubCourses(string searchText, int profileid, int? providerTypeId, int? categoryId, bool isProfile, LanguageType language = LanguageType.AR, int skip = 0, int take = 5);

		Task<IKnowledgeHubResponse> GetCourseIframe(int courseId, LanguageType language = LanguageType.AR);
		Task<IKnowledgeHubResponse> GetALLCoursesbyCategories(int profileid, int skip = 0, int take = 5);
		Task<IKnowledgeHubResponse> SetFavourite(int courseid, int profileid);
		Task<IKnowledgeHubResponse> SetUnFavourite(int courseid, int profileid);
		Task<IKnowledgeHubResponse> GetFavouriteCourses(int profileid, int skip = 0, int take = 5);
		Task<IKnowledgeHubResponse> GetFolderandfiles(int profileid, int parentfolderid);
		Task<IKnowledgeHubResponse> GetFolders(int profileid);
	}
}
