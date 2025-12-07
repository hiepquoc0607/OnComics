using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Hubs;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Models.Response.Chapter;
using OnComics.Application.Models.Response.ChapterSource;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IChapterSourceRepository _chapterSourceRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IHistoryRepository _historyRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAppwriteService _appwriteService;
        private readonly IRedisService _redisService;
        private readonly IHubContext<OnComicsHub> _hub;
        private readonly IMapper _mapper;
        private readonly Util _util;

        private static string cacheKey = "chapters:{id}";

        public ChapterService(
            IChapterRepository chapterRepository,
            IChapterSourceRepository chapterSourceRepository,
            IComicRepository comicRepository,
            IHistoryRepository historyRepository,
            INotificationRepository notificationRepository,
            IAppwriteService appwriteService,
            IRedisService redisService,
            IHubContext<OnComicsHub> hub,
            IMapper mapper,
            Util util)
        {
            _chapterRepository = chapterRepository;
            _chapterSourceRepository = chapterSourceRepository;
            _comicRepository = comicRepository;
            _historyRepository = historyRepository;
            _notificationRepository = notificationRepository;
            _appwriteService = appwriteService;
            _redisService = redisService;
            _hub = hub;
            _mapper = mapper;
            _util = util;
        }

        //Get All Chapters By Comic Id
        public async Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersAsync(GetChapterReq getChapterReq)
        {
            try
            {
                string? status = getChapterReq.Status switch
                {
                    ChapterStatus.ACTIVE => StatusConstant.ACTIVE,
                    ChapterStatus.INACTIVE => StatusConstant.INACTIVE,
                    ChapterStatus.ERROR => StatusConstant.ERROR,
                    _ => null
                };

                bool isDescending = getChapterReq.IsDescending;

                int pageNum = getChapterReq.PageNum;
                int pageIndex = getChapterReq.PageIndex;


                Expression<Func<Chapter, bool>>? search = c =>
                    c.ComicId == getChapterReq.ComicId &&
                    (string.IsNullOrEmpty(status) || c.Status.Equals(status));

                Func<IQueryable<Chapter>, IOrderedQueryable<Chapter>>? order = c => isDescending switch
                {
                    true => c.OrderByDescending(c => c.ChapNo),
                    _ => c.OrderBy(c => c.ChapNo),
                };

                var isComicExisted = await _comicRepository.CheckComicIdAsync(getChapterReq.ComicId);

                if (!isComicExisted)
                    return new ObjectResponse<IEnumerable<ChapterRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                var chapters = await _chapterRepository.GetAsync(search, order, pageNum, pageIndex);

                if (chapters == null)
                    return new ObjectResponse<IEnumerable<ChapterRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Has No Chapter Data!");

                var data = chapters.Adapt<IEnumerable<ChapterRes>>();

                var totalData = await _chapterRepository.CountChapterByComicIdAsync(getChapterReq.ComicId);
                int totalPage = (int)Math.Ceiling((decimal)totalData / getChapterReq.PageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<ChapterRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<ChapterRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Chapter By Id
        public async Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(Guid id, Guid accId)
        {
            var comic = new Comic();
            var chapter = new Chapter();
            var history = new History();

            try
            {
                string key = cacheKey.Replace("{id}", id.ToString());

                var chapCache = await _redisService.GetAsync<ChapterRes?>(key);

                if (chapCache is not null)
                {
                    return new ObjectResponse<ChapterRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        chapCache);
                }
                else
                {
                    var (chap, sources) = await _chapterRepository.GetChapterByIdAsync(id);

                    chapter = chap;

                    if (chapter == null)
                        return new ObjectResponse<ChapterRes?>(
                            (int)HttpStatusCode.BadRequest,
                            "Chapter Not Found!");

                    comic = await _comicRepository.GetByIdAsync(chapter.ComicId, true);

                    if (comic == null)
                        return new ObjectResponse<ChapterRes?>(
                            (int)HttpStatusCode.BadRequest,
                            "Comic Not Found!");

                    comic.TotalReadNum = comic.TotalReadNum + 1;
                    chapter.ReadNum = chapter.ReadNum + 1;

                    await _chapterRepository.UpdateAsync(chapter);

                    await _comicRepository.UpdateAsync(comic);

                    history = new History
                    {
                        Id = Guid.NewGuid(),
                        ChapterId = id,
                        AccountId = accId,
                        ReadTime = DateTime.UtcNow,
                    };

                    await _historyRepository.InsertAsync(history);

                    var data = _mapper.Map<ChapterRes>(chapter);
                    data.Chaptersources = sources.Adapt<List<ChapterSourceRes>>();

                    await _redisService.SetAsync<ChapterRes?>(key, data, TimeSpan.FromMinutes(10));

                    return new ObjectResponse<ChapterRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data);
                }
            }
            catch (Exception ex)
            {
                if (comic != null)
                {
                    comic.TotalReadNum = comic.TotalReadNum - 1;
                    chapter.ReadNum = chapter.ReadNum - 1;

                    await _chapterRepository.UpdateAsync(chapter);

                    await _comicRepository.UpdateAsync(comic);

                    await _historyRepository.DeleteAsync(history);
                }

                return new ObjectResponse<ChapterRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Chapter
        public async Task<ObjectResponse<Chapter>> CreateChapterAsync(List<IFormFile> files, CreateChapterReq createChapterReq)
        {
            Guid chapId = Guid.NewGuid();
            var newChapter = new Chapter();
            var chapSrcs = new List<Chaptersource>();

            try
            {
                if (files.IsNullOrEmpty())
                    return new ObjectResponse<Chapter>(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (files.Count > 5)
                    return new ObjectResponse<Chapter>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 5 Record At Once!");

                const long maxFileSize = 2 * 1024 * 1024; // 2MB 

                var comic = await _comicRepository.GetByIdAsync(createChapterReq.ComicId, true);

                if (comic == null)
                    return new ObjectResponse<Chapter>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                newChapter = _mapper.Map<Chapter>(createChapterReq);
                newChapter.Id = chapId;
                newChapter.ChapNo = await _chapterRepository
                    .GetMaxChapNoByComicIdAsync(createChapterReq.ComicId) + 1;

                if (!string.IsNullOrEmpty(newChapter.Name))
                    newChapter.Name = _util.FormatStringName(newChapter.Name);

                var fileRes = new FileRes();

                for (int i = 1; i <= files.Count; i++)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > maxFileSize)
                            return new ObjectResponse<Chapter>(
                            (int)HttpStatusCode.BadRequest,
                            "Max Size Per File Is 2MB!");

                        if ((comic.IsNovel && file.ContentType.Contains("image")) ||
                            (!comic.IsNovel && !file.ContentType.Contains("image")))
                            return new ObjectResponse<Chapter>(
                                (int)HttpStatusCode.BadRequest,
                                "Invalid Source Format!");

                        var chapSrc = new Chaptersource();
                        chapSrc.ChapterId = chapId;
                        chapSrc.Id = Guid.NewGuid();
                        chapSrc.Arrangement = i;

                        if (comic.IsNovel)
                        {
                            fileRes = await _appwriteService
                                .CreateFileAsync(file, chapSrc.Id.ToString());

                            chapSrc.IsImage = false;
                        }
                        else
                        {
                            fileRes = await _appwriteService
                                .CreateImgSourceFileAsync(file, chapSrc.Id.ToString());

                            chapSrc.IsImage = true;
                        }

                        chapSrc.SrcUrl = fileRes.Url;

                        chapSrcs.Add(chapSrc);
                    }
                }

                await _chapterRepository.InsertAsync(newChapter);

                comic.ChapNum = comic.ChapNum + 1;
                comic.UpdateTime = DateTime.UtcNow;

                await _comicRepository.UpdateAsync(comic);

                await _chapterSourceRepository.BulkInsertAsync(chapSrcs);

                newChapter.Chaptersources = chapSrcs;

                return new ObjectResponse<Chapter>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Successfully!",
                    newChapter);
            }
            catch (Exception ex)
            {
                if (newChapter != null)
                {
                    await _chapterRepository.DeleteAsync(newChapter);

                    foreach (var file in chapSrcs)
                    {
                        await _appwriteService.DeleteFileAsync(file.Id.ToString());
                    }
                }

                return new ObjectResponse<Chapter>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Chapter
        public async Task<VoidResponse> UpdateChapterAsync(Guid id, UpdateChapterReq updateChapterReq)
        {
            try
            {
                var oldChapter = await _chapterRepository.GetByIdAsync(id, true);

                if (oldChapter == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Not Found!");

                var newChapter = _mapper.Map(updateChapterReq, oldChapter);

                if (!string.IsNullOrEmpty(newChapter.Name))
                    newChapter.Name = _util.FormatStringName(newChapter.Name);

                await _chapterRepository.UpdateAsync(newChapter);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Chapter Status
        public async Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<ChapterStatus> updateStatusReq)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdAsync(id, true);

                if (chapter == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Not Found!");

                chapter.Status = updateStatusReq.Status switch
                {
                    ChapterStatus.ACTIVE => StatusConstant.ACTIVE,
                    ChapterStatus.INACTIVE => StatusConstant.INACTIVE,
                    _ => StatusConstant.ERROR
                };

                await _chapterRepository.UpdateAsync(chapter);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Chapter
        public async Task<VoidResponse> DeleteChapterAsync(Guid id)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdAsync(id, true);

                if (chapter == null) return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Chapter Not Found!");

                var comic = await _comicRepository.GetByIdAsync(chapter.ComicId, true);

                if (comic == null) return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Comic Not Found!");

                await _chapterRepository.DeleteAsync(chapter);

                comic.ChapNum = comic.ChapNum - 1;
                comic.UpdateTime = DateTime.UtcNow;

                await _comicRepository.UpdateAsync(comic);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Error Report A Chapter
        public async Task<ObjectResponse<Notification>> ReportChapterAsync(Guid id)
        {
            try
            {
                var chapter = await _chapterRepository.GetByIdAsync(id, false);

                if (chapter == null)
                    return new ObjectResponse<Notification>(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Not Found!");

                var noti = new Notification();
                noti.Id = Guid.NewGuid();
                noti.ChapterId = id;
                noti.Content = $"ChapterId: {id} Error Report!";
                noti.SendTime = DateTime.Now;
                noti.IsRead = false;

                await _notificationRepository.InsertAsync(noti);

                await _hub.Clients.All.SendAsync("NotificationCreated", noti);

                return new ObjectResponse<Notification>(
                    (int)HttpStatusCode.Created,
                    "Chapter Error Report Successfully!",
                    noti);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Notification>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
