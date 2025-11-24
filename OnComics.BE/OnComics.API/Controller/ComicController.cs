using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Comic;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/comics")]
    [ApiController]
    public class ComicController : ControllerBase
    {
        private readonly IComicService _comicService;

        public ComicController(IComicService comicService)
        {
            _comicService = comicService;
        }

        //Get All Comics
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetComicReq getComicReq)
        {
            var result = await _comicService.GetComicsAsync(getComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Comic By Id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _comicService.GetComicByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Comic
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateComicReq createComicReq)
        {
            var result = await _comicService.CreateComicAsync(createComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic
        [Authorize(Policy = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateComicReq updateComicReq)
        {
            var result = await _comicService.UpdateComicAsync(id, updateComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic Thumnail
        [Authorize(Policy = "Admin")]
        [RequestSizeLimit(2 * 1024 * 1024)] //Limit File To Max 2 MB
        [HttpPatch("{id:guid}/thumnail")]
        public async Task<IActionResult> UpdateThumbnailAsync(
            [FromRoute] Guid id,
            IFormFile file)
        {
            var result = await _comicService.UpdateThumbnailAsync(id, file);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Guid id,
            [FromQuery] UpdateStatusReq<ComicStatus> updateStatusReq)
        {
            var result = await _comicService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Comic
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _comicService.DeleteComicAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
