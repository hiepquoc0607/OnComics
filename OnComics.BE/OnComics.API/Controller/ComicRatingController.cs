using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [ApiController]
    public class ComicRatingController : ControllerBase
    {
        private readonly IComicRatingService _comicRatingService;

        public ComicRatingController(IComicRatingService comicRatingService)
        {
            _comicRatingService = comicRatingService;
        }

        //Get All Ratings By Account Id
        [Authorize]
        [Route("api/comic-ratings/accounts/{accId}")]
        [HttpGet]
        public async Task<IActionResult> GetRatingsByAccountIdAsync([FromRoute] int accId, [FromQuery] GetComicRatingReq getComicRatingReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || userRoleClaim == null ||
                (!userIdClaim.Equals(accId.ToString()) && !userRoleClaim.Equals(RoleConstant.ADMIN)))
                return Forbid();

            var result = await _comicRatingService.GetRatingsByAccountIdAsync(accId, getComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get All Ratings By Comic Id
        [Authorize(Roles = "Admin")]
        [Route("api/comic-ratings/comics/{comicId}")]
        [HttpGet]
        public async Task<IActionResult> GetRatingsByComicIdAsync([FromRoute] int comicId, [FromQuery] GetComicRatingReq getComicRatingReq)
        {
            var result = await _comicRatingService.GetRatingsByComicIdAsync(comicId, getComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Create Rating
        [Authorize]
        [Route("api/comic-ratings/")]
        [HttpPost]
        public async Task<IActionResult> CreateRatingAsync([FromBody] CreateComicRatingReq createComicRatingReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int accId = int.Parse(userIdClaim!);

            var result = await _comicRatingService.CreateRatingAsync(accId, createComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Rating
        [Authorize]
        [Route("api/comic-ratings/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateRatingAsync([FromRoute] int id, [FromBody] CreateComicRatingReq createComicRatingReq)
        {
            var result = await _comicRatingService.CreateRatingAsync(id, createComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Rating
        [Authorize]
        [Route("api/comic-ratings/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRatingAsync([FromRoute] int id)
        {
            var result = await _comicRatingService.DeleteRatingAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
