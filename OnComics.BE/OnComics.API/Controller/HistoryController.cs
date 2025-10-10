using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Enums.History;
using OnComics.Application.Models.Request.History;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/history")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        private bool CheckAuthentication(int? id, string? idClaim, string? roleClaim, HistoryIdType? idType)
        {
            if (id.HasValue &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER))
                return false;

            if (id.HasValue &&
                idType.Equals(CmtIdType.ACCOUNT) &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER) &&
                id != int.Parse(idClaim!))
            {
                return false;
            }

            return true;
        }

        //Get All History
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromRoute] GetHistoryReq getHistoryReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        //Create History
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateHistoryReq createHistoryReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int accId = int.Parse(userIdClaim!);

            var result = await _historyService.CreateHistroyAsync(accId, createHistoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Updaate History
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, UpdateHistoryReq updateHistoryReq)
        {
            var result = await _historyService.UpdateHistroyAsync(id, updateHistoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete History
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await _historyService.DeleteHistoryAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Delete History
        [Authorize]
        [HttpDelete("{accId}/bulk")]
        public async Task<IActionResult> BulkDeleteAsync([FromRoute] int accId)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim != null &&
                accId != int.Parse(userIdClaim))
                return Forbid();

            var result = await _historyService.DeleteRangeHistoriesAsync(accId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
