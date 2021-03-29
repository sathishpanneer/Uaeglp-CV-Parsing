using System.Collections.Generic;
using Uaeglp.Contract.Communication;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts.Communication
{
    public interface IProfileResponse : IBaseResponse
    {
       ProfileView Profile { get; set; }
	   IList<ProfileView> Profiles { get; set; }
       ContactDetailsView ContactDetailsView { get; set; }
       ProfileNameView ProfileNameView { get; set; }
       PersonalInfoView PersonalInfoView { get; set; }
       LanguageAndProficiencyView LanguageAndProficiencyView { get; set; }
       CountriesAndEmiratesView CountriesAndEmiratesView { get; set; }
       LanguageItemView LanguageItemView { get; set; }
        CVSavedResponseView CVProfileView { get; set; }
        MyProfileView MyProfileView { get; set; }
       List<FileView> DocumentsInfoList { get; set; }
       ProfileFollowingsView ProfileFollowingsView { get; set; }
       ProfileFollowersView ProfileFollowersView { get; set; }
       List<TrackApplication> TrackApplication { get; set; }
       List<AllTrackApplicationView> AllTrackApplicationList { get; set; }
        UserCustomNotificationView CustomNotification { get; set; }
       List<UserCustomNotificationView> CustomNotificationList { get; set; }
       UseQRCode UserQRCode { get; set; }

    }
}
