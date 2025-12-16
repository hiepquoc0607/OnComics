using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/accounts")]
    [ApiController]
    [EnableRateLimiting("BasePolicy")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        //Get Accounts
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAccountReq getAccReq)
        {
            var result = await _accountService.GetAccountsAsync(getAccReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Account By Id
        [Authorize]
        [HttpGet("{id:guid}")]

        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? userRoleClaim = HttpContext.User
                .FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || userRoleClaim == null ||
                (!userIdClaim.Equals(id.ToString()) &&
                !userRoleClaim.Equals(RoleConstant.ADMIN)))
                return Forbid();

            var result = await _accountService.GetAccountByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Update Account
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateAccountReq updateAccReq)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
                return Forbid();

            var result = await _accountService.UpdateAccountAsync(id, updateAccReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Profile Picture
        [Authorize]
        [RequestSizeLimit(2 * 1024 * 1024)] //Limit File To Max 2 MB
        [HttpPatch("{id:guid}/profile-picture")]
        public async Task<IActionResult> UpdateProfileImageAsync(
            [FromRoute] Guid id,
            IFormFile file)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
                return Forbid();

            var result = await _accountService.UpdateProfileImageAsync(id, file);

            return StatusCode(result.StatusCode, result);
        }

        //Update Password
        [Authorize]
        [HttpPatch("{id:guid}/password")]
        public async Task<IActionResult> UpdatePasswordAsync(
            [FromRoute] Guid id,
            [FromBody] UpdatePasswordReq updatePasswordReq)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
                return Forbid();

            var result = await _accountService.UpdatePasswordAsync(id, updatePasswordReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Account Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Guid id,
            [FromQuery] UpdateStatusReq<AccountStatus> updateStatusReq)
        {

            var result = await _accountService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Account
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? userRoleClaim = HttpContext.User
                .FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || userRoleClaim == null ||
                (!userIdClaim.Equals(id.ToString()) &&
                !userRoleClaim.Equals(RoleConstant.ADMIN)))
                return Forbid();

            var result = await _accountService.DeleteAccountAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}