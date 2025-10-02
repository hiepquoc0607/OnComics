using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/[controller]")]
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
        [Route("api/interactions")]
        public async Task<IActionResult> GetInteractionsAsync([FromQuery] GetInteractionReq getInteractionReq)
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
        [HttpGet]
        [Route("api/interactions/{id}")]
        public async Task<IActionResult> GetInteractionByIdAsync([FromRoute] int id)
        {
            var result = await _interactionService.GetInteractionByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Interaction
        [Authorize]
        [HttpPost]
        [Route("api/interactions")]
        public async Task<IActionResult> CreateInteractionAsync([FromBody] CreateInteractionReq createInteractionReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int accId = int.Parse(userIdClaim!);

            var result = await _interactionService.CreateInteractionAsync(accId, createInteractionReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction
        [Authorize]
        [HttpPut]
        [Route("api/interactions/{id}")]
        public async Task<IActionResult> UpdateInteractionAsync([FromRoute] int id, UpdateInteractionReq updateInteractionReq)
        {
            var result = await _interactionService.UpdateInteractionAsync(id, updateInteractionReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Interaction
        [Authorize]
        [HttpDelete]
        [Route("api/interactions/{id}")]
        public async Task<IActionResult> DeleteInteractionAsync([FromRoute] int id)
        {
            var result = await _interactionService.DeleteInteractionAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
