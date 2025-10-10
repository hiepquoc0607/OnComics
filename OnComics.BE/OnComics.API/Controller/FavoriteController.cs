using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/favorite")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        //Get All Favorite
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetFavoriteReq getFavoriteReq)
        {
            var result = await _favoriteService.GetFavoritesAsync(getFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Favorite By Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var result = await _favoriteService.GetFavoriteByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Favorite
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFavoriteReq createFavoriteReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var accId = int.Parse(userIdClaim!);

            var result = await _favoriteService.CreateFavoriteAsync(accId, createFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Favorite
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var result = await _favoriteService.DeleteFavoriteAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
