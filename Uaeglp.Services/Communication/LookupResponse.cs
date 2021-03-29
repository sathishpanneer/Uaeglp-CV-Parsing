using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class LookupResponse : BaseResponse, ILookupResponse
    {

        public List<LookupItemView> LookupItems { get; set; }
        public List<ProfileEducationFieldOfStudyView> EducationFieldOfStudyViews { get; set; }
        public List<OrganizationView> OrganizationViews { get; set; }
        public List<CountryView> CountryViews { get; set; }
        public List<IndustryView> IndustryViews { get; set; }
        public List<ProfileSkillView> ProfileSkillViews { get; set; }
        public List<ProfileWorkExperienceJobTitleView> JobTitleViews { get; set; }
        public List<WorkFieldView> WorkFieldViews { get; set; }
        public AllLookupItemsView AllLookupItemsView { get; set; }


        private LookupResponse(bool success, string message, AllLookupItemsView lookupItems) : base(success, message)
        {
            AllLookupItemsView = lookupItems;
        }
        private LookupResponse(bool success, string message, List<LookupItemView> lookupItems) : base(success, message)
        {
            LookupItems = lookupItems;
        }

        private LookupResponse(bool success, string message, List<ProfileEducationFieldOfStudyView> lookupItems) : base(success, message)
        {
            EducationFieldOfStudyViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<OrganizationView> lookupItems) : base(success, message)
        {
            OrganizationViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<CountryView> lookupItems) : base(success, message)
        {
            CountryViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<IndustryView> lookupItems) : base(success, message)
        {
            IndustryViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<ProfileSkillView> lookupItems) : base(success, message)
        {
            ProfileSkillViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<ProfileWorkExperienceJobTitleView> lookupItems) : base(success, message)
        {
            JobTitleViews = lookupItems;
        }

        private LookupResponse(bool success, string message, List<WorkFieldView> lookupItems) : base(success, message)
        {
            WorkFieldViews = lookupItems;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public LookupResponse(string message, HttpStatusCode status) : base(false, message)
        { }


        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<LookupItemView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<ProfileEducationFieldOfStudyView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<OrganizationView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { } 
        
        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<CountryView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<IndustryView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<ProfileSkillView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }


        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<ProfileWorkExperienceJobTitleView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="lookupItemViews">bioView view model.</param>
        /// <returns>Response.</returns>
        public LookupResponse(List<WorkFieldView> lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        public LookupResponse(AllLookupItemsView lookupItemViews) : this(true, string.Empty, lookupItemViews)
        { }

        public LookupResponse(Exception e) : base(e)
        {

        }


    }
}
