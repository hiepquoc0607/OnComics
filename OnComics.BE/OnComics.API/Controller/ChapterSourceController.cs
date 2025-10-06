using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Models.Request.ChapterSource;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
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
        [Route("api/chapter-sources")]
        public async Task<IActionResult> GetChapterSourcesAsync([FromQuery] GetChapterSourceReq getChapterSourceReq)
        {
            var result = await _chapterSourceService.GetChapterSourcesAsync(getChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Create Chapter Source
        [HttpPost]
        [Route("api/chapter-sources")]
        public async Task<IActionResult> CreateChapterSourceAsync([FromBody] CreateChapterSourceReq createChapterSourceReq)
        {
            var result = await _chapterSourceService.CreateChapterSourceAsync(createChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk (Range) Create Chapter Sources
        [HttpPost]
        [Route("api/chapter-sources/bulk")]
        public async Task<IActionResult> CreateChapterSourcesAsync([FromBody] List<CreateChapterSourceReq> sources)
        {
            var result = await _chapterSourceService.CreateChapterSourcesAsync(sources);

            return StatusCode(result.StatusCode, result);
        }

        //Update Chapter Source
        [HttpPut]
        [Route("api/chapter-sources/{id}")]
        public async Task<IActionResult> UpdateChapterSourceAsync([FromRoute] int id, [FromBody] UpdateChapterSourceReq updateChapterSourceReq)
        {
            var result = await _chapterSourceService.UpdateChapterSourceAsync(id, updateChapterSourceReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Chapter Source
        [HttpDelete]
        [Route("api/chapter-sources/{id}")]
        public async Task<IActionResult> DeleteChapterSourceAsync([FromRoute] int id)
        {
            var result = await _chapterSourceService.DeleteChapterSourceAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk (Range) Chapter Sources
        [HttpDelete]
        [Route("api/chapter-sources/{chapterId}/bulk")]
        public async Task<IActionResult> DeleteChapterSourcesAsync([FromRoute] int chapterId)
        {
            var result = await _chapterSourceService.DeleteChapterSourcesAsync(chapterId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
