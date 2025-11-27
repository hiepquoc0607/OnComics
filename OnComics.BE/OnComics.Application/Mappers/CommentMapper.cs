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
                .Map(dest => dest.Id, opt => Guid.NewGuid())
                .Map(dest => dest.IsEdited, opt => false)
                .Map(dest => dest.IsMainCmt, opt => true)
                .Map(dest => dest.CmtTime, opt => DateTime.UtcNow)
                .Map(dest => dest.InteractionNum, opt => 0);

            config.NewConfig<UpdateCommentReq, Comment>()
                .Map(dest => dest.IsEdited, src => true);
        }
    }
}
