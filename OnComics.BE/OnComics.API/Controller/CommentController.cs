using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Services.Interfaces;
using System.Security.Claims;

namespace OnComics.API.Controller
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private bool CheckAuthentication(int? id, string? idClaim, string? roleClaim, CmtIdType? idType)
        {
            if (id.HasValue &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER))
                return false;

            if (id.HasValue &&
                idType.Equals(CmtIdType.ACCOUNT) &&
                !string.IsNullOrEmpty(roleClaim) &&
                roleClaim.Equals(RoleConstant.USER) &&
                id != int.Parse(idClaim!))
            {
                return false;
            }

            return true;
        }

        //Get All Comments
        [Authorize]
        [HttpGet]
        [Route("api/comments")]
        public async Task<IActionResult> GetCommentsAsync([FromQuery] GetCommentReq getCommentReq)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [HttpGet]
        [Route("api/comments/{id}/reply-comments")]
        public async Task<IActionResult> GetReplyCommentsAsync([FromRoute] int id)
        {
            var result = await _commentService.GetReplyCommentsAsync(id);

            return StatusCode(result.StatusCode, result);
        }

        //Create Comment
        [Authorize]
        [HttpPost]
        [Route("api/comments")]
        public async Task<IActionResult> CreateCommentAsync([FromBody] CreateCommentReq createCommentReq)
        {
            var result = await _commentService.CreateCommentAsync(createCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Reply Comment
        [Authorize]
        [HttpPost]
        [Route("api/comments/{id}/reply-comments")]
        public async Task<IActionResult> ReplyCommentAsync([FromRoute] int id, CreateCommentReq createCommentReq)
        {
            var result = await _commentService.ReplyCommentAsync(id, createCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Update Comment
        [Authorize]
        [HttpPut]
        [Route("api/comments/{id}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int id, UpdateCommentReq updateCommentReq)
        {
            var result = await _commentService.UpdateCommentAsync(id, updateCommentReq);

            return StatusCode(result.StatusCode, result);
        }

        //Delete Comment
        [Authorize]
        [HttpDelete]
        [Route("api/comments/{id}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int id)
        {
            var result = await _commentService.DeleteCommentAsync(id);

            return StatusCode(result.StatusCode, result);
        }
    }
}
