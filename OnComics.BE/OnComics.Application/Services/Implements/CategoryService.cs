using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Response.Category;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRedisService _redisService;
        private readonly IMapper _mapper;

        private static string cacheKey = "categories";

        public CategoryService(
            ICategoryRepository categoryRepository,
            IRedisService redisService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _redisService = redisService;
            _mapper = mapper;
        }

        //Get All Categoriess
        public async Task<ObjectResponse<IEnumerable<CategoryRes>?>> GetCategoriesAsync(GetCategoryReq getCategoryReq)
        {
            try
            {
                string? searchKey = getCategoryReq.SearchKey;

                bool isDescending = getCategoryReq.IsDescending;

                int pageNum = getCategoryReq.PageNum;
                int pageIndex = getCategoryReq.PageIndex;

                var cateCache = await _redisService.GetAsync<IEnumerable<CategoryRes>?>(cacheKey);

                if (cateCache is not null)
                {
                    var query = cateCache.AsQueryable();

                    if (!string.IsNullOrEmpty(searchKey))
                        query = query.Where(c => EF.Functions.Like(c.Name, $"%{searchKey}%"));

                    query = getCategoryReq.SortBy switch
                    {
                        CategorySortOption.NAME => isDescending
                            ? query.OrderByDescending(c => c.Name)
                            : query.OrderBy(c => c.Name),
                        _ => query.OrderBy(c => c.Id)
                    };

                    var categories = await query
                        .Skip((pageNum - 1) * pageIndex)
                        .Take(pageIndex)
                        .ToListAsync();

                    var totalData = cateCache.Count();
                    int totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                    var data = categories.Adapt<IEnumerable<CategoryRes>>();

                    var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                    return new ObjectResponse<IEnumerable<CategoryRes>?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data,
                        pagination);
                }
                else
                {
                    Expression<Func<Category, bool>>? search = c =>
                        (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(c.Name, $"%{searchKey}%"));

                    Func<IQueryable<Category>, IOrderedQueryable<Category>>? order = c => getCategoryReq.SortBy switch
                    {
                        CategorySortOption.NAME => isDescending
                            ? c.OrderByDescending(c => c.Name)
                            : c.OrderBy(c => c.Name),
                        _ => c.OrderBy(c => c.Id)
                    };

                    var categories = await _categoryRepository.GetAsync(search, order, pageNum, pageIndex);

                    if (categories == null)
                        return new ObjectResponse<IEnumerable<CategoryRes>?>(
                            (int)HttpStatusCode.NotFound,
                            "Category Data Is Empty!");

                    var totalData = await _categoryRepository.CountRecordAsync(search);
                    int totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                    var data = categories.Adapt<IEnumerable<CategoryRes>>();

                    var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                    var cates = await _categoryRepository.GetCategoriesAsync();

                    if (cates is not null)
                    {
                        var cache = cates.Adapt<IEnumerable<CategoryRes>>();

                        await _redisService
                            .SetAsync<IEnumerable<CategoryRes>?>(cacheKey, cache, TimeSpan.FromMinutes(10));
                    }

                    return new ObjectResponse<IEnumerable<CategoryRes>?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data,
                        pagination);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<CategoryRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Category By Id
        public async Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                string key = cacheKey + $":{id}";

                var cateCache = await _redisService.GetAsync<CategoryRes>(key);

                if (cateCache is not null)
                {
                    return new ObjectResponse<CategoryRes>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        cateCache);
                }
                else
                {
                    var category = await _categoryRepository.GetByIdAsync(id, false);

                    if (category == null)
                        return new ObjectResponse<CategoryRes>(
                            (int)HttpStatusCode.NotFound,
                            "Category Not Found!");

                    var data = _mapper.Map<CategoryRes>(category);

                    await _redisService.SetAsync<CategoryRes>(key, data, TimeSpan.FromMinutes(10));

                    return new ObjectResponse<CategoryRes>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResponse<CategoryRes>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Category
        public async Task<ObjectResponse<Category>> CreateCategoryAsync(CreateCategoryReq createCategoryReq)
        {
            try
            {
                var name = Util.FormatStringName(createCategoryReq.Name);

                var isExited = await _categoryRepository.CheckCategoryIsExistedAsync(name);

                if (isExited)
                    return new ObjectResponse<Category>(
                        (int)HttpStatusCode.BadRequest,
                        "Category Is Existed!");

                var newCate = _mapper.Map<Category>(createCategoryReq);
                newCate.Name = name;

                await _categoryRepository.InsertAsync(newCate);

                await _redisService.RemoveAsync(cacheKey);

                return new ObjectResponse<Category>(
                    (int)HttpStatusCode.Created,
                    "Create Category Successfully!",
                    newCate);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Category>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Bulk (Range) Create Categories
        public async Task<ObjectResponse<IEnumerable<Category>>> CreateRangeCategoriesAsync(List<CreateCategoryReq> categories)
        {
            try
            {
                if (categories.Count > 10)
                    return new ObjectResponse<IEnumerable<Category>>(
                        (int)HttpStatusCode.BadRequest,
                        "Only Create Max 10 Record At Once!");

                string[] names = categories
                    .Select(c => Util.FormatStringName(c.Name))
                    .ToArray();
                string[] dataNames = await _categoryRepository.GetCateNamesAsync();
                string[] existedNames = Util.CompareStringArray(names, dataNames);

                if (existedNames.Length > 0)
                    return new ObjectResponse<IEnumerable<Category>>(
                        (int)HttpStatusCode.BadRequest,
                        "Categories Are Existed!, Fullname: " + string.Join(", ", names));

                var newCategories = categories.Adapt<IEnumerable<Category>>();

                foreach (var items in newCategories)
                {
                    items.Id = Guid.NewGuid();
                    items.Name = Util.FormatStringName(items.Name);
                }

                await _categoryRepository.BulkInsertAsync(newCategories);

                await _redisService.RemoveAsync(cacheKey);

                return new ObjectResponse<IEnumerable<Category>>(
                    (int)HttpStatusCode.Created,
                    "Create Categories Successfully!",
                    newCategories);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Category>>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Category
        public async Task<VoidResponse> UpdateCategoryAsync(Guid id, UpdateCategoryReq updateCategoryReq)
        {
            try
            {
                var oldCate = await _categoryRepository.GetByIdAsync(id, true);

                if (oldCate == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Category Not Found!");

                var name = Util.FormatStringName(updateCategoryReq.Name);

                var isExited = await _categoryRepository.CheckCategoryIsExistedAsync(name);

                if (isExited && !name.Equals(oldCate.Name))
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Category Is Existed!");

                var newCate = _mapper.Map(updateCategoryReq, oldCate);
                newCate.Name = name;

                await _categoryRepository.UpdateAsync(newCate);

                string key = cacheKey + $":{id}";

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Category Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Category
        public async Task<VoidResponse> DeleteCategoryAsync(Guid id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id, true);

                if (category == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Category Not Found!");

                await _categoryRepository.DeleteAsync(category);

                string key = cacheKey + $":{id}";

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Category Succesfully!");
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
