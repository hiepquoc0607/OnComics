using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Request.General;
using OnComics.Service.Interface;

namespace OnComics.API.Controller
{
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
        [Route("api/comic")]
        public async Task<IActionResult> GetComicsAsync([FromQuery] GetComicReq getComicReq)
        {
            var result = await _comicService.GetComicsAsync(getComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Comic By Id
        [HttpGet]
        [Route("api/comic/{id}")]
        public async Task<IActionResult> GetComicByIdAsync([FromRoute] int id)
        {
            var result = await _comicService.GetComicByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Comic
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/comic")]
        public async Task<IActionResult> CreateComicAsync([FromBody] CreateComicReq createComicReq)
        {
            var result = await _comicService.CreateComicAsync(createComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic
        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/comic/{id}")]
        public async Task<IActionResult> UpdateComicAsync([FromRoute] int id, [FromBody] UpdateComicReq updateComicReq)
        {
            var result = await _comicService.UpdateComicAsync(id, updateComicReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comic Status
        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/comic/{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] int id,
            [FromBody] UpdateStatusReq<ComicStatus> updateStatusReq)
        {
            var result = await _comicService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Comic
        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("api/comic/{id}")]
        public async Task<IActionResult> DeleteComicAsync([FromRoute] int id)
        {
            var result = await _comicService.DeleteComicAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
