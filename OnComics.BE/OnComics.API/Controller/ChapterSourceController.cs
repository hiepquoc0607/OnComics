using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Models.Request.ChapterSource;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/chapter-sources")]
    [ApiController]
    public class ChapterSourceController : ControllerBase
    {
        private readonly IChapterSourceService _chapterSourceService;

        public ChapterSourceController(IChapterSourceService chapterSourceService)
        {
            _chapterSourceService = chapterSourceService;
        }

        //Get All Chapter Sources
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetChapterSourceReq getChapterSourceReq)
        {
            var result = await _chapterSourceService
                .GetChapterSourcesAsync(getChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Create Chapter Source
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateChapterSourceReq createChapterSourceReq)
        {
            var result = await _chapterSourceService
                .CreateChapterSourceAsync(createChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk (Range) Create Chapter Sources
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateAsync([FromBody] List<CreateChapterSourceReq> sources)
        {
            var result = await _chapterSourceService
                .CreateRangeChapterSourcesAsync(sources);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Source
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateChapterSourceReq updateChapterSourceReq)
        {
            var result = await _chapterSourceService
                .UpdateChapterSourceAsync(id, updateChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter Source
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _chapterSourceService.DeleteChapterSourceAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk (Range) Chapter Sources
        [HttpDelete("{chapterId}/bulk")]
        public async Task<IActionResult> BulkDeleteAsync([FromRoute] Guid chapterId)
        {
            var result = await _chapterSourceService
                .DeleteRangeChapterSourcesAsync(chapterId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
