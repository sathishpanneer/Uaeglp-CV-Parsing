using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Uaeglp.Contracts;
using Uaeglp.Models;
using Uaeglp.ViewModels;
using Uaeglp.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Uaeglp.Web.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RecommendLeaderController : Controller
    {
        private readonly IRecommendLeaderService _recommendLeaderService;   
        public RecommendLeaderController(IRecommendLeaderService recommendLeaderService)
        {
            _recommendLeaderService = recommendLeaderService;
        }

        [HttpPost("recommendSubmission", Name = "AddRecommendLeader")]
        public async Task<IActionResult> AddRecommendLeader([FromForm] RecommendLeaderView view)
        {
            var result = await _recommendLeaderService.AddRecommendLeader(view);
            return Ok(result);
        }

        [HttpGet("getRecommendFitList", Name = "GetRecommendFitList")]
        public async Task<IActionResult> GetRecommendFitListAsync()
        {
            var data = await _recommendLeaderService.GetRecommendFitListAsync();

            return Ok(data);
        }

        [HttpPost("requestCallback", Name = "RequestCallback")]
        public async Task<IActionResult> RequestCallbackAsync([FromBody] RecommendationCallbackView view)
        {
            var data = await _recommendLeaderService.RequestCallbackAsync(view);

            return Ok(data);
        }

        [HttpGet("getRecommendLeaderDetails/{recommendId}", Name = "GetRecommendLeaderDetails")]
        public async Task<IActionResult> getRecommendLeaderDetailsAsync(int recommendId)
        {
            var data = await _recommendLeaderService.getRecommendLeaderDetailsAsync(recommendId);

            return Ok(data);
        }

        [HttpGet("getViewMatchProfile", Name = "GetViewMatchProfile")]
        public async Task<IActionResult> GetViewMatchProfile(int recommendId)
        {
            var data = await _recommendLeaderService.GetViewMatchProfile(recommendId);

            return Ok(data);
        }

        [HttpGet("getAllRecommendLeaderList/{skip}/{limit}", Name = "GetAllRecommendLeaderList")]
        public async Task<IActionResult> GetAllRecommendLeaderListAsync(int skip, int limit)
        {
            var data = await _recommendLeaderService.GetAllRecommendLeaderListAsync(skip,limit);

            return Ok(data);
        }

    }
       
}