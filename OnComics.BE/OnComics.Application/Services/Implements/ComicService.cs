using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Comic;
using OnComics.Application.Models.Request.Comic;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Appwrite;
using OnComics.Application.Models.Response.Category;
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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IComicCategoryRepository _comicCategoryRepository;
        private readonly IAppwriteService _appwriteService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Util _util;

        public ComicService(
            IComicRepository comicRepository,
            ICategoryRepository categoryRepository,
            IComicCategoryRepository comicCategoryRepository,
            IAppwriteService appwriteService,
            IMapper mapper,
            IConfiguration configuration,
            Util util)
        {
            _comicRepository = comicRepository;
            _categoryRepository = categoryRepository;
            _comicCategoryRepository = comicCategoryRepository;
            _appwriteService = appwriteService;
            _mapper = mapper;
            _configuration = configuration;
            _util = util;
        }

        //Get All Comics
        public async Task<ObjectResponse<IEnumerable<ComicRes>?>> GetComicsAsync(GetComicReq getComicReq)
        {
            try
            {
                string? searchKey = getComicReq.SearchKey;

                Guid? categoryId = getComicReq.CategoryId;

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

                var comics = await _comicRepository
                    .GetComicsAsync(categoryId, search, order, pageNum, pageIndex);

                if (comics == null)
                    return new ObjectResponse<IEnumerable<ComicRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Data Empty!");

                var data = comics.Adapt<IEnumerable<ComicRes>>();
                int totalData = 0;

                if (categoryId.HasValue)
                {
                    totalData = await _comicRepository.CountComicsByCateId(categoryId.Value);
                }
                else
                {
                    totalData = await _comicRepository.CountRecordAsync(search);
                }

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
                var (comic, categories) = await _comicRepository.GetComicByIdAsync(id);

                if (comic == null)
                    return new ObjectResponse<ComicRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                var data = _mapper.Map<ComicRes>(comic);
                data.Categories = categories.Adapt<List<CateNameRes>>(); ;

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
            Guid comicId = Guid.Empty;

            try
            {
                var IsComicExisted = await _comicRepository
                    .CheckComicExistedAsync(createComicReq.Name, createComicReq.Author);

                if (IsComicExisted)
                    return new ObjectResponse<Comic>(
                        (int)HttpStatusCode.BadRequest,
                        "Comic Is Existed!");

                Guid[] cateIds = createComicReq.Categories.ToArray();
                Guid[] dataIds = await _categoryRepository.GetCateIdsAsync();
                var isCateExisted = _util.CheckGuidArray(cateIds, dataIds);

                if (isCateExisted == false)
                    return new ObjectResponse<Comic>(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Category!");

                var newComic = _mapper.Map<Comic>(createComicReq);
                newComic.Name = _util.FormatStringName(createComicReq.Name);
                newComic.Author = _util.FormatStringName(createComicReq.Author);

                var newCates = new List<Comiccategory>();
                var cates = createComicReq.Categories;

                await _comicRepository.InsertAsync(newComic);

                comicId = newComic.Id;

                foreach (var item in cates)
                {
                    newCates.Add(new Comiccategory
                    {
                        Id = Guid.NewGuid(),
                        ComicId = comicId,
                        CategoryId = item
                    });
                }

                await _comicCategoryRepository.BulkInsertAsync(newCates);

                return new ObjectResponse<Comic>(
                    (int)HttpStatusCode.Created,
                    "Create Comic Successfully!",
                    newComic);
            }
            catch (Exception ex)
            {
                var comic = await _comicRepository.GetByIdAsync(comicId, true);

                if (comic != null)
                    await _comicRepository.DeleteAsync(comic);

                return new ObjectResponse<Comic>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Comic
        public async Task<VoidResponse> UpdateComicAsync(Guid id, UpdateComicReq updateComicReq)
        {
            var tempData = new Comic();

            try
            {
                var oldComic = await _comicRepository.GetByIdAsync(id, true);

                if (oldComic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                tempData = oldComic;

                Guid[] cateIds = updateComicReq.Categories.ToArray();
                Guid[] dataIds = await _categoryRepository.GetCateIdsAsync();
                var isCateExisted = _util.CheckGuidArray(cateIds, dataIds);

                if (isCateExisted == false)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Category!");

                var newComic = _mapper.Map(updateComicReq, oldComic);
                newComic.Name = _util.FormatStringName(updateComicReq.Name);
                newComic.Author = _util.FormatStringName(updateComicReq.Author);

                var cates = updateComicReq.Categories;
                var newCates = new List<Comiccategory>();

                await _comicRepository.UpdateAsync(newComic);

                await _comicCategoryRepository.DeleteComicCateoriesAsync(id);

                foreach (var item in cates)
                {
                    newCates.Add(new Comiccategory
                    {
                        ComicId = newComic.Id,
                        CategoryId = item
                    });
                }

                await _comicCategoryRepository.BulkInsertAsync(newCates);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Comic Successfully!");
            }
            catch (Exception ex)
            {
                await _comicRepository.UpdateAsync(tempData);

                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Comic Thumnail
        public async Task<VoidResponse> UpdateThumbnailAsync(Guid id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (!file.ContentType.Contains("image"))
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Picture File Format!");

                var oldComic = await _comicRepository.GetByIdAsync(id, true);

                if (oldComic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                string? imgUrl = oldComic.ThumbnailUrl;
                string fileName = oldComic.Id.ToString();

                var fileRes = new FileRes();

                if (string.IsNullOrEmpty(imgUrl) ||
                    imgUrl.Equals(_configuration["AppReturnUrl:DefaultProfileUrl"]))
                {
                    fileRes = await _appwriteService
                        .CreateThumbnailFileAsync(file, fileName);
                }

                fileRes = await _appwriteService
                    .UpdateThumbnailFileAsync(oldComic.Id.ToString(), file, fileName);

                oldComic.ThumbnailUrl = fileRes.Url;

                await _comicRepository.UpdateAsync(oldComic);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Thumnail Picture Successfully!");
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
