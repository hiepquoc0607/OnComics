using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Library.Models.Request.Auth;
using OnComics.Service.Interface;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //Login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginReq loginReq)
        {
            var result = await _authService.LoginAsync(loginReq);

            return StatusCode(result.StatusCode, result);
        }

        //Register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterReq registerReq)
        {
            var result = await _authService.RegisterAsync(registerReq);

            return StatusCode(result.StatusCode, result);
        }

        //Refresh Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenReq refreshTokenReq)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenReq);

            return StatusCode(result.StatusCode, result);
        }

        //Request Reset Password
        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPasswordAsync([FromBody] EmailReq emailReq)
        {
            var result = await _authService.RequestResetPasswordAsync(emailReq.Email);

            return StatusCode(result.StatusCode, result);
        }

        //Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(
            [FromQuery] InfoQuery infoQuery,
            [FromQuery] ResetPassReq resetPassReq)
        {
            var result = await _authService.ResetPasswordAsync(infoQuery, resetPassReq);

            return StatusCode(result.StatusCode, result);
        }

        //Request Confirm Email
        [Authorize(Policy = "User")]
        [HttpPost("{id}/request-confirm-email")]
        public async Task<IActionResult> RequestConfirmEmailAsync([FromRoute] int id)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString())) return Forbid();

            var result = await _authService.RequestConfirmEmailAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Confirm Email
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] InfoQuery infoQuery)
        {
            var result = await _authService.ConfirmEmailAsync(infoQuery);

            return StatusCode(result.StatusCode, result);
        }
    }
}