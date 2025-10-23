using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/interactions")]
    [ApiController]
    public class InteractionController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        //Get All Interactions
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetInteractionReq getInteractionReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userRoleClaim) &&
                userRoleClaim.Equals(RoleConstant.USER) &&
                !getInteractionReq.AccountId.HasValue)
            {
                return Forbid();
            }

            var result = await _interactionService.GetInteractionsAsync(getInteractionReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Interaction By Id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await _interactionService.GetInteractionByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Interaction
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInteractionReq createInteractionReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int accId = int.Parse(userIdClaim!);

            var result = await _interactionService.CreateInteractionAsync(accId, createInteractionReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] UpdateInteractionReq updateInteractionReq)
        {
            var result = await _interactionService.UpdateInteractionAsync(id, updateInteractionReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Interaction
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await _interactionService.DeleteInteractionAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
