using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnComics.Library.Models.Request.Auth;
using OnComics.Service.Interface;

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

            if (result.StatusCode != 200) return StatusCode(result.StatusCode, result);

            return Ok(result);
        }

        //Register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterReq registerReq)
        {
            var result = await _authService.RegisterAsync(registerReq);

            if (result.StatusCode != 200) return StatusCode(result.StatusCode, result);

            return Ok(result);
        }

        //Refresh Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenReq refreshTokenReq)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenReq);

            if (result.StatusCode != 200) return StatusCode(result.StatusCode, result);

            return Ok(result);
        }
    }
}
