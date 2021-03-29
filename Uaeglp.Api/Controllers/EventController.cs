using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.Event;

namespace Uaeglp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IEventService _service;

        public EventController(ILogger<ProfileController> logger, IEventService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("get-event/{eventId}/{crunchDays}/{language}", Name = "GetEvent")]
        public async Task<IActionResult> GetEvent(int eventId, bool crunchDays = false, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetEvent(eventId, crunchDays, language);
            return Ok(result);
        }

        [HttpGet("get-eventdays-by-batch/{batchId}/{language}", Name = "GetEventDays")]
        public async Task<IActionResult> GetEventDays(int batchId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetEventDays(batchId, language);
            return Ok(result);
        }

        [HttpGet("get-events/{userId}/{language}/{forProfile}/{ispublic}", Name = "GetEvents")]
        public async Task<IActionResult> GetEvents(int userId, LanguageType language = LanguageType.AR, bool forProfile = false, bool ispublic = false)
        {
            var result = await _service.GetEvents(userId, language, forProfile, ispublic);
            return Ok(result);
        }

        [HttpGet("get-events-by-batches/{userId}/{language}", Name = "GetEventsByBatches")]
        public async Task<IActionResult> GetEventsByBatches(int userId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetEventsByBatches(userId, language);
            return Ok(result);
        }

        [HttpGet("get-events-for-calender/{userId}/{language}", Name = "GetEventsForCalender")]
        public async Task<IActionResult> GetEventsForCalender(int userId, LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetEventsForCalender(userId, language);
            return Ok(result);
        }

        [HttpGet("get-events-attending/{language}", Name = "GetAttendingEvents")]
        public async Task<IActionResult> GetAttendingEvents(LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetAttendingEvents(language);
            return Ok(result);
        }

        [HttpGet("get-events-not-attending/{language}", Name = "GetNotAttendingEvents")]
        public async Task<IActionResult> GetNotAttendingEvents(LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetNotAttendingEvents(language);
            return Ok(result);
        }

        [HttpGet("get-events-maybe-attending/{language}", Name = "GetMaBeEvents")]
        public async Task<IActionResult> GetMaBeEvents(LanguageType language = LanguageType.AR)
        {
            var result = await _service.GetMaBeEvents(language);
            return Ok(result);
        }

        [HttpGet("get-meeting-request/{eventId}", Name = "GetMeetingRequest")]
        public async Task<IActionResult> GetMeetingRequest(int eventId)
        {
            var result = await _service.GetMeetingRequest(eventId);
            return Ok(result);
        }

        [HttpGet("get-user-decision/{userId}/{eventId}", Name = "GetUserDecision")]
        public async Task<IActionResult> GetUserDecision(int userId, int eventId)
        {
            var result = await _service.GetUserDecision(userId, eventId);
            return Ok(result);
        }

        [HttpPut("update-event", Name = "UpdateEvent")]
        public async Task<IActionResult> UpdateEvent(EventView eventView)
        {
            var result = await _service.UpdateEvent(eventView);
            return Ok(result);
        }

        [HttpPut("update-user-decision/{userId}/{eventId}/{decisionId}", Name = "SetEventDesicion")]
        public async Task<IActionResult> SetEventDesicion(int userId, int eventId, int decisionId)
        {
            var result = await _service.SetEventDesicion(userId, decisionId, eventId);
            return Ok(result);
        }
    }
}