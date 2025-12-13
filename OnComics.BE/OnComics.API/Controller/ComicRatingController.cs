using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using OnComics.Application.Constants;
using OnComics.Application.Enums.ComicRating;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/comic-ratings")]
    [ApiController]
    public class ComicRatingController : ControllerBase
    {
        private readonly IComicRatingService _comicRatingService;

        public ComicRatingController(IComicRatingService comicRatingService)
        {
            _comicRatingService = comicRatingService;
        }

        private bool CheckAuthentication(
            Guid id,
            string? idClaim,
            string? roleClaim,
            RatingIdType idType)
        {
            if (idType.Equals(CmtIdType.ACCOUNT) &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER) &&
                !id.ToString().Equals(idClaim))
            {
                return false;
            }

            return true;
        }

        //Get All Ratings
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetComicRatingReq getComicRatingReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
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
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateComicRatingReq createComicRatingReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            Guid accId = Guid.Parse(userIdClaim!);

            var result = await _comicRatingService.CreateRatingAsync(accId, createComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Rating
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateComicRatingReq updateComicRatingReq)
        {
            var result = await _comicRatingService.UpdateRatingAsync(id, updateComicRatingReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Rating
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _comicRatingService.DeleteRatingAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
