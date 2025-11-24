using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/chapters")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;
        private readonly IChapterSourceService _chapterSourceService;

        public ChapterController(
            IChapterService chapterService,
            IChapterSourceService chapterSourceService)
        {
            _chapterService = chapterService;
            _chapterSourceService = chapterSourceService;
        }

        //Get All Chapters
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetChapterReq getChapterReq)
        {
            var result = await _chapterService.GetChaptersAsync(getChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Chapter By Id
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            string? userIdClaim = HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Forbid();

            Guid accId = Guid.Parse(userIdClaim);

            var result = await _chapterService.GetChapterByIdAsync(id, accId);

            return StatusCode(result.StatusCode, result);
        }

        //Create Chapter
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromForm] CreateChapterReq createChapterReq,
            [FromForm] List<IFormFile> files)
        {
            var result = await _chapterService
                .CreateChapterAsync(files, createChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter
        [Authorize(Policy = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateChapterReq updateChapterReq)
        {
            var result = await _chapterService.UpdateChapterAsync(id, updateChapterReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Guid id,
            [FromQuery] UpdateStatusReq<ChapterStatus> updateStatusReq)
        {
            var result = await _chapterService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Sources
        [Authorize(Policy = "Admin")]
        [HttpPut("{id:guid}/chapter-sources")]
        public async Task<IActionResult> UpdateSourcesAsync(
            [FromRoute] Guid id,
            [FromForm] List<IFormFile> files)
        {
            var result = await _chapterSourceService
                .UpdateChapterSourceAsync(id, files);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter Sources
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id:guid}/chapter-sources")]
        public async Task<IActionResult> BulkDeleteAsync([FromRoute] Guid id)
        {
            var result = await _chapterSourceService
                .DeleteChapterSourcesAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}