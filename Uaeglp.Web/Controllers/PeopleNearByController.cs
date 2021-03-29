using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.ViewModels;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PeopleNearByController : ControllerBase
    {
        private readonly IPeopleNearByService _peopleNearBy;
        public PeopleNearByController(IPeopleNearByService peopleNearBy)
        {
            _peopleNearBy = peopleNearBy;
        }

        [HttpPost("addUpdateUserLocation", Name = "AddUpdateUserLocation")]
        public async Task<IActionResult> AddUpdateUserLocation([FromBody] UserLocationModel view)
        {
            var result = await _peopleNearBy.AddUpdateUserLocation(view);
            return Ok(result);
        }

        [HttpGet("getUserLocation/{userId}/{latitude}/{longitude}", Name = "GetUserLocation")]
        public async Task<IActionResult> GetUserLocation(int userId, string latitude, string longitude)
        {
            var data = await _peopleNearBy.GetUserLocation(userId, latitude, longitude);

            return Ok(data);
        }

        [HttpPut("hideUserLocation/{userId}/{isHideLocation}", Name = "HideUserLocation")]
        public async Task<IActionResult> HideUserLocation(int userId, bool isHideLocation)
        {
            var result = await _peopleNearBy.HideUserLocation(userId, isHideLocation);
            return Ok(result);
        }
    }
}