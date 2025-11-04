using OnComics.Application.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace OnComics.Application.Services.Implements
{
    public class FileService : IFileService
    {
        public async Task<MemoryStream> ResizeProfileAsync(MemoryStream memoryStream)
        {
            try
            {
                if (memoryStream.CanSeek)
                    memoryStream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(memoryStream, default)
                    .ConfigureAwait(false);

                image.Mutate(x =>
                {
                    x.AutoOrient();
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(512, 512),
                        Mode = ResizeMode.Max,
                        Sampler = KnownResamplers.Bicubic
                    });
                });

                IImageEncoder encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossless,
                    Quality = default
                };

                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MemoryStream> ResizeEmoteAsync(MemoryStream memoryStream)
        {
            try
            {
                if (memoryStream.CanSeek)
                    memoryStream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(memoryStream, default)
                    .ConfigureAwait(false);

                image.Mutate(x =>
                {
                    x.AutoOrient();
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(128, 128),
                        Mode = ResizeMode.Max,
                        Sampler = KnownResamplers.Bicubic
                    });
                });

                IImageEncoder encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossless,
                    Quality = default
                };

                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<MemoryStream> ConvertWebPAsync(MemoryStream memoryStream)
        {
            try
            {
                if (memoryStream.CanSeek)
                    memoryStream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(memoryStream, default)
                    .ConfigureAwait(false);

                IImageEncoder encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossless,
                    Quality = default
                };

                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;

                return outputStream;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<MemoryStream> ConvertMarkdownAsync(MemoryStream memoryStream, string extension)
        {
            try
            {
                FormatType loadFormat = extension switch
                {
                    ".doc" => FormatType.Doc,
                    ".rtf" => FormatType.Rtf,
                    ".txt" => FormatType.Txt,
                    _ => FormatType.Docx
                };

                using var document = new WordDocument(memoryStream, loadFormat);

                await using var outStream = new MemoryStream();
                document.Save(outStream, FormatType.Markdown);
                outStream.Position = 0;

                return outStream;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
