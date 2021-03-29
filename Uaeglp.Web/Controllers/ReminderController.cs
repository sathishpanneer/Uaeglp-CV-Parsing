using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.ViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly IReminderService _reminderService;
        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost("set-reminder", Name = "SetReminder")]
        public async Task<IActionResult> SetReminder([FromBody] ReminderView view)
        {
            var result = await _reminderService.SetReminder(view);
            return Ok(result);
        }

        [HttpDelete("remove-reminder", Name = "RemoveReminder")]
        public async Task<IActionResult> RemoveReminder(int userId, int applicationId, int activityId)
        {
            var result = await _reminderService.RemoveReminder(userId, applicationId, activityId);
            return Ok(result);
        }

    }
}