using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Favorite;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<ObjectResponse<IEnumerable<FavoriteRes>?>> GetFavoritesAsync(GetFavoriteReq getFavoriteReq);

        Task<ObjectResponse<Favorite>> CreateFavoriteAsync(Guid accId, CreateFavoriteReq createFavoriteReq);

        Task<VoidResponse> DeleteFavoriteAsync(Guid id);
    }
}
