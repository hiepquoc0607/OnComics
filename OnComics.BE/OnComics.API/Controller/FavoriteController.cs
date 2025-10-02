using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
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
        [Route("api/favorite")]
        public async Task<IActionResult> GetFavoritesAsync([FromQuery] GetFavoriteReq getFavoriteReq)
        {
            var result = await _favoriteService.GetFavoritesAsync(getFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get Favorite By Id
        [HttpGet]
        [Route("api/favorite/{id}")]
        public async Task<IActionResult> GetFavoriteByIdAsync([FromRoute] int id)
        {
            var result = await _favoriteService.GetFavoriteByIdAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Favorite
        [HttpPost]
        [Route("api/favorite")]
        public async Task<IActionResult> CreateFavoriteAsync([FromBody] CreateFavoriteReq createFavoriteReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var accId = int.Parse(userIdClaim!);

            var result = await _favoriteService.CreateFavoriteAsync(accId, createFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Favorite
        [HttpDelete]
        [Route("api/favorite/{id}")]
        public async Task<IActionResult> DeleteFavoriteAsync([FromRoute] int id)
        {
            var result = await _favoriteService.DeleteFavoriteAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
