using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Enums.ComicRating;
using OnComics.Application.Enums.Comment;
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

        private bool CheckAuthentication(int id, string? idClaim, string? roleClaim, RatingIdType idType)
        {
            if (idType.Equals(CmtIdType.ACCOUNT) &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER) &&
                id != int.Parse(idClaim!))
            {
                return false;
            }

            return true;
        }

        //Get All Ratings
        [Authorize]
        [Route("api/comic-ratings")]
        [HttpGet]
        public async Task<IActionResult> GetRatingsByAccountIdAsync([FromQuery] GetComicRatingReq getComicRatingReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            bool isAuthen = CheckAuthentication(
                getComicRatingReq.Id,
                userIdClaim,
                userRoleClaim,
                getComicRatingReq.IdType);

            var result = await _comicRatingService.GetRatingsAsync(getComicRatingReq);

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
