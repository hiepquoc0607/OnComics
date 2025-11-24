using Microsoft.AspNetCore.Http;
using OnComics.Application.Models.Response.Appwrite;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAppwriteService
    {
        Task<FileRes> GetFileAsync(string id);

        Task<byte[]> GetFileDownloadAsync(string id);

        Task<FileRes> CreateFileAsync(IFormFile file, string fileName);

        Task<FileRes> CreateProfileFileAsync(IFormFile file, string fileName);

        Task<FileRes> CreateThumbnailFileAsync(IFormFile file, string fileName);

        Task<FileRes> CreateImgSourceFileAsync(IFormFile file, string fileName);

        Task<FileRes> CreateEmoteFileAsync(IFormFile file, string fileName);

        Task<FileRes> UpdateFileAsync(string id, IFormFile file, string fileName);

        Task<FileRes> UpdateProfileFileAsync(string id, IFormFile file, string fileName);

        Task<FileRes> UpdateThumbnailFileAsync(string id, IFormFile file, string fileName);

        Task<FileRes> UpdateImgSourceFileAsync(string id, IFormFile file, string fileName);

        Task<FileRes> UpdateEmoteFileAsync(string id, IFormFile file, string fileName);

        Task DeleteFileAsync(string id);
    }
}
