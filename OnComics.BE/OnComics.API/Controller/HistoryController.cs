using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using OnComics.Application.Constants;
using OnComics.Application.Enums.History;
using OnComics.Application.Models.Request.History;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/history")]
    [ApiController]
    [EnableRateLimiting("BasePolicy")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private bool CheckAuthentication(
            Guid? id,
            string? idClaim,
            string? roleClaim,
            HistoryIdType? idType)
        {
            if (string.IsNullOrEmpty(idClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return false;
            }
            else if (id.HasValue && roleClaim.Equals(RoleConstant.USER))
            {
                return false;
            }
            else if (id.HasValue &&
                    idType.Equals(HistoryIdType.ACCOUNT) &&
                    roleClaim.Equals(RoleConstant.USER) &&
                    id != Guid.Parse(idClaim))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //Get All History
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetHistoryReq getHistoryReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            bool isAuthen = CheckAuthentication(
                getHistoryReq.Id,
                userIdClaim,
                userRoleClaim,
                getHistoryReq.IdType);

            if (isAuthen == false) return Forbid();

            var result = await _historyService.GetHistoriesAsync(getHistoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete History
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _historyService.DeleteHistoryAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Delete History
        [Authorize]
        [HttpDelete("bulk")]
        public async Task<IActionResult> BulkDeleteAsync()
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Forbid();

            Guid accId = Guid.Parse(userIdClaim);

            var result = await _historyService.DeleteRangeHistoriesAsync(accId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
