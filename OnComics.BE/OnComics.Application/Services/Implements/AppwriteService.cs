using Appwrite;
using Appwrite.Models;
using Appwrite.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnComics.Application.Helpers;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Services.Interfaces;

namespace OnComics.Application.Services.Implements
{
    public class AppwriteService : IAppwriteService
    {
        private readonly Storage _storage;
        private readonly AppwriteHelper _appwriteHelper;
        private readonly IMapper _mapper;

        public AppwriteService(
            Storage storage,
            IOptions<AppwriteHelper> appwriteHelper,
            IMapper mapper)
        {
            _storage = storage;
            _appwriteHelper = appwriteHelper.Value;
            _mapper = mapper;
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
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException(nameof(file));

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                List<string> permissions = new List<string>();
                permissions.Add(Permission.Read(Role.Any()));
                permissions.Add(Permission.Write(Role.Any()));
                permissions.Add(Permission.Delete(Role.Any()));

                var inputFile = InputFile
                    .FromBytes(bytes, fileName, file.ContentType);

                var data = await _storage.CreateFile(
                    _appwriteHelper.BucketId,
                    ID.Unique(),
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
        public async Task UpdateFileAsync(string id, IFormFile file, string fileName)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentNullException(nameof(file));

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                List<string>? permissions = new List<string>();
                permissions.Add(Permission.Read(Role.Any()));
                permissions.Add(Permission.Write(Role.Any()));
                permissions.Add(Permission.Delete(Role.Any()));

                var inputFile = InputFile
                    .FromBytes(bytes, fileName, file.ContentType);

                await _storage.DeleteFile(_appwriteHelper.BucketId, id);

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
    }
}
