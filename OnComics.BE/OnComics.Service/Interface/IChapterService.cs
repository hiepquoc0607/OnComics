using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Chapter;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Chapter;

namespace OnComics.Service.Interface
{
    public interface IChapterService
    {
        Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersByComicIdAsync(GetChapterReq getChapterReq);

        Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(int id);

        Task<ObjectResponse<Chapter>> CreateChapterAsync(CreateChapterReq createChapterReq);

        Task<ObjectResponse<IEnumerable<Chapter>>> CreateChaptersAsync(List<CreateChapterReq> chapters);

        Task<VoidResponse> UpdateChapterAsync(int id, UpdateChapterReq updateChapterReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ChapStatus> updateStatusReq);

        Task<VoidResponse> DeleteChapterAsync(int id);
    }
}