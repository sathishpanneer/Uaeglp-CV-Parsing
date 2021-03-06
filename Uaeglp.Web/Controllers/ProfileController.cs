using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Web.Extensions;
using Uaeglp.ViewModels;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("/api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : Controller
    {
		//Check Checkin
		private readonly ILogger<ProfileController> _logger;
		private readonly Contracts.IProfileService _service;

		public ProfileController(ILogger<ProfileController> logger, Contracts.IProfileService service)
		{
			_logger = logger;
			_service = service;
		}


        [HttpGet("get-myprofile/{userId}", Name = "GetMyProfile")]
        public async Task<IActionResult> GetMyProfileAsync(int userId)
        {

            var result = await _service.GetMyProfileAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-user-recommendation/{userId}", Name = "GetRecommendedPeople")]
        public async Task<IActionResult> GetRecommendedPeople(int userId)
        {

            var result = await _service.GetRecommendedPeople(userId);
            return Ok(result);
        }


        [HttpGet("get-public-profile/{userId}/{publicProfileId}", Name = "GetPublicProfile")]
        public async Task<IActionResult> GetPublicProfileAsync(int userId,int publicProfileId)
        {
            var result = await _service.GetPublicProfileAsync(userId,publicProfileId);
            return Ok(result);
        }


        [HttpGet("get-languageandproficiency", Name = "GetLanguagesAndProficiency")]
        public async Task<IActionResult> GetLanguagesAndProficiencyAsync()
        {
            var result = await _service.GetLanguagesAndProficiencyAsync();
            return Ok(result);
        }


        [HttpGet("get-all-uploaded-documents/{userId}", Name = "GetAllUploadedDocuments")]
        public async Task<IActionResult> GetAllUploadedDocumentsAsync(int userId)
        {
            var result = await _service.GetAllUploadedDocumentsAsync(userId);

            return Ok(result);
        }

        [HttpGet("get-countriesandemirates", Name = "GetCountriesAndEmirates")]
        public async Task<IActionResult> GetCountriesAndEmiratesAsync()
        {
            var result = await _service.GetCountriesAndEmiratesAsync();
            return Ok(result);
        }

        [HttpGet("get-personalInfo/{userId}", Name = "GetPersonalInfo")]
		public async Task<IActionResult> GetPersonalInfoAsync(int userId)
        {
            var result = await _service.GetPersonalInfoAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-profileName/{userId}", Name = "GetProfileName")]
        public async Task<IActionResult> GetProfileNameAsync(int userId)
        {
            var result = await _service.GetProfileNameAsync(userId);
            return Ok(result);
        }

		[HttpGet("get-favorite-profiles/{userId}/{skip}/{limit}", Name = "GetFavoriteProfilesByUser")]
		public async Task<IActionResult> GetUserFavoritePostAsync(int userId, int skip = 0, int limit = 5)
		{
			var result = await _service.GetUserFavoriteProfilesAsync(userId, skip, limit);
			if (!result.Success)
				return BadRequest(result);

			return Ok(result);
		}

        [HttpPost("post-favorite-profiles/{userId}/{profileId}", Name = "AddFavoriteProfile")]
        public async Task<IActionResult> AddUserFavoriteProfileAsync(int userId, int profileId)
        {
            var result = await _service.AddUserFavoriteProfileAsync(userId, profileId);

            return Ok(result);
        }

        [HttpGet("get-contactdetails/{userId}", Name = "GetContactDetails")]
        public async Task<IActionResult> GetContactDetailsAsync(int userId)
        {
            var result = await _service.GetContactDetailsAsync(userId);
            return Ok(result);
        }

        [HttpGet("get-profile-followings/{userId}/{take}/{page}", Name = "GetFollowings")]
        public async Task<IActionResult> GetFollowingListAsync(int userId,int take,int page, [FromQuery] string search)
        {
            var result = await _service.GetFollowingListAsync(userId,take,page,search);
            result.ProfileFollowingsView?.FollowingsList?.ForEach(k=>
            {
                k.IsAmFollowing = true;
            });

            return Ok(result);
        }

        [HttpGet("get-profile-followers/{userId}/{take}/{page}", Name = "GetFollowers")]
        public async Task<IActionResult> GetFollowersListAsync(int userId, int take, int page, [FromQuery] string search)
        {
            var result = await _service.GetFollowersListAsync(userId, take, page, search);

            return Ok(result);
        }

        //[HttpPost("add-user-follower/{userId}/{followerId}", Name = "AddUserFollowerAsync")]
        //public async Task<IActionResult> AddUserFollowerAsync(int userId, int followerId)
        //{
        //    var result = await _service.AddUserFollowerAsync(userId, followerId);
        //    return Ok(result);
        //}

        //[HttpDelete("delete-user-follower/{userId}/{followerId}", Name = "DeleteUserFollowerAsync")]
        //public async Task<IActionResult> DeleteUserFollowerAsync(int userId, int followerId)
        //{
        //    var result = await _service.DeleteUserFollowerAsync(userId, followerId);
        //    return Ok(result);
        //}

        [HttpPost("add-user-following/{userId}/{followerId}", Name = "AddUserFollowingAsync")]
        public async Task<IActionResult> AddUserFollowingAsync(int userId, int followerId)
        {
            var result = await _service.AddUserFollowingAsync(userId, followerId);
            return Ok(result);
        }

        [HttpDelete("delete-user-following/{userId}/{followerId}", Name = "DeleteUserFollowingAsync")]
        public async Task<IActionResult> DeleteUserFollowingAsync(int userId, int followerId)
        {
            var result = await _service.DeleteUserFollowingAsync(userId, followerId);
            return Ok(result);
        }

        [HttpPost("post-updatecontactdetails", Name = "UpdateContactDetails")]
        public async Task<IActionResult> UpdateContactDetailsAsync([FromBody]ContactDetailsView model)
        {
            var result = await _service.UpdateContactDetailsAsync(model);
            return Ok(result);
        }

        [HttpPost("post-updateprofilename", Name = "UpdateProfileName")]
        public async Task<IActionResult> UpdateProfileNameAsync([FromBody]ProfileNameView model)
        {
            var result = await _service.UpdateProfileNameAsync(model);
            return Ok(result);
        }

        [HttpPost("post-updatepersonalinfo", Name = "UpdatePersonalInfo")]
        public async Task<IActionResult> UpdatePersonalInfoAsync([FromBody]PersonalInfoView model)
        {
            var result = await _service.UpdatePersonalInfoAsync(model);
            return Ok(result);
        }

        [HttpPost("post-add-or-update-language", Name = "AddOrUpdateLanguage")]
        public async Task<IActionResult> AddOrUpdateKnownLanguageAsync([FromBody]LanguageItemView model)
        {
            var result = await _service.AddOrUpdateKnownLanguageAsync(model);
            return Ok(result);
        }

        [HttpDelete("post-delete-language/{profileId}/{id}", Name = "DeleteLanguage")]
        public async Task<IActionResult> DeleteKnownLanguageAsync(int profileId, int id)
        {
            var result = await _service.DeleteKnownLanguageAsync(profileId,id);
            return Ok(result);
        }


        [HttpPost("add-endorsement-count", Name = "AddEndorsementCount")]
        public async Task<IActionResult> AddEndorsementCountAsync([FromBody]EndorsementView model)
        {
            var result = await _service.AddEndorsementCountAsync(model);
            return Ok(result);
        }

        [HttpGet("get-profile-alumni/{userId}", Name = "GetAlumni")]
        public async Task<IActionResult> GetAlumniAysnc(int userId)
        {
            var result = await _service.GetAlumniAysnc(userId);

            return Ok(result);
        }

        [HttpGet("all-track-application-list/{userId}", Name = "GetAllTrackApplication")]
        public async Task<IActionResult> GetAllTrackApplicationAsync(int userId)
        {
            var result = await _service.GetAllTrackApplicationAsync(userId);

            return Ok(result);
        }

        [HttpGet("track-application/{userId}/{applicationTypeId}/{skip}/{limit}", Name = "GetTrackApplication")]
        public async Task<IActionResult> GetTrackApplicationAsync(int userId, int applicationTypeId, int skip, int limit)
        {
            var result = await _service.GetTrackApplicationAsync(userId, applicationTypeId, skip, limit);

            return Ok(result);
        }

        [HttpGet("search-track-application/{userId}/{applicationTypeId}/{text}", Name = "GetSearchTrackApplication")]
        public async Task<IActionResult> SearchTrackApplicationAsync(int userId, int applicationTypeId, string text)
        {
            var result = await _service.SearchTrackApplicationAsync(userId, applicationTypeId, text);

            return Ok(result);
        }

        [HttpPost("custom-notification", Name = "CustomNotification")]
        public async Task<IActionResult> UserCustomNotificationAsync([FromBody] UserCustomNotificationView view)
        {
            var result = await _service.UserCustomNotificationAsync(view);

            return Ok(result);
        }

        [HttpGet("get-custom-notification/{userId}", Name = "GetCustomNotification")]
        public async Task<IActionResult> GetCustomNotificationAsync(int userId)
        {
            var result = await _service.GetCustomNotificationAsync(userId);

            return Ok(result);
        }

        [HttpPut("get-User-qrcode/{userId}", Name = "GetUserQrCode")]
        public async Task<IActionResult> GenerateUserQRCodeAsync(int userId)
        {
            var result = await _service.GenerateUserQRCodeAsync(userId);

            return Ok(result);
        }

        [HttpPost("cv-parsing", Name = "CVParsing")]
        public async Task<IActionResult> CVParsingAsync([FromForm] CVParsingView view)
        {
            var result = await _service.CVParsingAsync(view);

            return Ok(result);
        }

        [HttpPost("save-cv-parsing", Name = "SaveCVDetails")]
        public async Task<IActionResult> SaveCVParsedDetailsAsync([FromBody] CVSaveProfileView model)
        {
            var result = await _service.SaveCVParsedDetailsAsync(model);
            return Ok(result);
        }
    }
}