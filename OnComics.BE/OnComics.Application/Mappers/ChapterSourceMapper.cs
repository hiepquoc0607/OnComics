using Mapster;
using OnComics.Application.Models.Request.ChapterSource;
using OnComics.Application.Models.Response.ChapterSource;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class ChapterSourceMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Chaptersource, ChapterSourceRes>();

            config.NewConfig<CreateChapterSourceReq, Chaptersource>()
                .Map(dest => dest.Id, src => Guid.NewGuid());

            config.NewConfig<UpdateChapterSourceReq, Chaptersource>();
        }
    }
}
