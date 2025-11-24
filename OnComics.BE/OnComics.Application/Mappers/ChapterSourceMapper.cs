using Mapster;
using OnComics.Application.Models.Response.ChapterSource;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class ChapterSourceMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Chaptersource, ChapterSourceRes>();
        }
    }
}
