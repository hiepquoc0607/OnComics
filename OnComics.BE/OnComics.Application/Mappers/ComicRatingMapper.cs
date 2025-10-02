using Mapster;
using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Models.Response.ComicRating;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class ComicRatingMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Comicrating, ComicRatingRes>();

            config.NewConfig<CreateComicRatingReq, Comicrating>();

            config.NewConfig<UpdateComicRatingReq, Comicrating>();
        }
    }
}
