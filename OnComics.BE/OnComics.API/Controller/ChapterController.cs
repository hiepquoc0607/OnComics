using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/chapters")]
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
        public async Task<IActionResult> GetAllAsync([FromQuery] GetChapterReq getChapterReq)
        {
            var result = await _chapterService.GetChaptersAsync(getChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Chapter By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await _chapterService.GetChapterByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Chapter
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateChapterReq createChapterReq)
        {
            var result = await _chapterService.CreateChapterAsync(createChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Create Chapters
        [Authorize(Policy = "Admin")]
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateAsync([FromBody] List<CreateChapterReq> chapters)
        {
            var result = await _chapterService.CreateRangeChaptersAsync(chapters);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter
        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateChapterReq updateChapterReq)
        {
            var result = await _chapterService.UpdateChapterAsync(id, updateChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<ChapterStatus> updateStatusReq)
        {
            var result = await _chapterService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}