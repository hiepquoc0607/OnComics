using Microsoft.AspNetCore.Mvc;
using OnComics.Library.Models.Request.Account;
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

        //[Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("api/accounts")]
        public async Task<IActionResult> GetAccountsAsync([FromQuery] GetAccReq getAccReq)
        {
            var result = await _accountService.GetAccountsAsync(getAccReq);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize]
        [HttpGet]
        [Route("api/accounts/{id}")]
        public async Task<IActionResult> GetAccountByIdAsync([FromRoute] int id)
        {
            var result = await _accountService.GetAccountByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize]
        [HttpPut]
        [Route("api/accounts/{id}")]
        public async Task<IActionResult> UpdateAccountAsync([FromRoute] int id, UpdateAccReq updateAccReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
            {
                return Forbid();
            }

            var result = await _accountService.UpdateAccountAsync(id, updateAccReq);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize]
        [HttpDelete]
        [Route("api/accounts/{id}")]
        public async Task<IActionResult> DeleteAccountAsync([FromRoute] int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
