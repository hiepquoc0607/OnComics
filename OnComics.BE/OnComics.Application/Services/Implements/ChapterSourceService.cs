using Mapster;
using MapsterMapper;
using OnComics.Application.Models.Request.ChapterSource;
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
    public class ChapterSourceService : IChapterSourceService
    {
        private readonly IChapterSourceRepository _chapterSourceRepository;
        private readonly IMapper _mapper;
        private readonly Util _utils;

        public ChapterSourceService(
            IChapterSourceRepository chapterSourceRepository,
            IMapper mapper,
            Util util)
        {
            _chapterSourceRepository = chapterSourceRepository;
            _mapper = mapper;
            _utils = util;
        }

        //Get All Chapter Sources
        public async Task<ObjectResponse<IEnumerable<ChapterSourceRes>?>> GetChapterSourcesAsync(GetChapterSourceReq getChapterSourceReq)
        {
            try
            {
                int searchId = getChapterSourceReq.ChapterId;

                int pageIndex = getChapterSourceReq.PageIndex;
                int pageNum = getChapterSourceReq.PageNum;

                Expression<Func<Chaptersource, bool>>? search = s => s.ChapterId == searchId;

                Func<IQueryable<Chaptersource>, IOrderedQueryable<Chaptersource>>? order = s =>
                    s.OrderBy(s => s.Arrangement);

                var srcs = await _chapterSourceRepository.GetAsync(search, order, pageNum, pageIndex);

                if (srcs == null)
                    return new ObjectResponse<IEnumerable<ChapterSourceRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Source Data Empty!");

                var data = srcs.Adapt<IEnumerable<ChapterSourceRes>>();

                var totalData = await _chapterSourceRepository.CountSourceByChapterId(searchId);
                int totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<ChapterSourceRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<ChapterSourceRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Chapter Source
        public async Task<ObjectResponse<Chaptersource>> CreateChapterSourceAsync(CreateChapterSourceReq createChapterSourceReq)
        {
            try
            {
                bool isExisted = await _chapterSourceRepository
                    .CheckChapterSourceAsync(
                        createChapterSourceReq.ChapterId,
                        createChapterSourceReq.Arrangement);

                if (isExisted)
                    return new ObjectResponse<Chaptersource>(
                        (int)HttpStatusCode.BadRequest,
                        "Chapter Source Is Existed!");

                var newSrc = _mapper.Map<Chaptersource>(createChapterSourceReq);

                await _chapterSourceRepository.InsertAsync(newSrc);

                return new ObjectResponse<Chaptersource>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Source Sucessfully!",
                    newSrc);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Chaptersource>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Bulk Create Range Chapter Sources
        public async Task<ObjectResponse<IEnumerable<Chaptersource>>> CreateRangeChapterSourcesAsync(List<CreateChapterSourceReq> sources)
        {
            try
            {
                if (sources.Count > 10)
                    return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 10 Record At Once!");

                var srcs = sources.ToDictionary(s => s.ChapterId, s => s.Arrangement);
                var dataSrcs = await _chapterSourceRepository.GetChapterSourcesAsync();
                var existedSrcs = _utils.CompareIntDictionary(dataSrcs, srcs);

                if (existedSrcs != null)
                    return new ObjectResponse<IEnumerable<Chaptersource>>(
                        (int)HttpStatusCode.BadRequest,
                        "Chapter Sources Are Existed, Record:\n" + existedSrcs);

                var newSrcs = sources.Adapt<IEnumerable<Chaptersource>>();

                await _chapterSourceRepository.BulkInsertRangeAsync(newSrcs);

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

        //Update Chapter Source
        public async Task<VoidResponse> UpdateChapterSourceAsync(int id, UpdateChapterSourceReq updateChapterSourceReq)
        {
            try
            {
                var src = await _chapterSourceRepository.GetByIdAsync(id, true);

                if (src == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Source Not Found!");

                var newSrc = _mapper.Map(updateChapterSourceReq, src);

                await _chapterSourceRepository.UpdateAsync(newSrc);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Chapter Source Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Chapter Source
        public async Task<VoidResponse> DeleteChapterSourceAsync(int id)
        {
            try
            {
                var src = await _chapterSourceRepository.GetByIdAsync(id);

                if (src == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Source Not Found!");

                await _chapterSourceRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Source Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Bulk Delete Range Chapter Sources
        public async Task<VoidResponse> DeleteRangeChapterSourcesAsync(int chapterId)
        {
            try
            {
                var srcs = await _chapterSourceRepository
                    .GetSourcesByChapterIdAsync(chapterId);

                if (srcs == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Chapter Sources Not Found!");

                await _chapterSourceRepository.BulkDeleteRangeAsync(srcs);

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
