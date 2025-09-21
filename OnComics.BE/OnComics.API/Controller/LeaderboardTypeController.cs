using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.LeaderboardType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.LeaderboardType;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [ApiController]
    public class LeaderboardTypeController : ControllerBase
    {
        private readonly ILeaderboardTypeService _leaderboardTypeService;

        public LeaderboardTypeController(ILeaderboardTypeService leaderboardTypeService)
        {
            _leaderboardTypeService = leaderboardTypeService;
        }

        //Get All Leaderboard Types
        [HttpGet]
        [Route("api/leaderboard-types")]
        public async Task<IActionResult> GetLdbTypesAsync([FromQuery] GetLdbTypeReq getLdbTypeReq)
        {
            var result = await _leaderboardTypeService.GetLdbTypesAsync(getLdbTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Leaderboard Type By Id
        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("api/leaderboard-types/{id}")]
        public async Task<IActionResult> GetLdbTypeByIdAsync([FromRoute] int id)
        {
            var result = await _leaderboardTypeService.GetLbdTypeByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Leaderboard Type
        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/leaderboard-types")]
        public async Task<IActionResult> CreateLbdTypeAsync([FromBody] CreateLdbTypeReq createLdbTypeReq)
        {
            var result = await _leaderboardTypeService.CreateLdbTypeAsync(createLdbTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Leaderboard Type
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/leaderboard-types/{id}")]
        public async Task<IActionResult> UpdateLbdTypeAsync([FromRoute] int id, [FromBody] UpdateLdbTypeReq updateLdbTypeReq)
        {
            var result = await _leaderboardTypeService.UpdateLdbTypeAsync(id, updateLdbTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Leaderboard Type Status
        [Authorize(Policy = "Admin")]
        [HttpPatch]
        [Route("api/leaderboard-types/{id}/status")]
        public async Task<IActionResult> UpdateLbdTypeStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<LdbTypeStatus> updateStatusReq)
        {
            var result = await _leaderboardTypeService.UpdateLdbTypeStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Leaderboard Type
        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("api/leaderboard-types/{id}")]
        public async Task<IActionResult> DeleteLbdTypeAsync([FromRoute] int id)
        {
            var result = await _leaderboardTypeService.DeleteLbdTypeAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
