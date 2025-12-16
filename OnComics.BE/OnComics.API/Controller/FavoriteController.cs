using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Services.Interfaces;

namespace OnComics.API.Controller
{
    [Route("api/favorite")]
    [ApiController]
    [EnableRateLimiting("BasePolicy")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        //Get All Favorite
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetFavoriteReq getFavoriteReq)
        {
            var result = await _favoriteService.GetFavoritesAsync(getFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Create Favorite
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateFavoriteReq createFavoriteReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            Guid accId = Guid.Parse(userIdClaim!);

            var result = await _favoriteService.CreateFavoriteAsync(accId, createFavoriteReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Favorite
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _favoriteService.DeleteFavoriteAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
