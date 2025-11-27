using Mapster;
using OnComics.Application.Models.Request.Comment;
using OnComics.Application.Models.Response.Comment;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class CommentMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Comment, CommentRes>();

            config.NewConfig<CreateCommentReq, Comment>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.IsEdited, src => false)
                .Map(dest => dest.IsMainCmt, src => true)
                .Map(dest => dest.MainCmtId, src => Guid.Empty)
                .Map(dest => dest.CmtTime, src => DateTime.UtcNow)
                .Map(dest => dest.InteractionNum, src => 0);

            config.NewConfig<UpdateCommentReq, Comment>()
                .Map(dest => dest.IsEdited, src => true);
        }
    }
}
