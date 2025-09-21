using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{

    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        //Get All Chapters
        [HttpGet]
        [Route("api/chapter")]
        public async Task<IActionResult> GetChaptersByComicIdAsync([FromQuery] GetChapterReq getChapterReq)
        {
            var result = await _chapterService.GetChaptersByComicIdAsync(getChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Chapter By Id
        [HttpGet]
        [Route("api/chapter/{id}")]
        public async Task<IActionResult> GetChapterByIdAsync([FromRoute] int id)
        {
            var result = await _chapterService.GetChapterByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Chapter
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/chapter")]
        public async Task<IActionResult> CreateChapterAsync([FromBody] CreateChapterReq createChapterReq)
        {
            var result = await _chapterService.CreateChapterAsync(createChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Create Chapters
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/chapter/bulk")]
        public async Task<IActionResult> CreateChaptersAsync([FromBody] List<CreateChapterReq> chapters)
        {
            var result = await _chapterService.CreateChaptersAsync(chapters);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter
        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/chapter/{id}")]
        public async Task<IActionResult> UpdateChapterAsync([FromRoute] int id, [FromBody] UpdateChapterReq updateChapterReq)
        {
            var result = await _chapterService.UpdateChapterAsync(id, updateChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Status
        [Authorize(Policy = "Admin")]
        [HttpPatch]
        [Route("api/chapter/{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<ChapterStatus> updateStatusReq)
        {
            var result = await _chapterService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter
        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("api/chapter/{id}")]
        public async Task<IActionResult> DeleteChapterAsync([FromRoute] int id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}