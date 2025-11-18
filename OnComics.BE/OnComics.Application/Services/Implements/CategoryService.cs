using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Request.General;
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
        private readonly IMapper _mapper;
        private readonly Util _util;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            Util util)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Categoriess
        public async Task<ObjectResponse<IEnumerable<CategoryRes>?>> GetCategoriesAsync(GetCategoryReq getCategoryReq)
        {
            try
            {
                string? searchKey = getCategoryReq.SearchKey;

                string? status = getCategoryReq.Status switch
                {
                    CategoryStatus.ACTIVE => StatusConstant.ACTIVE,
                    CategoryStatus.INACTIVE => StatusConstant.INACTIVE,
                    _ => null
                };

                bool isDescending = getCategoryReq.IsDescending;

                int pageNum = getCategoryReq.PageNum;
                int pageIndex = getCategoryReq.PageIndex;


                Expression<Func<Category, bool>>? search = c =>
                    (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(c.Name, $"%{searchKey}%")) &&
                    (string.IsNullOrEmpty(status) || c.Status.Equals(status));

                Func<IQueryable<Category>, IOrderedQueryable<Category>>? order = c => getCategoryReq.SortBy switch
                {
                    CategorySortOption.NAME => isDescending
                        ? c.OrderByDescending(c => c.Name)
                        : c.OrderBy(c => c.Name),
                    CategorySortOption.STATUS => isDescending
                        ? c.OrderByDescending(c => c.Status)
                        : c.OrderBy(c => c.Status),
                    _ => c.OrderBy(c => c.Id)
                };

                var categories = await _categoryRepository.GetAsync(search, order, pageNum, pageIndex);

                if (categories == null)
                    return new ObjectResponse<IEnumerable<CategoryRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Category Data Is Empty!");

                var totalData = await _categoryRepository.CountRecordAsync(search);
                int totalPage = (int)Math.Ceiling((decimal)totalData / getCategoryReq.PageIndex);
                var data = categories.Adapt<IEnumerable<CategoryRes>>();

                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<CategoryRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
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
                var category = await _categoryRepository.GetByIdAsync(id, false);

                if (category == null)
                    return new ObjectResponse<CategoryRes>(
                        (int)HttpStatusCode.NotFound,
                        "Category Not Found!");

                var data = _mapper.Map<CategoryRes>(category);

                return new ObjectResponse<CategoryRes>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!", data);
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
                var name = _util.FormatStringName(createCategoryReq.Name);

                var isExited = await _categoryRepository.CheckCategoryIsExistedAsync(name);

                if (isExited)
                    return new ObjectResponse<Category>(
                        (int)HttpStatusCode.BadRequest,
                        "Category Is Existed!");

                var newCate = _mapper.Map<Category>(createCategoryReq);
                newCate.Name = name;

                await _categoryRepository.InsertAsync(newCate, true);

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

                string[] names = categories.Select(c => c.Name).ToArray();
                string[] dataNames = await _categoryRepository.GetCateNamesAsync();
                string[] existedNames = _util.CompareStringArray(names, dataNames);

                if (existedNames.Length > 0)
                    return new ObjectResponse<IEnumerable<Category>>(
                        (int)HttpStatusCode.BadRequest,
                        "Categories Are Existed!, Fullname: " + string.Join(", ", names));

                var newCategories = categories.Adapt<IEnumerable<Category>>();

                foreach (var items in newCategories)
                {
                    items.Name = _util.FormatStringName(items.Name);
                }

                await _categoryRepository.BulkInsertAsync(newCategories);

                return new ObjectResponse<IEnumerable<Category>>(
                    (int)HttpStatusCode.OK,
                    "Create Category Successfully!");
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Category>>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Caategory
        public async Task<VoidResponse> UpdateCategoryAsync(Guid id, UpdateCategoryReq updateCategoryReq)
        {
            try
            {
                var oldCate = await _categoryRepository.GetByIdAsync(id, true);

                if (oldCate == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Category Not Found!");

                var newCate = _mapper.Map(updateCategoryReq, oldCate);
                newCate.Name = _util.FormatStringName(updateCategoryReq.Name);

                await _categoryRepository.UpdateAsync(newCate, true);

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

        //Update Category Status
        public async Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<CategoryStatus> updateStatus)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id, true);

                if (category == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Category Not Found!");

                category.Status = updateStatus.Status switch
                {
                    CategoryStatus.ACTIVE => StatusConstant.ACTIVE,
                    _ => StatusConstant.INACTIVE
                };

                await _categoryRepository.UpdateAsync(category, true);

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

                await _categoryRepository.DeleteAsync(category, true);

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
