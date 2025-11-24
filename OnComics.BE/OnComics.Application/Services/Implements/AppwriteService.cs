using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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
        public async Task<FileRes> CreateFileAsync(IFormFile file, string fileName)
        {
            try
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (file.ContentType.Contains("image"))
                {
                    file = await _fileService.ConvertWebPAsync(file);
                }

                if (_util.CheckWordExtension(ext))
                {
                    file = await _fileService.ConvertMarkdownAsync(file);
                }

                using var ms = new MemoryStream();
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

        //Upload Profile Picture File
        public async Task<FileRes> CreateProfileFileAsync(IFormFile file, string fileName)
        {
            try
            {
                file = await _fileService.ResizeProfileAsync(file);

                using var ms = new MemoryStream();
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

        //Upload Thumnail Picture File
        public async Task<FileRes> CreateThumbnailFileAsync(IFormFile file, string fileName)
        {
            try
            {
                file = await _fileService.ResizeThumbnailAsync(file);

                using var ms = new MemoryStream();
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

        //Upload Image Chapter Source File
        public async Task<FileRes> CreateImgSourceFileAsync(IFormFile file, string fileName)
        {
            try
            {
                file = await _fileService.ResizeImgSourceAsync(file);

                using var ms = new MemoryStream();
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


        //Upload Emote File
        public async Task<FileRes> CreateEmoteFileAsync(IFormFile file, string fileName)
        {
            try
            {
                file = await _fileService.ResizeEmoteAsync(file);

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray(); ;

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
        public async Task<FileRes> UpdateFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (file.ContentType.Contains("image"))
                {
                    file = await _fileService.ConvertWebPAsync(file);
                }

                if (_util.CheckWordExtension(ext))
                {
                    file = await _fileService.ConvertMarkdownAsync(file);
                }

                using var ms = new MemoryStream();
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

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update Profile Picture File
        public async Task<FileRes> UpdateProfileFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                file = await _fileService.ResizeProfileAsync(file);

                using var ms = new MemoryStream();
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

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update Thumnail File
        public async Task<FileRes> UpdateThumbnailFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                file = await _fileService.ResizeThumbnailAsync(file);

                using var ms = new MemoryStream();
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

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update Image Chapter Source
        public async Task<FileRes> UpdateImgSourceFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                file = await _fileService.ResizeImgSourceAsync(file);

                using var ms = new MemoryStream();
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

                return fileRes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Update Emote File
        public async Task<FileRes> UpdateEmoteFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

                file = await _fileService.ResizeProfileAsync(file);

                using var ms = new MemoryStream();
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

                return fileRes;
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
    }
}
