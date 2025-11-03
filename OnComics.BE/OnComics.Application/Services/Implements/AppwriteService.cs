using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnComics.Application.Enums.Common;
using OnComics.Application.Helpers;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;

namespace OnComics.Application.Services.Implements
{
    public class AppwriteService : IAppwriteService
    {
        private readonly Storage _storage;
        private readonly AppwriteHelper _appwriteHelper;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public AppwriteService(
            Storage storage,
            IOptions<AppwriteHelper> appwriteHelper,
            IFileService fileService,
            IMapper mapper,
            Util util)
        {
            _storage = storage;
            _appwriteHelper = appwriteHelper.Value;
            _fileService = fileService;
            _mapper = mapper;
            _util = util;
        }

        //Get File By Id
        public async Task<FileRes> GetFileAsync(string id)
        {
            try
            {
                var file = await _storage.GetFile(_appwriteHelper.BucketId, id);

                string path = "{endpoint}/storage/buckets/{bucketId}/files/{fileId}/view?project={projectId}"
                    .Replace("{endpoint}", _appwriteHelper.Endpoint)
                    .Replace("{bucketId}", _appwriteHelper.BucketId)
                    .Replace("{fileId}", id)
                    .Replace("{projectId}", _appwriteHelper.ProjectId);

                var fileRes = _mapper.Map<FileRes>(file);
                fileRes.Url = path;

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get File For Download
        public async Task<byte[]> GetFileDownloadAsync(string id)
        {
            try
            {
                var file = await _storage.GetFileDownload(_appwriteHelper.BucketId, id);

                return file;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Upload File
        public async Task<FileRes> CreateFileAsync(IFormFile file, string fileName, ImageType imageType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException("No File Uploaded!");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                var isImage = _util.CheckStringContain(ext);

                int type = CheckFileType(isImage, imageType);

                var ms = new MemoryStream();

                switch (type)
                {
                    case 0: //Is Profile Picture
                        ms = await _fileService.ResizeProfileAsync(ms);
                        break;
                    case 1: //Is React Picture (Emote Content Source)
                        ms = await _fileService.ResizeReactAsync(ms);
                        break;
                    case 2: //Is Chapter Source Picture
                        ms = await _fileService.ConvertWebPAsync(ms);
                        break;
                    case 3: //Is Not Picture
                        ms = await _fileService.ConvertMarkdownAsync(ms, ext);
                        break;
                }

                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var inputFile = InputFile
                        .FromBytes(bytes, fileName, file.ContentType);

                List<string> permissions = new List<string>();
                permissions.Add(Permission.Read(Role.Any()));
                permissions.Add(Permission.Write(Role.Any()));
                permissions.Add(Permission.Delete(Role.Any()));

                var data = await _storage.CreateFile(
                    _appwriteHelper.BucketId,
                    fileName,
                    inputFile,
                    permissions);

                string path = "{endpoint}/storage/buckets/{bucketId}/files/{fileId}/view?project={projectId}"
                    .Replace("{endpoint}", _appwriteHelper.Endpoint)
                    .Replace("{bucketId}", _appwriteHelper.BucketId)
                    .Replace("{fileId}", data.Id)
                    .Replace("{projectId}", _appwriteHelper.ProjectId);

                var fileRes = _mapper.Map<FileRes>(data);
                fileRes.Url = path;

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update File
        public async Task UpdateFileAsync(string id, IFormFile file, string fileName, ImageType imageType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException(nameof(file));

                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                var isImage = _util.CheckStringContain(ext);

                int type = CheckFileType(isImage, imageType);

                var ms = new MemoryStream();

                switch (type)
                {
                    case 0: //Is Profile Picture
                        ms = await _fileService.ResizeProfileAsync(ms);
                        break;
                    case 1: //Is React Picture (Emote Content Source)
                        ms = await _fileService.ResizeReactAsync(ms);
                        break;
                    case 2: //Is Chapter Source Picture
                        ms = await _fileService.ConvertWebPAsync(ms);
                        break;
                    case 3: //Is Not Picture
                        ms = await _fileService.ConvertMarkdownAsync(ms, ext);
                        break;
                }

                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var inputFile = InputFile
                    .FromBytes(bytes, fileName, file.ContentType);

                List<string>? permissions = new List<string>();
                permissions.Add(Permission.Read(Role.Any()));
                permissions.Add(Permission.Write(Role.Any()));
                permissions.Add(Permission.Delete(Role.Any()));

                var data = await _storage.CreateFile(
                    _appwriteHelper.BucketId,
                    id,
                    inputFile,
                    permissions);

                string path = "{endpoint}/storage/buckets/{bucketId}/files/{fileId}/view?project={projectId}"
                    .Replace("{endpoint}", _appwriteHelper.Endpoint)
                    .Replace("{bucketId}", _appwriteHelper.BucketId)
                    .Replace("{fileId}", data.Id)
                    .Replace("{projectId}", _appwriteHelper.ProjectId);

                var fileRes = _mapper.Map<FileRes>(data);
                fileRes.Url = path;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Delete File
        public async Task DeleteFileAsync(string id)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int CheckFileType(bool isImage, ImageType imageType)
        {
            if (isImage && imageType.Equals(ImageType.PROFILE))
            {
                return 0;
            }
            else if (isImage && imageType.Equals(ImageType.REACT))
            {
                return 1;
            }
            else if (isImage && imageType.Equals(ImageType.SOURCE))
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }
}
