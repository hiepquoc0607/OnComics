using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
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
        [Route("api/category")]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] GetCategoryReq getCategoryReq)
        {
            var result = await _categoryService.GetCategoriesAsync(getCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Category By Id
        [HttpGet]
        [Route("api/category/{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Category
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/category")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryReq createCategoryReq)
        {
            var result = await _categoryService.CreateCategoryAsync(createCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Bulk(Range) Create Categories
        [Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/category/bulk")]
        public async Task<IActionResult> CreateCategoriesAsync([FromBody] List<CreateCategoryReq> categories)
        {
            var result = await _categoryService.CreateCategoriesAsync(categories);

            return StatusCode(result.StatusCode, result);
        }

        //Update Category
        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/category/{id}")]
        public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] UpdateCategoryReq updateCategoryReq)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Category Status
        [Authorize(Policy = "Admin")]
        [HttpPatch]
        [Route("api/category/{id}/status")]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromBody] UpdateStatusReq<CategoryStatus> updateStatusReq)
        {
            var result = await _categoryService.UpdateStatusAsync(id, updateStatusReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Category
        [Authorize(Policy = "Admin")]
        [HttpDelete]
        [Route("api/category/{id}")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}