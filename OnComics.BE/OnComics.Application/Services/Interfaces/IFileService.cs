using Microsoft.AspNetCore.Http;

namespace OnComics.Application.Services.Interfaces
{
    public interface IFileService
    {
        Task<IFormFile> ResizeProfileAsync(IFormFile file);

        Task<IFormFile> ResizeThumbnailAsync(IFormFile file);

        Task<IFormFile> ResizeImgSourceAsync(IFormFile file);

        Task<IFormFile> ResizeEmoteAsync(IFormFile file);

        Task<IFormFile> ConvertWebPAsync(IFormFile file);

        Task<IFormFile> ConvertMarkdownAsync(IFormFile file);

        Task<IFormFile> ConvertIFormFileAsync(byte[] bytes, string fileName);
    }
}
