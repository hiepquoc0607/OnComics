using OnComics.Application.Models.Request.ChapterSource;
using OnComics.Application.Models.Response.ChapterSource;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IChapterSourceService
    {
        Task<ObjectResponse<IEnumerable<ChapterSourceRes>?>> GetChapterSourcesAsync(GetChapterSourceReq getChapterSourceReq);

        Task<ObjectResponse<Chaptersource>> CreateChapterSourceAsync(CreateChapterSourceReq createChapterSourceReq);

        Task<ObjectResponse<IEnumerable<Chaptersource>>> CreateChapterSourcesAsync(List<CreateChapterSourceReq> sources);

        Task<VoidResponse> UpdateChapterSourceAsync(int id, UpdateChapterSourceReq updateChapterSourceReq);

        Task<VoidResponse> DeleteChapterSourceAsync(int id);

        Task<VoidResponse> DeleteChapterSourcesAsync(int chapterId);
    }
}
