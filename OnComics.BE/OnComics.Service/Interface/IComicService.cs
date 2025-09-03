using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Comic;

namespace OnComics.Service.Interface
{
    public interface IComicService
    {
        Task<ObjectResponse<IEnumerable<ComicRes>?>> GetComicsAsync(GetComicReq getComicReq);

        Task<ObjectResponse<ComicRes?>> GetComicByIdAsync(GetComicByIdReq getComicByIdReq);

        Task<ObjectResponse<ComicRes>> CreateComicAsync(CreateComicReq createComicReq);

        Task<VoidResponse> UpdateComicAsync(int id, UpdateComicReq updateComicReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ComicStatus> updateStatusReq);

        Task<VoidResponse> DeleteComicAsync(int id);
    }
}