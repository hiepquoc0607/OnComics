using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private bool CheckAuthentication(
            Guid? id,
            string? idClaim,
            string? roleClaim,
            CmtIdType? idType)
        {
            if (id.HasValue &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER))
                return false;

            if (id.HasValue &&
                idType.Equals(CmtIdType.ACCOUNT) &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER) &&
                id != Guid.Parse(idClaim!))
            {
                return false;
            }

            return true;
        }

        //Get All Comments
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetCommentReq getCommentReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            string? userRoleClaim = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            bool isAuthen = CheckAuthentication(
                getCommentReq.Id,
                userIdClaim,
                userRoleClaim,
                getCommentReq.IdType);

            if (isAuthen == false) return Forbid();

            var result = await _commentService.GetCommentsAsync(getCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Get All Reply Comments
        [Authorize]
        [HttpGet("{id}/reply-comments")]
        public async Task<IActionResult> GetReplyCommentsAsync([FromRoute] Guid id)
        {
            var result = await _commentService.GetReplyCommentsAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Comment
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromForm] CreateCommentReq createCommentReq,
            [FromForm] List<IFormFile>? files)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();

            Guid accId = Guid.Parse(userIdClaim);

            var result = await _commentService.CreateCommentAsync(accId, files, createCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Reply Comment
        [Authorize]
        [HttpPost("{id}/reply-comments")]
        public async Task<IActionResult> ReplyCommentAsync(
            [FromRoute] Guid id,
            [FromForm] CreateCommentReq createCommentReq,
            [FromForm] List<IFormFile>? files)
        {
            string? userIdClaim = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();

            Guid accId = Guid.Parse(userIdClaim);

            var result = await _commentService.ReplyCommentAsync(id, accId, files, createCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comment
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateCommentReq updateCommentReq)
        {
            var result = await _commentService.UpdateCommentAsync(id, updateCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Comment
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var result = await _commentService.DeleteCommentAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
