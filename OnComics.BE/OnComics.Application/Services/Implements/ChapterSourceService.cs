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

        //Create Chapter Source
        public async Task<ObjectResponse<Chaptersource>> CreateChapterSourceAsync(CreateChapterSourceReq createChapterSourceReq)
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

            try
            {
                await _chapterSourceRepository.InsertAsync(newSrc);

                return new ObjectResponse<Chaptersource>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Source Sucessfully!",
                    newSrc);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Chaptersource>(
                    (int)HttpStatusCode.BadRequest,
                    "Create Chapter Source Fail!, Error Message:\n\n" + ex);
            }
        }

        //Bulk (Range) Create Chapter Sources
        public async Task<ObjectResponse<IEnumerable<Chaptersource>>> CreateChapterSourcesAsync(List<CreateChapterSourceReq> sources)
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

            try
            {
                await _chapterSourceRepository.InsertRangeAsync(newSrcs);

                return new ObjectResponse<IEnumerable<Chaptersource>>(
                    (int)HttpStatusCode.Created,
                    "Create Chapter Sources Successfully!",
                    newSrcs);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Chaptersource>>(
                    (int)HttpStatusCode.BadRequest,
                    "Create Chapter Sources Fail!,Error Message:\n\n" + ex);
            }
        }

        //Update Chapter Source
        public async Task<VoidResponse> UpdateChapterSourceAsync(int id, UpdateChapterSourceReq updateChapterSourceReq)
        {
            var src = await _chapterSourceRepository.GetByIdAsync(id, true);

            if (src == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Chapter Source Not Found!");

            var newSrc = _mapper.Map(updateChapterSourceReq, src);

            try
            {
                await _chapterSourceRepository.UpdateAsync(newSrc);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Chapter Source Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Update Chapter Source Fail!, Error Message:\n\n" + ex);
            }
        }

        //Delete Chapter Source
        public async Task<VoidResponse> DeleteChapterSourceAsync(int id)
        {
            var src = await _chapterSourceRepository.GetByIdAsync(id);

            if (src == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Chapter Source Not Found!");

            try
            {
                await _chapterSourceRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Source Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Delete Chapter Source Fail!, Error Message:\n\n" + ex);
            }
        }

        //Bulk (Range) Delete Chapter Sources
        public async Task<VoidResponse> DeleteChapterSourcesAsync(int chapterId)
        {
            var srcs = await _chapterSourceRepository
                .GetSourcesByChapterIdAsync(chapterId);

            if (srcs == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Chapter Sources Not Found!");

            try
            {
                await _chapterSourceRepository.DeleteRangeAsync(srcs);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Chapter Sources Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Delete Chapter Sources Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
