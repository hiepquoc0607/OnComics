using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Models.Response.ComicRating;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IComicRatingService
    {
        Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsAsync(GetComicRatingReq getComicRatingReq);

        Task<ObjectResponse<Comicrating>> CreateRatingAsync(Guid accId, CreateComicRatingReq createComicRatingReq);

        Task<VoidResponse> UpdateRatingAsync(Guid id, UpdateComicRatingReq updateRatingReq);

        Task<VoidResponse> DeleteRatingAsync(Guid id);
    }
}
