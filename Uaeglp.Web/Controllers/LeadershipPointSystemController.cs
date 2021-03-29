using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.ViewModels.Enums;
using Uaeglp.ViewModels.ProfileViewModels;

namespace Uaeglp.Web.Controllers
{
	[ApiVersion("1.0")]
	[Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeadershipPointSystemController : ControllerBase
    {
        private readonly ILeadershipPointSystemService _leadershipPointSystemService;
        public LeadershipPointSystemController(ILeadershipPointSystemService systemService)
        {
            _leadershipPointSystemService = systemService;
        }

        [HttpGet("leadership-point-system/{profileId}", Name = "LeadershipPointSystem")]
        public async Task<IActionResult> GetLeadershipPointSystemAsync(int profileId)
        {
            var result = await _leadershipPointSystemService.GetLeadershipPointSystemAsync(profileId);
            return Ok(result);
        }

        [HttpGet("criteria-claimed-points/{profileId}/{criteriaId}", Name = "CriteriaClaimedPoints")]
        public async Task<IActionResult> GetCriteriaClaimedPointsAsync(int profileId, int criteriaId)
        {
            var result = await _leadershipPointSystemService.GetCriteriaClaimedPointsAsync(profileId, criteriaId);
            return Ok(result);
        }

        [HttpGet("criteria-more-details/{correlationId}", Name = "CriteriaMoreDetails")]
        public async Task<IActionResult> GetCriteriaMoreDetailsAsync(string correlationId)
        {
            var result = await _leadershipPointSystemService.GetCriteriaMoreDetailsAsync(new Guid(correlationId));
            return Ok(result);
        }

        [HttpPost("add-claim", Name = "ClaimCriteria")]
        public async Task<IActionResult> AddCriteriaClaimAsync([FromForm]CriteriaClaimRequestView  claimRequest)
        {
            var result = await _leadershipPointSystemService.AddCriteriaClaimAsync(claimRequest);
            return Ok(result);
        }



    }
}