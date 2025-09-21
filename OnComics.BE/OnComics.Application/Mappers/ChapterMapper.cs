using Mapster;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Response.Chapter;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Mappers
{
    public class ChapterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Chapter, ChapterRes>();

            config.NewConfig<CreateChapterReq, Chapter>()
                .Map(dest => dest.ReadNum, src => 0)
                .Map(dest => dest.ReleaseTime, src => DateTime.UtcNow)
                .Map(dest => dest.Status, src => StatusConstant.ACTIVE);

            config.NewConfig<UpdateChapterReq, ChapterRes>();
        }
    }
}
