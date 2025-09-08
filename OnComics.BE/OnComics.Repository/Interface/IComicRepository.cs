using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Comic;

namespace OnComics.Repository.Interface
{
    public interface IComicRepository
    {
        Task<(IEnumerable<Comic>?, Pagination)> GetComicsAsync(GetComicReq getComicReq);

        Task<Comic?> GetComicByIdAsync(int id);

        Task<ComicRes?> GetComicByIdNoTrackingAsync(int id);

        Task CreateComicAsync(Comic comic);

        Task UpdateComicAsync(Comic comic);

        Task DeleteComicAsync(int id);

        Task<bool> CheckComicExistedAsync(string name, string author);

        Task<bool> CheckComicIdAsync(int id);

        Task<int[]> GetComicIdsAsync();
    }
}