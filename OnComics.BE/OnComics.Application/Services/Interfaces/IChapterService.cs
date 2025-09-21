using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Chapter;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface IChapterService
    {
        Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersByComicIdAsync(GetChapterReq getChapterReq);

        Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(int id);

        Task<ObjectResponse<Chapter>> CreateChapterAsync(CreateChapterReq createChapterReq);

        Task<ObjectResponse<IEnumerable<Chapter>>> CreateChaptersAsync(List<CreateChapterReq> chapters);

        Task<VoidResponse> UpdateChapterAsync(int id, UpdateChapterReq updateChapterReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ChapterStatus> updateStatusReq);

        Task<VoidResponse> DeleteChapterAsync(int id);
    }
}
