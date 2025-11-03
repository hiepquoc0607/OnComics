namespace OnComics.Application.Services.Interfaces
{
    public interface IFileService
    {
        Task<MemoryStream> ResizeProfileAsync(MemoryStream memoryStream);

        Task<MemoryStream> ResizeReactAsync(MemoryStream memoryStream);

        Task<MemoryStream> ConvertWebPAsync(MemoryStream memoryStream);

        Task<MemoryStream> ConvertMarkdownAsync(MemoryStream memoryStream, string extension);
    }
}
