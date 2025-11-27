using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Models.Response.Comment;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ObjectResponse<IEnumerable<CommentRes>?>> GetCommentsAsync(GetCommentReq getCommentReq);

        Task<ObjectResponse<IEnumerable<CommentRes>?>> GetReplyCommentsAsync(Guid mainCmtId);

        Task<ObjectResponse<Comment>> CreateCommentAsync(
            Guid accId,
            List<IFormFile>? files,
            CreateCommentReq createCommentReq);

        Task<ObjectResponse<Comment>> ReplyCommentAsync(
            Guid mainCmtId,
            Guid accId,
            List<IFormFile>? files,
            CreateCommentReq createCommentReq);

        Task<VoidResponse> UpdateCommentAsync(Guid id, UpdateCommentReq updateCommentReq);

        Task<VoidResponse> DeleteCommentAsync(Guid id);
    }
}
