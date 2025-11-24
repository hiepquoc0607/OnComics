using Microsoft.AspNetCore.Http;
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
        //Resize Profile Picture To 512x512px
        public async Task<IFormFile> ResizeProfileAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(stream, default)
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

                IFormFile ouputfile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/webp"
                };

                return ouputfile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Resize Thumnail Picture To 180x320px
        public async Task<IFormFile> ResizeThumbnailAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(stream, default)
                    .ConfigureAwait(false);

                image.Mutate(x =>
                {
                    x.AutoOrient();
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(180, 320),
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

                IFormFile ouputfile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/webp"
                };

                return ouputfile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Resize Imange Chapter Source To 800x1200px 
        public async Task<IFormFile> ResizeImgSourceAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(stream, default)
                    .ConfigureAwait(false);

                image.Mutate(x =>
                {
                    x.AutoOrient();
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(800, 1200),
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

                IFormFile ouputfile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/webp"
                };

                return ouputfile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Resize Emote Picture To 128x128px
        public async Task<IFormFile> ResizeEmoteAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(stream, default)
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

                IFormFile ouputfile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/webp"
                };

                return ouputfile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Convert Picture To WebP
        public async Task<IFormFile> ConvertWebPAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                using Image image = await Image
                    .LoadAsync(stream, default)
                    .ConfigureAwait(false);

                IImageEncoder encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossless,
                    Quality = default
                };

                var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, encoder);
                outputStream.Position = 0;

                IFormFile ouputfile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".webp")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/webp"
                };

                return ouputfile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Convert Text File To Markdown
        public async Task<IFormFile> ConvertMarkdownAsync(IFormFile file)
        {
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                await using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                ms.Position = 0;

                FormatType loadFormat = ext switch
                {
                    ".doc" => FormatType.Doc,
                    ".rtf" => FormatType.Rtf,
                    ".txt" => FormatType.Txt,
                    _ => FormatType.Docx
                };

                using var document = new WordDocument(ms, loadFormat);

                await using var outputStream = new MemoryStream();
                document.Save(outputStream, FormatType.Markdown);
                outputStream.Position = 0;

                IFormFile outputFile = new FormFile(
                    outputStream,
                    0,
                    outputStream.Length,
                    file.Name,
                    Path.GetFileNameWithoutExtension(file.FileName) + ".md")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "text/markdown"
                };

                return outputFile;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
