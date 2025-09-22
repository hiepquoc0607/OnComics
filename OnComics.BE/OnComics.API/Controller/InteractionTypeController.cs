using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [ApiController]
    public class InteractionTypeController : ControllerBase
    {
        private readonly IInteractionService _interactionService;

        public InteractionTypeController(IInteractionService interactionService)
        {
            _interactionService = interactionService;
        }

        //Get All Interaction Types
        [HttpGet]
        [Route("api/interaction-types")]
        public async Task<IActionResult> GetIrtTypesAsync([FromQuery] GetItrTypeReq getItrTypeReq)
        {
            var result = await _interactionService.GetItrTypesAsync(getItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Interaction Type By Id
        [HttpGet]
        [Route("api/interaction-types/{id}")]
        public async Task<IActionResult> GetItrTypeByIdAsync([FromRoute] int id)
        {
            var result = await _interactionService.GetItrTypeByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Interaction Type
        [HttpPut]
        [Route("api/interaction-types")]
        public async Task<IActionResult> CreateItrTypeAsync([FromBody] CreateItrTypeReq createItrTypeReq)
        {
            var result = await _interactionService.CreateItrTypeAsync(createItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type
        [HttpPost]
        [Route("api/interaction-types/{id}")]
        public async Task<IActionResult> UpdateItrTypeAsync([FromRoute] int id, [FromBody] UpdateItrTypeReq updateItrTypeReq)
        {
            var result = await _interactionService.UpdateItrTypeAsync(id, updateItrTypeReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Interaction Type Status
        [HttpPatch]
        [Route("api/interaction-types/{id}/status")]
        public async Task<IActionResult> UpdateIrtTypeStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<ItrTypeStatus> updateStatusReq)
        {
            var result = await _interactionService.UpdateItrTypeStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Interaction Type
        [HttpDelete]
        [Route("api/interaction-types/{id}")]
        public async Task<IActionResult> DeleteIrtTypeAsync([FromRoute] int id)
        {
            var result = await _interactionService.DeleteItrTypeAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
