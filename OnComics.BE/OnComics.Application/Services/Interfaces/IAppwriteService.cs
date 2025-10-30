using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Response.Appwrite;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAppwriteService
    {
        Task<FileRes> GetFileAsync(string id);

        Task<byte[]> GetFileDownloadAsync(string id);

        Task<FileRes> CreateFileAsync(IFormFile file, string fileName);

        Task UpdateFileAsync(string id, IFormFile file, string fileName);

        Task DeleteFileAsync(string id);
    }
}
