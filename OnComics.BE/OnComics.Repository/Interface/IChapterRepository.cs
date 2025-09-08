using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Chapter;
using OnComics.Library.Models.Response.Api;

namespace OnComics.Repository.Interface
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        Task<(IEnumerable<Chapter>?, Pagination)> GetChaptersAsync(GetChapterReq getChapterReq);

        Task<Chapter?> GetChapterByIdAsync(int id, bool isTracking);

        Task CreateChapterAsync(Chapter chapter);

        Task CreateChaptersAsync(IEnumerable<Chapter> chapters);

        Task UpdateChapterAsync(Chapter chapter);

        Task DeleteChapterAsync(int id);

        Task<int> GetMaxChapNoByComicIdAsync(int id);

        Task<Dictionary<int, int>> GetMaxChapNosByComicIdsAsync(int[] ids);
    }
}