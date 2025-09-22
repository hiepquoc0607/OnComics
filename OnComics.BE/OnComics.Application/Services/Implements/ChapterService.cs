using Mapster;
using MapsterMapper;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Chapter;
using OnComics.Application.Models.Request.Chapter;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Chapter;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public ChapterService(
            IChapterRepository chapterRepository,
            IComicRepository comicRepository,
            IMapper mapper,
            Util util)
        {
            _chapterRepository = chapterRepository;
            _comicRepository = comicRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Chapters By Comic Id
        public async Task<ObjectResponse<IEnumerable<ChapterRes>?>> GetChaptersByComicIdAsync(GetChapterReq getChapterReq)
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

            var data = _mapper.Map<IEnumerable<ChapterRes>>(chapters);

            var totalData = await _chapterRepository.CountChapterByComicIdAsync(getChapterReq.ComicId);
            int totalPage = (int)Math.Ceiling((decimal)totalData / getChapterReq.PageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return new ObjectResponse<IEnumerable<ChapterRes>?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!",
                data,
                pagination);
        }

        //Get Chapter By Id
        public async Task<ObjectResponse<ChapterRes?>> GetChapterByIdAsync(int id)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id);

            if (chapter == null)
                return new ObjectResponse<ChapterRes?>(
                    (int)HttpStatusCode.BadRequest,
                    "Chapter Not Found!");

            var data = _mapper.Map<ChapterRes>(chapter);

            return new ObjectResponse<ChapterRes?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!", data);
        }

        //Create Chapter
        public async Task<ObjectResponse<Chapter>> CreateChapterAsync(CreateChapterReq createChapterReq)
        {
            var isComicExist = await _comicRepository.CheckComicIdAsync(createChapterReq.ComicId);

            if (!isComicExist)
                return new ObjectResponse<Chapter>(
                    (int)HttpStatusCode.NotFound,
                    "Comic Not Found!");

            var newChapter = _mapper.Map<Chapter>(createChapterReq);
            newChapter.ChapNo = await _chapterRepository.GetMaxChapNoByComicIdAsync(createChapterReq.ComicId) + 1;

            if (!string.IsNullOrEmpty(newChapter.Name))
                newChapter.Name = _util.FormatStringName(newChapter.Name);

            try
            {
                await _chapterRepository.InsertAsync(newChapter);

                return new ObjectResponse<Chapter>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Successfully!",
                    newChapter);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Chapter>(
                    (int)HttpStatusCode.BadRequest,
                    "Create Chapter Fail!, Message Error:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Bulk (Range) Create Chapters
        public async Task<ObjectResponse<IEnumerable<Chapter>>> CreateChaptersAsync(List<CreateChapterReq> chapters)
        {
            if (chapters.Count > 10)
                return new ObjectResponse<IEnumerable<Chapter>>(
                    (int)HttpStatusCode.BadRequest,
                    "Only Create Max 10 Record At Once!");

            int[] ids = chapters.Select(c => c.ComicId).ToArray();
            int[] dataIds = await _comicRepository.GetComicIdsAsync();
            int[] nonIds = _util.CompareIntArray(ids, dataIds);

            if (nonIds.Length > 0)
                return new ObjectResponse<IEnumerable<Chapter>>(
                    (int)HttpStatusCode.NotFound,
                    "Comics Not Found!, IDs: " + string.Join(", ", nonIds));

            var chapNos = await _chapterRepository.GetMaxChapNosByComicIdsAsync(ids);

            var newChapters = chapters.Adapt<IEnumerable<Chapter>>();

            foreach (var items in newChapters)
            {
                if (!string.IsNullOrEmpty(items.Name))
                    items.Name = _util.FormatStringName(items.Name);

                items.ChapNo = chapNos[items.ComicId] + 1;
            }

            try
            {
                await _chapterRepository.InsertRangeAsync(newChapters);

                return new ObjectResponse<IEnumerable<Chapter>>(
                    (int)HttpStatusCode.OK,
                    "Create Chapters Successfully!",
                    newChapters);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Chapter>>(
                    (int)HttpStatusCode.BadRequest,
                    "Create Chapters Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Chapter
        public async Task<VoidResponse> UpdateChapterAsync(int id, UpdateChapterReq updateChapterReq)
        {
            var oldChapter = await _chapterRepository.GetByIdAsync(id, true);

            if (oldChapter == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Chapter Not Found!");

            var newChapter = _mapper.Map(updateChapterReq, oldChapter);

            if (!string.IsNullOrEmpty(newChapter.Name))
                newChapter.Name = _util.FormatStringName(newChapter.Name);

            try
            {
                await _chapterRepository.UpdateAsync(newChapter);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Update Chapter Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Chapter Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<ChapterStatus> updateStatusReq)
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

            try
            {
                await _chapterRepository.UpdateAsync(chapter);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Update Status Fail!, Message Error:\n\n" + ex);
            }
        }

        //Delete Chapter
        public async Task<VoidResponse> DeleteChapterAsync(int id)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id);

            if (chapter == null) return new VoidResponse(
                (int)HttpStatusCode.NotFound,
                "Chapter Not Found!");

            try
            {
                await _chapterRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Delete Chapter Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
