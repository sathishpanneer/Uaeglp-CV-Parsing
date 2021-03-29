using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;

namespace Uaeglp.Api.Controllers
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

        [HttpGet("get-courses", Name = "GetCourses")]
        public async Task<IActionResult> GetCourses(LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetCoursesRandomly(language);
            return Ok(result);
        }

        [HttpGet("get-knowledgehub/{id}", Name = "Getknowledgehub")]
        public async Task<IActionResult> Getknowledgehub(int id, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetDetalis(id, language);
            return Ok(result);
        }

        [HttpGet("get-knowledgehub-courses/{searchText}", Name = "GetKnowledgeHubCourses")]
        public async Task<IActionResult> GetKnowledgeHubCourses(string searchText, int? providerTypeID, int? categoryID, bool isProfile, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetKnowledgeHubCourses(searchText, providerTypeID, categoryID, isProfile, language);
            return Ok(result);
        }

        [HttpGet("get-course-iframe/{categoryId}", Name = "GetCourseIframe")]
        public async Task<IActionResult> GetCourseIframe(int courseId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetCourseIframe(courseId, language);
            return Ok(result);
        }
    }
}