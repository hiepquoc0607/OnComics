using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IChapterSourceService
    {
        Task<ObjectResponse<IEnumerable<Chaptersource>>> UpdateChapterSourceAsync(
            Guid chapterId,
            List<IFormFile> files);

        Task<VoidResponse> DeleteChapterSourcesAsync(Guid chapterId);
    }
}
