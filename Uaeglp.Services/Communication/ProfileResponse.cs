using System;
using System.Collections.Generic;
using System.Net;
using Uaeglp.Contract.Communication;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models.ProfileModels;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Services.Communication
{
	public class ProfileResponse : BaseResponse, IProfileResponse
	{
		public ProfileView Profile { get; set; }
		public IList<ProfileView> Profiles { get; set; }
        public FavoriteProfile FavoriteProfile { get; set; }
        public IList<MyProfileView> Recommendations { get; set; }
        public SearchPublicProfileView SearchPublicProfile { get; set; }
        public ContactDetailsView ContactDetailsView { get; set; }
		public ProfileNameView ProfileNameView { get; set; }
		public PersonalInfoView PersonalInfoView { get; set; }
		public LanguageAndProficiencyView LanguageAndProficiencyView { get; set; }
		public CountriesAndEmiratesView CountriesAndEmiratesView { get; set; }
        public LanguageItemView LanguageItemView { get; set; }
        public CVSavedResponseView CVProfileView { get; set; }
        public MyProfileView MyProfileView { get; set; }
        public List<FileView> DocumentsInfoList { get; set; }
        public ProfileFollowingsView ProfileFollowingsView { get; set; }
        public ProfileFollowersView ProfileFollowersView { get; set; }
        public List<ProfileAlumniList> ProfileAlumni { get; set; }
        public List<TrackApplication> TrackApplication { get; set; }
        public List<AllTrackApplicationView> AllTrackApplicationList { get; set; }
        public SearchTrackApplicationView SearchTrackApplication { get; set; }
        public UserCustomNotificationView CustomNotification { get; set; }
        public List<UserCustomNotificationView> CustomNotificationList { get; set; }
        public UseQRCode UserQRCode { get; set; }
        private ProfileResponse(bool success, string message, ProfileView profile) : base(success, message)
        {
            Profile = profile;
        }
        private ProfileResponse(bool success, string message, UseQRCode code) : base(success, message)
        {
            UserQRCode = code;
        }
        private ProfileResponse(bool success, string message, IList<ProfileView> profiles) : base(success, message)
		{
			Profiles = profiles;
		}
        private ProfileResponse(bool success, string message, FavoriteProfile favoriteProfile) : base(success, message)
        {
            FavoriteProfile = favoriteProfile;
        }

        private ProfileResponse(bool success, string message, IList<MyProfileView> profiles) : base(success, message)
        {
            Recommendations = profiles;
        }
        private ProfileResponse(bool success, string message, SearchPublicProfileView searchPublicProfileView) : base(success, message)
        {
            SearchPublicProfile = searchPublicProfileView;
        }

        private ProfileResponse(bool success, string message, ContactDetailsView contactDetails) : base(success, message)
        {
            ContactDetailsView = contactDetails;
        }

        private ProfileResponse(bool success, string message, ProfileNameView profileName) : base(success, message)
        {
            ProfileNameView = profileName;
        }

        private ProfileResponse(bool success, string message, PersonalInfoView personalInfo) : base(success, message)
        {
            PersonalInfoView = personalInfo;
        }

        private ProfileResponse(bool success, string message, LanguageAndProficiencyView languageAndProficiencyView) : base(success, message)
        {
            LanguageAndProficiencyView = languageAndProficiencyView;
        }

        private ProfileResponse(bool success, string message, CountriesAndEmiratesView languageAndProficiencyView) : base(success, message)
        {
            CountriesAndEmiratesView = languageAndProficiencyView;
        }


        private ProfileResponse(bool success, string message, LanguageItemView languageItemView) : base(success, message)
        {
            LanguageItemView = languageItemView;
        }
        private ProfileResponse(bool success, string message, CVSavedResponseView cvProfileView) : base(success, message)
        {
            CVProfileView = cvProfileView;
        }

        private ProfileResponse(bool success, string message, MyProfileView myProfileView) : base(success, message)
        {
            MyProfileView = myProfileView;
        }

        private ProfileResponse(bool success, string message, List<FileView> myProfileView) : base(success, message)
        {
            DocumentsInfoList = myProfileView;
        }

        private ProfileResponse(bool success, string message, ProfileFollowingsView profileFollowingsView) : base(success, message)
        {
            ProfileFollowingsView = profileFollowingsView;
        }

        private ProfileResponse(bool success, string message, ProfileFollowersView profileFollowersView) : base(success, message)
        {
            ProfileFollowersView = profileFollowersView;
        }

        private ProfileResponse(bool success, string message, List<ProfileAlumniList> profileAlumni) : base(success, message)
        {
            ProfileAlumni = profileAlumni;
        }

        private ProfileResponse(bool success, string message, List<TrackApplication> trackApp) : base(success, message)
        {
            TrackApplication = trackApp;
        }

        private ProfileResponse(bool success, string message, List<AllTrackApplicationView> AlltrackApp) : base(success, message)
        {
            AllTrackApplicationList = AlltrackApp;
        }

        private ProfileResponse(bool success, string message, SearchTrackApplicationView trackAppSearch) : base(success, message)
        {
            SearchTrackApplication = trackAppSearch;
        }

        private ProfileResponse(bool success, string message, UserCustomNotificationView customNotification) : base(success, message)
        {
            CustomNotification = customNotification;
        }

        private ProfileResponse(bool success, string message, List<UserCustomNotificationView> customNotificationList) : base(success, message)
        {
            CustomNotificationList = customNotificationList;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <param name="ex"></param>
        public ProfileResponse(string message, HttpStatusCode status, Exception ex) : base(false, message, status, ex)
		{ }


        public ProfileResponse(string message, HttpStatusCode status) : base(false, message, status)
        { }


        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profile">profile view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(ProfileView profile) : this(true, ClientMessageConstant.Success, profile)
		{ }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="QrCode"></param>
		public ProfileResponse(UseQRCode QrCode) : this(true, ClientMessageConstant.Success, QrCode)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profiles"></param>
        public ProfileResponse(IList<ProfileView> profiles) : this(true, ClientMessageConstant.Success, profiles)
		{ }
        public ProfileResponse(FavoriteProfile favoriteProfile) : this(true, ClientMessageConstant.Success, favoriteProfile)
        { }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="profiles"></param>
		public ProfileResponse(IList<MyProfileView> profiles) : this(true, ClientMessageConstant.Success, profiles)
        { }
        public ProfileResponse(SearchPublicProfileView searchPublicProfileView) : this(true, ClientMessageConstant.Success, searchPublicProfileView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="contactDetails">contactDetails view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(ContactDetailsView contactDetails) : this(true, ClientMessageConstant.Success, contactDetails)
        { }

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="profileName">profileName view model.</param>
		/// <returns>Response.</returns>
		public ProfileResponse(ProfileNameView profileName) : this(true, ClientMessageConstant.Success, profileName)
        { }

		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="personalInfo">personalInfo view model.</param>
		/// <returns>Response.</returns>
		public ProfileResponse(PersonalInfoView personalInfo) : this(true, ClientMessageConstant.Success, personalInfo)
        { }



		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="languageAndProficiencyView">languageAndProficiencyView view model.</param>
		/// <returns>Response.</returns>
		public ProfileResponse(LanguageAndProficiencyView languageAndProficiencyView) : this(true, ClientMessageConstant.Success, languageAndProficiencyView)
        { }


		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="countriesAndEmiratesView">countriesAndEmiratesView view model.</param>
		/// <returns>Response.</returns>
		public ProfileResponse(CountriesAndEmiratesView countriesAndEmiratesView) : this(true, ClientMessageConstant.Success, countriesAndEmiratesView)
        { }


		/// <summary>
		/// Creates a success response.
		/// </summary>
		/// <param name="languageItemView">languageItemView view model.</param>
		/// <returns>Response.</returns>
		public ProfileResponse(LanguageItemView languageItemView) : this(true, ClientMessageConstant.Success, languageItemView)
        { }
        public ProfileResponse(CVSavedResponseView cvProfileView) : this(true, ClientMessageConstant.Success, cvProfileView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="myProfileView">myProfileView view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(MyProfileView myProfileView) : this(true, ClientMessageConstant.Success, myProfileView)
        { }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="myProfileView">myProfileView view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(List<FileView> myProfileView) : this(true, ClientMessageConstant.Success, myProfileView)
        { }


        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileFollowingsView">myProfileView view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(ProfileFollowingsView profileFollowingsView) : this(true, ClientMessageConstant.Success, profileFollowingsView)
        { }


        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="profileFollowersView">myProfileView view model.</param>
        /// <returns>Response.</returns>
        public ProfileResponse(ProfileFollowersView profileFollowersView) : this(true, ClientMessageConstant.Success, profileFollowersView)
        { }

        public ProfileResponse(List<ProfileAlumniList> ProfileAlumniList) : this(true, ClientMessageConstant.Success, ProfileAlumniList)
        { }

        public ProfileResponse(List<TrackApplication> TrackApplication) : this(true, ClientMessageConstant.Success, TrackApplication)
        { }

        public ProfileResponse(List<AllTrackApplicationView> AllTrackApplication) : this(true, ClientMessageConstant.Success, AllTrackApplication)
        { }

        public ProfileResponse(SearchTrackApplicationView searchTrackAppView) : this(true, ClientMessageConstant.Success, searchTrackAppView)
        { }

        public ProfileResponse(UserCustomNotificationView CustomNotification) : this(true, ClientMessageConstant.Success, CustomNotification)
        { }

        public ProfileResponse(List<UserCustomNotificationView> CustomNotificationList) : this(true, ClientMessageConstant.Success, CustomNotificationList)
        { }
        public ProfileResponse() : base()
        { }

        public ProfileResponse(Exception e) : base(e)
        { }
    }
}
