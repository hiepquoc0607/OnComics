using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [ApiController]
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
        [Route("api/account")]
        public async Task<IActionResult> GetAccountsAsync([FromQuery] GetAccountReq getAccReq)
        {
            var result = await _accountService.GetAccountsAsync(getAccReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Account By Id
        [Authorize(Policy = "User")]
        [HttpGet]
        [Route("api/account/{id}")]
        public async Task<IActionResult> GetAccountByIdAsync([FromRoute] int id)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || userRoleClaim == null ||
                (!userIdClaim.Equals(id.ToString()) && !userRoleClaim.Equals(RoleConstant.ADMIN)))
                return Forbid();

            var result = await _accountService.GetAccountByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Update Account
        [Authorize(Policy = "User")]
        [HttpPut]
        [Route("api/account/{id}")]
        public async Task<IActionResult> UpdateAccountAsync([FromRoute] int id, [FromBody] UpdateAccountReq updateAccReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString())) return Forbid();

            var result = await _accountService.UpdateAccountAsync(id, updateAccReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Password
        [Authorize(Policy = "User")]
        [HttpPatch]
        [Route("api/account/{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromRoute] int id, [FromBody] UpdatePasswordReq updatePasswordReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString())) return Forbid();

            var result = await _accountService.UpdatePasswordAsync(id, updatePasswordReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Account Status
        [Authorize(Policy = "Admin")]
        [HttpPatch]
        [Route("api/account/{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromQuery] UpdateStatusReq<AccountStatus> updateStatusReq)
        {

            var result = await _accountService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Account
        [Authorize(Policy = "User")]
        [HttpDelete]
        [Route("api/account/{id}")]
        public async Task<IActionResult> DeleteAccountAsync([FromRoute] int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}