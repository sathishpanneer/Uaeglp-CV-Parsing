using System.Collections.Generic;
using System.Threading.Tasks;
using Uaeglp.Contracts.Communication;
using Uaeglp.Models.ProfileModels;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Contracts
{
    public interface IProfileService
	{

		Task<IProfileResponse> UpdateProfileFollowersCountAsync(int profileId, bool increment);
		Task<IProfileResponse> UpdateProfileFollowingCountAsync(int profileId, bool increment);
        Task<IProfileResponse> GetPersonalInfoAsync(int userId);
        Task<IProfileResponse> GetProfileNameAsync(int userId);
		Task<IProfileResponse> GetUserFavoriteProfilesAsync(int userId, int skip = 0, int limit = 5);
        Task<IProfileResponse> AddUserFavoriteProfileAsync(int userId, int profileId);

        Task<IProfileResponse> GetContactDetailsAsync(int userId);
        Task<IProfileResponse> UpdateContactDetailsAsync(ContactDetailsView model);
        Task<IProfileResponse> UpdatePersonalInfoAsync(PersonalInfoView model);
        Task<IProfileResponse> UpdateProfileNameAsync(ProfileNameView model);
        Task<IProfileResponse> AddOrUpdateKnownLanguageAsync(LanguageItemView model);
        Task<IProfileResponse> DeleteKnownLanguageAsync(int profileId, int id);
        Task<IProfileResponse> GetLanguagesAndProficiencyAsync();
        Task<IProfileResponse> GetCountriesAndEmiratesAsync();
        Task<IProfileResponse> GetMyProfileAsync(int userId);

        Task<IProfileResponse> GetPublicProfileAsync(int userId, int publicProfileId);

        Task<IProfileResponse> GetAllUploadedDocumentsAsync(int userId);

        Task<IProfileResponse> GetFollowingListAsync(int userId, int take, int page, string search, LanguageType languageType = LanguageType.EN);
        Task<IProfileResponse> GetFollowersListAsync(int userId, int take, int page, string search, LanguageType languageType = LanguageType.EN);

        //Task<IProfileResponse> AddUserFollowerAsync(int userId, int followerId);

        //Task<IProfileResponse> DeleteUserFollowerAsync(int userId, int followerId);

        Task<IProfileResponse> AddUserFollowingAsync(int userId, int followingId);
        Task<IProfileResponse> DeleteUserFollowingAsync(int userId, int followingId);

        Task<IProfileResponse> AddEndorsementCountAsync(EndorsementView model);

        Task<IProfileResponse> GetRecommendedPeople(int userId);

        Task<IProfileResponse> SearchPublicProfilesAsync(string text, int skip = 0, int limit = 5);

        Task<List<ProfileAlumniList>> GetAlumniAysnc(int userId);
        Task<IProfileResponse> GetAllTrackApplicationAsync(int userId, string text = "");
        Task<IProfileResponse> GetTrackApplicationAsync(int userId, int applicationTypeId, int skip, int limit);
        Task<IProfileResponse> SearchTrackApplicationAsync(int userId, int applicationTypeId, string text);
        Task<IProfileResponse> UserCustomNotificationAsync(UserCustomNotificationView view);
        Task<IProfileResponse> GetCustomNotificationAsync(int userId);
        Task<IProfileResponse> GenerateUserQRCodeAsync(int userId);
        Task<CVParsedDataView> CVParsingAsync(CVParsingView view);
        Task<IProfileResponse> SaveCVParsedDetailsAsync(CVSaveProfileView view);

    }
}
