using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/interaction-types")]
    [ApiController]
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _interactionService.GetItrTypeByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Interaction Type
        [HttpPut]
        public async Task<IActionResult> CreateAsync([FromBody] CreateItrTypeReq createItrTypeReq)
        {
            var result = await _interactionService.CreateItrTypeAsync(createItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateItrTypeReq updateItrTypeReq)
        {
            var result = await _interactionService.UpdateItrTypeAsync(id, updateItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type Status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateStatusReq<ItrTypeStatus> updateStatusReq)
        {
            var result = await _interactionService.UpdateItrTypeStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Interaction Type
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _interactionService.DeleteItrTypeAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
