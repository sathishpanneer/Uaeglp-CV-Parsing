using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.AssessmentViewModels;
using Uaeglp.ViewModels.KnowledgeHub;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class KnowledgeHubResponse : BaseResponse, IKnowledgeHubResponse
    {
        public List<KnowledgeHubView> KnowledgeHubs { get; set; }
        public List<KnowledgeHubCategoryView> Categories { get; set; }
        public List<KnowledgeHubCategoryCourseView> ALLCoursesbyCategories { get; set; }
        public List<KnowledgeHubCourseView> AllCourses { get; set; }
        public KnowledgeHubView KnowledgeHub { get; set; }
        public KnowledgeHubCourseIframe CourseIframe { get; set; }
        public bool Favourite { get; set; }
       public long CourseCount { get; set; }
        public List<FolderView> FoldersList { get; set; }
        public LibraryView FolderandFilesList { get; set; }
        private KnowledgeHubResponse(bool success, string message, List<KnowledgeHubView> views) : base(success, message)
        {
            KnowledgeHubs = views;
        }
        private KnowledgeHubResponse(bool success, string message, List<KnowledgeHubCategoryView> views) : base(success, message)
        {
            Categories = views;
        }
        private KnowledgeHubResponse(bool success, string message, List<KnowledgeHubCategoryCourseView> views) : base(success, message)
        {
            ALLCoursesbyCategories = views;
        }
        private KnowledgeHubResponse(bool success, string message, List<KnowledgeHubCourseView> views) : base(success, message)
        {
            AllCourses = views;
        }
        private KnowledgeHubResponse(bool success, string message, KnowledgeHubView view) : base(success, message)
        {
            KnowledgeHub = view;
        }
        private KnowledgeHubResponse(bool success, string message, KnowledgeHubCourseIframe view) : base(success, message)
        {
            CourseIframe = view;
        }
        private KnowledgeHubResponse(bool success, string message, bool view) : base(success, message)
        {
            Favourite = view;
        }
        private KnowledgeHubResponse(bool success, string message, long view) : base(success, message)
        {
            CourseCount = view;
        }
        private KnowledgeHubResponse(bool success, string message, List<FolderView> view) : base(success, message)
        {
            FoldersList = view;
        }
        private KnowledgeHubResponse(bool success, string message, LibraryView view) : base(success, message)
        {
            FolderandFilesList = view;
        }
        public KnowledgeHubResponse(List<KnowledgeHubView> views) : this(true, string.Empty, views)
        { }

        public KnowledgeHubResponse(List<KnowledgeHubCategoryView> views) : this(true, string.Empty, views)
        { }
        public KnowledgeHubResponse(List<KnowledgeHubCategoryCourseView> views) : this(true, string.Empty, views)
        { }
        public KnowledgeHubResponse(List<KnowledgeHubCourseView> views) : this(true, string.Empty, views)
        { }
        public KnowledgeHubResponse(KnowledgeHubView view) : this(true, string.Empty, view)
        { }
        public KnowledgeHubResponse(KnowledgeHubCourseIframe view) : this(true, string.Empty, view)
        { }
        public KnowledgeHubResponse(bool view) : this(true, string.Empty, view)
        { }
        public KnowledgeHubResponse(long view) : this(true, string.Empty, view)
        { }
        public KnowledgeHubResponse(List<FolderView> view) : this(true, string.Empty, view)
        { }
        public KnowledgeHubResponse(LibraryView view) : this(true, string.Empty, view)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public KnowledgeHubResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        public KnowledgeHubResponse(Exception e) : base(e)
        { }

        public KnowledgeHubResponse() : base()
        { }

    }
}
