using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/interaction-types")]
    [ApiController]
    [EnableRateLimiting("BasePolicy")]
    public class InteractionTypeController : ControllerBase
    {
        private readonly IInteractionTypeService _interactionService;

        public InteractionTypeController(IInteractionTypeService interactionService)
        {
            _interactionService = interactionService;
        }

        //Get All Interaction Types
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetItrTypeReq getItrTypeReq)
        {
            var result = await _interactionService.GetItrTypesAsync(getItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Interaction Type By Id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _interactionService.GetItrTypeByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Interaction Type
        [Authorize(Policy = "Admin")]
        [HttpPut]
        public async Task<IActionResult> CreateAsync([FromForm] CreateItrTypeReq createItrTypeReq)
        {
            var result = await _interactionService.CreateItrTypeAsync(createItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type
        [Authorize(Policy = "Admin")]
        [HttpPost("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateItrTypeReq updateItrTypeReq)
        {
            var result = await _interactionService.UpdateItrTypeAsync(id, updateItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type Image
        [Authorize(Policy = "Admin")]
        [RequestSizeLimit(2 * 1024 * 1024)] //Limit File To Max 2 MB
        [HttpPost("{id:guid}/image")]
        public async Task<IActionResult> UpdateImageAsync(
            [FromRoute] Guid id,
            IFormFile file)
        {
            var result = await _interactionService.UpdateItrTypeImgAsync(id, file);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Interaction Type
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _interactionService.DeleteItrTypeAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
