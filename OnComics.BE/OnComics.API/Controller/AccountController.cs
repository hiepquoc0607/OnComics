using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Utils.Constants;
using OnComics.Service.Interface;
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
        public async Task<IActionResult> UpdateAccountAsync([FromRoute] int id, UpdateAccountReq updateAccReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString())) return Forbid();

            var result = await _accountService.UpdateAccountAsync(id, updateAccReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Password
        [Authorize(Policy = "User")]
        [HttpPut]
        [Route("api/account/{id}/password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromRoute] int id, UpdatePasswordReq updatePasswordReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString())) return Forbid();

            var result = await _accountService.UpdatePasswordAsync(id, updatePasswordReq);

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