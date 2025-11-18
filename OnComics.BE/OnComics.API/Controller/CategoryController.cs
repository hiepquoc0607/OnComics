using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //Get All Categories
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetCategoryReq getCategoryReq)
        {
            var result = await _categoryService.GetCategoriesAsync(getCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Category By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Category
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryReq createCategoryReq)
        {
            var result = await _categoryService.CreateCategoryAsync(createCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Create Categories
        [Authorize(Policy = "Admin")]
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateAsync([FromBody] List<CreateCategoryReq> categories)
        {
            var result = await _categoryService.CreateRangeCategoriesAsync(categories);

            return StatusCode(result.StatusCode, result);
        }

        //Update Category
        [Authorize(Policy = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateCategoryReq updateCategoryReq)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Category Status
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync(
            [FromRoute] Guid id,
            [FromQuery] UpdateStatusReq<CategoryStatus> updateStatusReq)
        {
            var result = await _categoryService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Category
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}