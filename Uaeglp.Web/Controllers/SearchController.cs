using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SocialController> _socialLogger;
        private readonly ILogger<ProfileController> _profileLogger;
        private readonly Contracts.ISocialService _socialService;
        private readonly Contracts.IProfileService _profileService;

        public SearchController(Contracts.ISocialService socialservice, Contracts.IProfileService profileService,
            ILogger<SocialController> socialLogger, ILogger<ProfileController> profileLogger)
        {
            _socialLogger = socialLogger;
            _socialLogger = socialLogger;
            _socialService = socialservice;
            _profileService = profileService;
        }

        [HttpGet("search-posts", Name = "SearchPosts")]
        public async Task<IActionResult> SearchPosts([FromQuery] string text, int userId, int skip = 0, [FromQuery] int limit = 5)
        {
            var result = await _socialService.SearchPostsAsync(userId,text, skip, limit);

            return Ok(result);
        }

        [HttpGet("search-profiles", Name = "SearchPublicProfile")]
        public async Task<IActionResult> SearchPublicProfiles([FromQuery] string text, int skip = 0, [FromQuery] int limit = 5)
        {
            var result = await _profileService.SearchPublicProfilesAsync(text, skip, limit);
            return Ok(result);
        }
    }
}