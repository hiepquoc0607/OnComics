using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Comic;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Comic;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ComicService : IComicService
    {
        private readonly IComicRepository _comicRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public ComicService(
            IComicRepository comicRepository,
            IMapper mapper,
            Util util)
        {
            _comicRepository = comicRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Comics
        public async Task<ObjectResponse<IEnumerable<ComicRes>?>> GetComicsAsync(GetComicReq getComicReq)
        {
            try
            {
                string? searchKey = getComicReq.SearchKey;

                string? status = getComicReq.Status switch
                {
                    ComicStatus.ONGOING => StatusConstant.ONGOING,
                    ComicStatus.UPCOMING => StatusConstant.UPCOMING,
                    ComicStatus.FINISHED => StatusConstant.FINISHED,
                    _ => null
                };

                bool isDescending = getComicReq.IsDescending;

                int pageNum = getComicReq.PageNum;
                int pageIndex = getComicReq.PageIndex;


                Expression<Func<Comic, bool>>? search = c =>
                    (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(c.Name, $"%{searchKey}%")) &&
                    (string.IsNullOrEmpty(status) || c.Status.Equals(status));

                Func<IQueryable<Comic>, IOrderedQueryable<Comic>>? order = a => getComicReq.SortBy switch
                {
                    ComicSortOption.NAME => isDescending
                        ? a.OrderByDescending(a => a.Name)
                        : a.OrderBy(a => a.Name),
                    ComicSortOption.AUTHOR => isDescending
                        ? a.OrderByDescending(a => a.Author)
                        : a.OrderBy(a => a.Author),
                    ComicSortOption.RELEASE => isDescending
                        ? a.OrderByDescending(a => a.ReleaseDate)
                        : a.OrderBy(a => a.ReleaseDate),
                    ComicSortOption.UPDATE => isDescending
                        ? a.OrderByDescending(a => a.UpdateTime)
                        : a.OrderBy(a => a.UpdateTime),
                    ComicSortOption.RATING => isDescending
                        ? a.OrderByDescending(a => a.Rating)
                        : a.OrderBy(a => a.Rating),
                    ComicSortOption.FAVORITE => isDescending
                        ? a.OrderByDescending(a => a.FavoriteNum)
                        : a.OrderBy(a => a.FavoriteNum),
                    ComicSortOption.DATE => isDescending
                        ? a.OrderByDescending(a => a.DayReadNum)
                        : a.OrderBy(a => a.DayReadNum),
                    ComicSortOption.WEEK => isDescending
                        ? a.OrderByDescending(a => a.WeekReadNum)
                        : a.OrderBy(a => a.WeekReadNum),
                    ComicSortOption.MONTH => isDescending
                        ? a.OrderByDescending(a => a.MonthReadNum)
                        : a.OrderBy(a => a.MonthReadNum),
                    ComicSortOption.TOTAL => isDescending
                        ? a.OrderByDescending(a => a.TotalReadNum)
                        : a.OrderBy(a => a.TotalReadNum),
                    ComicSortOption.NOVEL => isDescending
                        ? a.OrderByDescending(a => a.IsNovel)
                        : a.OrderBy(a => a.IsNovel),
                    ComicSortOption.STATUS => isDescending
                        ? a.OrderByDescending(a => a.Status)
                        : a.OrderBy(a => a.Status),
                    _ => a.OrderBy(a => a.Id)
                };

                var comics = await _comicRepository.GetAsync(search, order, pageNum, pageIndex);

                if (comics == null)
                    return new ObjectResponse<IEnumerable<ComicRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Data Empty!");

                var data = comics.Adapt<IEnumerable<ComicRes>>();

                var totalData = await _comicRepository.CountRecordAsync();
                int totalPage = (int)Math.Ceiling((decimal)totalData / getComicReq.PageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<ComicRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<ComicRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Comic By Id
        public async Task<ObjectResponse<ComicRes?>> GetComicByIdAsync(Guid id)
        {
            try
            {
                var comic = await _comicRepository.GetByIdAsync(id, false);

                if (comic == null)
                    return new ObjectResponse<ComicRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                var data = comic.Adapt<ComicRes>();

                return new ObjectResponse<ComicRes?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<ComicRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Comic
        public async Task<ObjectResponse<Comic>> CreateComicAsync(CreateComicReq createComicReq)
        {
            try
            {
                var IsComicExisted = await _comicRepository.CheckComicExistedAsync(createComicReq.Name, createComicReq.Author);

                if (IsComicExisted)
                    return new ObjectResponse<Comic>(
                        (int)HttpStatusCode.BadRequest,
                        "Comic Is Existed!");

                var newComic = _mapper.Map<Comic>(createComicReq);

                await _comicRepository.InsertAsync(newComic);

                return new ObjectResponse<Comic>(
                    (int)HttpStatusCode.Created,
                    "Create Comic Successfully!",
                    newComic);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Comic>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Comic
        public async Task<VoidResponse> UpdateComicAsync(Guid id, UpdateComicReq updateComicReq)
        {
            try
            {
                var oldComic = await _comicRepository.GetByIdAsync(id, true);

                if (oldComic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                var newComic = _mapper.Map(updateComicReq, oldComic);
                newComic.Name = _util.FormatStringName(updateComicReq.Name);

                await _comicRepository.UpdateAsync(newComic);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Comic Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Comic Status
        public async Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<ComicStatus> updateStatusReq)
        {
            try
            {
                var comic = await _comicRepository.GetByIdAsync(id, true);

                if (comic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                comic.Status = updateStatusReq.Status switch
                {
                    ComicStatus.ONGOING => StatusConstant.ONGOING,
                    ComicStatus.UPCOMING => StatusConstant.UPCOMING,
                    _ => StatusConstant.FINISHED
                };

                await _comicRepository.UpdateAsync(comic);

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

        //Delete Comic
        public async Task<VoidResponse> DeleteComicAsync(Guid id)
        {
            try
            {
                var comic = await _comicRepository.GetByIdAsync(id, true);

                if (comic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                await _comicRepository.DeleteAsync(comic);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Comic Successfully!");
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
