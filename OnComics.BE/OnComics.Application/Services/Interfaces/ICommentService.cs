using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Models.Response.Comment;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ObjectResponse<IEnumerable<CommentRes>?>> GetCommentsAsync(GetCommentReq getCommentReq);

        Task<ObjectResponse<IEnumerable<CommentRes>?>> GetReplyCommentsAsync(int mainCmtId);

        Task<ObjectResponse<Comment>> CreateCommentAsync(CreateCommentReq createCommentReq);

        Task<ObjectResponse<Comment>> ReplyCommentAsync(int mainCmtId, CreateCommentReq createCommentReq);

        Task<VoidResponse> UpdateCommentAsync(int id, UpdateCommentReq updateCommentReq);

        Task<VoidResponse> DeleteCommentAsync(int id);
    }
}
