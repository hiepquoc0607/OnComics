using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Models.Response.ComicRating;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface IComicRatingService
    {
        Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsByAccountIdAsync(int accId, GetComicRatingReq getComicRatingReq);

        Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsByComicIdAsync(int comicId, GetComicRatingReq getComicRatingReq);

        Task<ObjectResponse<Comicrating>> CreateRatingAsync(int accId, CreateComicRatingReq createComicRatingReq);

        Task<VoidResponse> UpdateRatingAsync(int id, UpdateComicRatingReq updateRatingReq);

        Task<VoidResponse> DeleteRatingAsync(int id);
    }
}
