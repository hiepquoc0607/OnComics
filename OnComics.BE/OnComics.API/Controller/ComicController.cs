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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateComicReq updateComicReq)
        {
            var result = await _comicService.UpdateComicAsync(id, updateComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<ComicStatus> updateStatusReq)
        {
            var result = await _comicService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Comic
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await _comicService.DeleteComicAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
