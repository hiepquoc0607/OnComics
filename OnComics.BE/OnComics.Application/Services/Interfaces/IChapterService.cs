using Microsoft.AspNetCore.Http;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Chapter;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IChapterService
    {
        Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersAsync(GetChapterReq getChapterReq);

        Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(Guid id, Guid accId);

        Task<ObjectResponse<Chapter>> CreateChapterAsync(List<IFormFile> files, CreateChapterReq createChapterReq);

        Task<VoidResponse> UpdateChapterAsync(Guid id, UpdateChapterReq updateChapterReq);

        Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<ChapterStatus> updateStatusReq);

        Task<VoidResponse> DeleteChapterAsync(Guid id);
    }
}
