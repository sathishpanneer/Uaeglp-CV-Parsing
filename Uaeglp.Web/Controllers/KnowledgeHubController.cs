using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KnowledgeHubController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IKnowledgeHubService _service;

        public KnowledgeHubController(ILogger<ProfileController> logger, IKnowledgeHubService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("get-knowledgehubs", Name = "GetKnowledgeHubs")]
        public async Task<IActionResult> GetKnowledgeHubs(LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetKnowledgeHubs();
            return Ok(result);
        }

        [HttpGet("get-categories", Name = "GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _service.GetCategories();
            return Ok(result);
        }
        [HttpGet("get-allcourses/{profileid}", Name = "GetALLCoursesbyCategories")]
        public async Task<IActionResult> GetALLCoursesbyCategories(int profileid, int skip = 0, int take = 5)
        {
            var result = await _service.GetALLCoursesbyCategories(profileid, skip, take);
            return Ok(result);
        }
        [HttpGet("get-courses/{categoryId}/{profileid}", Name = "GetCourses")]
        public async Task<IActionResult> GetCourses(int categoryId, int profileid, int skip = 0, int take = 5)
        {
            var result = await _service.GetCoursesbyCategories(categoryId, profileid, skip,take);
            return Ok(result);
        }

        [HttpGet("get-knowledgehub/{id}", Name = "Getknowledgehub")]
        public async Task<IActionResult> Getknowledgehub(int id, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetDetalis(id, language);
            return Ok(result);
        }

        [HttpGet("get-knowledgehub-courses/{searchText}/{profileid}", Name = "GetKnowledgeHubCourses")]
        public async Task<IActionResult> GetKnowledgeHubCourses(string searchText, int profileid, int? providerTypeID, int? categoryID, bool isProfile, LanguageType language = LanguageType.AR, int skip = 0, int take = 5)
        {
            var result = await _service.GetKnowledgeHubCourses(searchText, profileid, providerTypeID, categoryID, isProfile, language, skip, take);
            return Ok(result);
        }

        [HttpGet("get-course-iframe/{categoryId}", Name = "GetCourseIframe")]
        public async Task<IActionResult> GetCourseIframe(int courseId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetCourseIframe(courseId, language);
            return Ok(result);
        }
        [HttpPost("set-favourite/{courseid}/{profileid}", Name = "SetFavourite")]
        public async Task<IActionResult> SetFavourite(int courseid, int profileid)
        {
            var result = await _service.SetFavourite(courseid, profileid);
            return Ok(result);
        }
        [HttpPost("set-unfavourite/{courseid}/{profileid}", Name = "SetUnFavourite")]
        public async Task<IActionResult> SetUnFavourite(int courseid, int profileid)
        {
            var result = await _service.SetUnFavourite(courseid, profileid);
            return Ok(result);
        }
        [HttpGet("get-favouritecourse/{profileid}", Name = "GetFavouriteCourses")]
        public async Task<IActionResult> GetFavouriteCourses(int profileid, int skip = 0, int take = 5)
        {
            var result = await _service.GetFavouriteCourses(profileid, skip, take);
            return Ok(result);
        }

        [HttpGet("get-libraryfolders/{profileid}", Name = "GetFolders")]
        public async Task<IActionResult> GetFolders(int profileid)
        {
            var result = await _service.GetFolders( profileid);
            return Ok(result);
        }
        [HttpGet("get-folderfiles/{profileid}/{parentfolderid}", Name = "GetFolderandfiles")]
        public async Task<IActionResult> GetFolderandfiles(int profileid, int parentfolderid)
        {
            var result = await _service.GetFolderandfiles(profileid, parentfolderid);
            return Ok(result);
        }
    }
}