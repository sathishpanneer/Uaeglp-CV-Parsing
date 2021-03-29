using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Uaeglp.Web.Extensions;
using Uaeglp.ViewModels;
namespace Uaeglp.Web.Controllers
{
	//[Authorize]
	[ApiVersion("1.0")]
	[Route("/api/[controller]")]
	public class AccountController : Controller
    {
		//Check Checkin
		private readonly ILogger<AccountController> _logger;
		private readonly Contracts.IAccountService _service;

        public AccountController(ILogger<AccountController> logger, Contracts.IAccountService service)
		{
			_logger = logger;
			_service = service;
        }

		[HttpPost("user-registration", Name = "Signup")]
		public async Task<IActionResult> SignupAsync([FromBody] UserRegistration view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.SignupAsync(view);
		
			return Ok(result.User);
		}

		[Authorize]
		[HttpPost("reset-password", Name = "ResetPassword")]
		public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPassword view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.ResetPassword(view);

			return Ok(result.User);
		}

		[HttpPost("new-password", Name = "SetNewPassword")]
		public async Task<IActionResult> SetNewPasswordAsync([FromBody] SetNewPassword view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.SetNewPassword(view);

			return Ok(result.User);
		}

		[HttpPost("validate-otp", Name = "ValidateOtp")]
		public async Task<IActionResult> ValidateOtpAsync([FromBody] ValidateOtp view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.ValidateOtp(view);

            return Ok(result.Resource);
		}

		[HttpPost("resend-otp", Name = "resendOtp")]
		public async Task<IActionResult> ResendOtpAsync([FromBody] ResendOTP view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.ResendOtp(view);

            return Ok(result.Resource);
		}

		[HttpPost("forgot-pass", Name = "ForgotPass")]
		public async Task<IActionResult> ForgotPassAsync([FromBody] ForgotPassword view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.ForgotPass(view);

			return Ok(result.User);
		}

		[HttpPost("forgot-email", Name = "ForgotEmailID")]
		public async Task<IActionResult> ForgotEmailAsync([FromBody] ForgotEmail view)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState.GetErrorMessages());

			var result = await _service.ForgotEmailID(view);

            return Ok(result.User);
		}

		[Authorize]
        [HttpGet("user-details/{userId}", Name = "GetUserDetails")]
        public async Task<IActionResult> GetUserDetailsAsync(int userId)
        {
            var result = await _service.GetUserDetailsAsync(userId);
            return Ok(result.Resource);
        }

        [HttpPost("addorupdate-diviceinfo", Name = "AddOrUpdateDeviceInfoAsync")]
        private async Task<IActionResult> AddOrUpdateDeviceInfoAsync([FromBody] UserDeviceInfoViewModel model)
        {
            var result = await _service.AddOrUpdateDeviceInfoAsync(model);
            return Ok(result.DiviceInfoViewModel);
        }

		//[Authorize]
  //      [HttpGet("Test", Name = "Test")]
  //      public IActionResult Test()
  //      {
            
  //          return Ok("Working");
  //      }


		[HttpPost("Login", Name = "Login")]
		public async Task<IActionResult> LoginAsync([FromBody]AuthenticateViewModel model)
        {

           var data = await _service.LoginAsync(model);

			return Ok(data);
		}

		[Authorize]
		[HttpPost("Logout/{userId}/{deviceId}", Name = "LogOut")]
        public async Task<IActionResult> LogOutAsync(int userId, string deviceId)
        {

            var data = await _service.LogOutAsync(userId, deviceId);

            return Ok(data);
        }

	}
}