using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels.KnowledgeHub;

namespace Uaeglp.Contracts.Communication
{
	public interface IKnowledgeHubResponse : IBaseResponse
	{
         List<KnowledgeHubView> KnowledgeHubs { get; set; }
         List<KnowledgeHubCategoryView> Categories { get; set; }
         List<KnowledgeHubCategoryCourseView> ALLCoursesbyCategories { get; set; }
         List<KnowledgeHubCourseView> AllCourses { get; set; }
         KnowledgeHubView KnowledgeHub { get; set; }
         KnowledgeHubCourseIframe CourseIframe { get; set; }
        long CourseCount { get; set; }
        bool Favourite { get; set; }
         List<FolderView> FoldersList { get; set; }
         LibraryView FolderandFilesList { get; set; }
    }
}
