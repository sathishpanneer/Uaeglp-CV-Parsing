using System;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class ProfileAssessmentResponse : BaseResponse, IProfileAssessmentResponse
	{

		public ProfileEducationView ProfileEducationView { get; set; }
		public BioView BioView { get; set; }
		public ProfileWorkExperienceView ProfileWorkExperienceView { get; set; }
		public SkillAndInterestView SkillAndInterestView { get; set; }
		public ProfileAchievementView ProfileAchievementView { get; set; }
		public ProfileMembershipView ProfileMembershipView { get; set; }
		public ProfileTrainingView ProfileTrainingView { get; set; }

	

        private ProfileAssessmentResponse(bool success, string message, BioView bioView) : base(success, message)
        {
            BioView = bioView;
        }

        private ProfileAssessmentResponse(bool success, string message, ProfileEducationView profileEducationView) : base(success, message)
        {
            ProfileEducationView = profileEducationView;
        }

        private ProfileAssessmentResponse(bool success, string message, ProfileWorkExperienceView profileWorkExperienceView) : base(success, message)
        {
            ProfileWorkExperienceView = profileWorkExperienceView;
        }

        private ProfileAssessmentResponse(bool success, string message, SkillAndInterestView skillAndInterestView) : base(success, message)
        {
            SkillAndInterestView = skillAndInterestView;
        }


        private ProfileAssessmentResponse(bool success, string message, ProfileAchievementView profileAchievementView) : base(success, message)
        {
            ProfileAchievementView = profileAchievementView;
        }  
        private ProfileAssessmentResponse(bool success, string message, ProfileMembershipView profileMembershipView) : base(success, message)
        {
            ProfileMembershipView = profileMembershipView;
        }

        private ProfileAssessmentResponse(bool success, string message, ProfileTrainingView profileTrainingView) : base(success, message)
        {
            ProfileTrainingView = profileTrainingView;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public ProfileAssessmentResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public ProfileAssessmentResponse(bool state, string message, HttpStatusCode status) : base(state, message, status)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="bioView">bioView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(BioView bioView) : this(true, string.Empty, bioView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileEducationView">profileEducationView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(ProfileEducationView profileEducationView) : this(true, string.Empty, profileEducationView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileWorkExperienceView">profileWorkExperienceView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(ProfileWorkExperienceView profileWorkExperienceView) : this(true, string.Empty, profileWorkExperienceView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="skillAndInterestView">skillAndInterestView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(SkillAndInterestView skillAndInterestView) : this(true, string.Empty, skillAndInterestView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileAchievementView">skillAndInterestView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(ProfileAchievementView profileAchievementView) : this(true, string.Empty, profileAchievementView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileMembershipView">skillAndInterestView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(ProfileMembershipView profileMembershipView) : this(true, string.Empty, profileMembershipView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileTrainingView">skillAndInterestView view model.</param>
        /// <returns>Response.</returns>
        public ProfileAssessmentResponse(ProfileTrainingView  profileTrainingView) : this(true, string.Empty, profileTrainingView)
        { }


        public ProfileAssessmentResponse() : base()
        { }

        public ProfileAssessmentResponse(Exception ex) : base(ex)
        { }
    }
}
