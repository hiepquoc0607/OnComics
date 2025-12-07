using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ChapterSourceService : IChapterSourceService
    {
        private readonly IChapterSourceRepository _chapterSourceRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IAppwriteService _appwriteService;
        private readonly IRedisService _redisService;

        private static string cacheKey = "chapters:{id}";

        public ChapterSourceService(
            IChapterSourceRepository chapterSourceRepository,
            IChapterRepository chapterRepository,
            IComicRepository comicRepository,
            IAppwriteService appwriteService,
            IRedisService redisService)
        {
            _chapterSourceRepository = chapterSourceRepository;
            _chapterRepository = chapterRepository;
            _comicRepository = comicRepository;
            _appwriteService = appwriteService;
            _redisService = redisService;
        }

        //Update Chapter Sources
        public async Task<ObjectResponse<IEnumerable<Chaptersource>>> UpdateChapterSourceAsync(Guid chapterId, List<IFormFile> files)
        {
            try
            {
                if (files.IsNullOrEmpty())
                    return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (files.Count > 5)
                    return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 5 Record At Once!");

                const long maxFileSize = 2 * 1024 * 1024; // 2MB 

                foreach (var file in files)
                {
                    if (file.Length > maxFileSize)
                        return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.BadRequest,
                        "Max Size Per File Is 2MB!");
                }

                var chapter = await _chapterRepository.GetByIdAsync(chapterId, false);

                if (chapter == null)
                    return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Not Found!");

                var comic = await _comicRepository.GetByIdAsync(chapter.ComicId, false);

                var oldSrcs = await _chapterSourceRepository
                    .GetSourcesByChapterIdAsync(chapterId);

                if (oldSrcs != null)
                {
                    await _chapterSourceRepository.BulkDeleteAsync(oldSrcs);

                    Guid[] oldSrcIds = oldSrcs.Select(s => s.Id).ToArray();

                    foreach (var id in oldSrcIds)
                    {
                        await _appwriteService.DeleteFileAsync(id.ToString());
                    }
                }

                var fileRes = new FileRes();
                var newSrcs = new List<Chaptersource>();

                for (int i = 1; i < files.Count; i++)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > maxFileSize)
                            return new ObjectResponse<IEnumerable<Chaptersource>>(
                            (int)HttpStatusCode.BadRequest,
                            "Max Size Per File Is 2MB!");

                        if ((comic!.IsNovel == true && file.ContentType.Contains("image")) ||
                            (comic!.IsNovel == false && !file.ContentType.Contains("image")))
                            return new ObjectResponse<IEnumerable<Chaptersource>>(
                                (int)HttpStatusCode.BadRequest,
                                "Invalid Source Format!");

                        var chapSrc = new Chaptersource();
                        chapSrc.ChapterId = chapterId;
                        chapSrc.Id = Guid.NewGuid();
                        chapSrc.Arrangement = i;

                        if (file.ContentType.Contains("image"))
                        {
                            fileRes = await _appwriteService
                                .CreateImgSourceFileAsync(file, chapSrc.Id.ToString());

                            chapSrc.IsImage = true;
                        }
                        else
                        {
                            fileRes = await _appwriteService
                                .CreateFileAsync(file, chapSrc.Id.ToString());

                            chapSrc.IsImage = false;
                        }

                        chapSrc.SrcUrl = fileRes.Url;

                        newSrcs.Add(chapSrc);
                    }
                }

                await _chapterSourceRepository.BulkInsertAsync(newSrcs);

                string key = cacheKey.Replace("{id}", chapterId.ToString());

                await _redisService.RemoveAsync(key);

                return new ObjectResponse<IEnumerable<Chaptersource>>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Sources Successfully!",
                    newSrcs);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Chaptersource>>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        // Delete Chapter Sources By Chapter Id
        public async Task<VoidResponse> DeleteChapterSourcesAsync(Guid chapterId)
        {
            try
            {
                var srcs = await _chapterSourceRepository
                    .GetSourcesByChapterIdAsync(chapterId);

                if (srcs == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Sources Not Found!");

                await _chapterSourceRepository.BulkDeleteAsync(srcs);

                foreach (var file in srcs)
                {
                    await _appwriteService.DeleteFileAsync(file.Id.ToString());
                }

                string key = cacheKey.Replace("{id}", chapterId.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Sources Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
