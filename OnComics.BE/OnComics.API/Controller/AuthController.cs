using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IGoogleService _googleService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(
            IAuthService authService,
            IGoogleService googleService,
            IHttpClientFactory httpClientFactory)
        {
            _authService = authService;
            _googleService = googleService;
            _httpClientFactory = httpClientFactory;
        }

        //Login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginReq loginReq)
        {
            var result = await _authService.LoginAsync(loginReq);

            return StatusCode(result.StatusCode, result);
        }

        //Login By Google
        [HttpGet("google")]
        public IActionResult GoogleLoginAsync()
        {
            try
            {
                var result = _googleService.CreateLoginLinkAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Google Callback
        [HttpPost("google-callback")]
        public async Task<IActionResult> GoogleCallbackAsync([FromQuery] string code)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var result = await _authService.GoogleCallbackAsync(code, httpClient);

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
        [Authorize]
        [HttpPost("{id}/request-confirm-email")]
        public async Task<IActionResult> RequestConfirmEmailAsync([FromRoute] int id)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
                return Forbid();

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