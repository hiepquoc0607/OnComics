using Mapster;
using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Models.Response.Favorite;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class FavoriteMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Favorite, FavoriteRes>();

            config.NewConfig<CreateFavoriteReq, FavoriteRes>()
                .Map(dest => dest.Id, src => Guid.NewGuid());
        }
    }
}
