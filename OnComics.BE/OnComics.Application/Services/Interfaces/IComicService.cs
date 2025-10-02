using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Comic;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Comic;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IComicService
    {
        Task<ObjectResponse<IEnumerable<ComicRes>?>> GetComicsAsync(GetComicReq getComicReq);

        Task<ObjectResponse<ComicRes?>> GetComicByIdAsync(int id);

        Task<ObjectResponse<Comic>> CreateComicAsync(CreateComicReq createComicReq);

        Task<VoidResponse> UpdateComicAsync(int id, UpdateComicReq updateComicReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ComicStatus> updateStatusReq);

        Task<VoidResponse> DeleteComicAsync(int id);
    }
}
