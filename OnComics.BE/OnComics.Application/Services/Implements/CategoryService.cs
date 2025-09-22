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
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;

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
                return new ObjectResponse<IEnumerable<CategoryRes>?>("Error", 404, "Category Data Is Empty!");

            var totalData = await _categoryRepository.CountRecordAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getCategoryReq.PageIndex);
            var data = categories.Adapt<IEnumerable<CategoryRes>>();

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return new ObjectResponse<IEnumerable<CategoryRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Category By Id
        public async Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null) return new ObjectResponse<CategoryRes>("Error", 404, "Category Not Found!");

            var data = _mapper.Map<CategoryRes>(category);

            return new ObjectResponse<CategoryRes>("Success", 200, "Fetch Data Successfully!", data);
        }

        //Create Category
        public async Task<ObjectResponse<Category>> CreateCategoryAsync(CreateCategoryReq createCategoryReq)
        {
            var name = _util.FormatStringName(createCategoryReq.Name);

            var isExited = await _categoryRepository.CheckCategoryIsExistedAsync(name);

            if (isExited) return new ObjectResponse<Category>("Error", 400, "Category Is Existed!");

            var newCate = _mapper.Map<Category>(createCategoryReq);
            newCate.Name = name;

            try
            {
                await _categoryRepository.InsertAsync(newCate);

                return new ObjectResponse<Category>("Success", 200, "Create Category Successfully!", newCate);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Category>("Error", 400, "Create Category Fail!, Error Message:\n\n" + ex);
            }
        }

        //Bulk (Range) Create Categories
        public async Task<ObjectResponse<IEnumerable<Category>>> CreateCategoriesAsync(List<CreateCategoryReq> categories)
        {
            if (categories.Count > 10)
                return new ObjectResponse<IEnumerable<Category>>("Error", 400, "Only Create Max 10 Record At Once!");

            string[] names = categories.Select(c => c.Name).ToArray();
            string[] dataNames = await _categoryRepository.GetCateNamesAsync();
            string[] existedNames = _util.CompareStringArray(names, dataNames);

            if (existedNames.Length > 0)
                return new ObjectResponse<IEnumerable<Category>>("Error", 400, "Categories Are Existed!, Name: " + string.Join(", ", names));

            var newCategories = categories.Adapt<IEnumerable<Category>>();

            foreach (var items in newCategories)
            {
                items.Name = _util.FormatStringName(items.Name);
            }

            try
            {
                await _categoryRepository.InsertRangeAsync(newCategories);

                return new ObjectResponse<IEnumerable<Category>>("Success", 200, "Create Category Successfully!");
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<Category>>("Error", 400, "Create Categories Fail!, Error Message:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Update Caategory
        public async Task<VoidResponse> UpdateCategoryAsync(int id, UpdateCategoryReq updateCategoryReq)
        {
            var oldCate = await _categoryRepository.GetByIdAsync(id, true);

            if (oldCate == null) return new VoidResponse("Error", 404, "Category Not Found!");

            var newCate = _mapper.Map(updateCategoryReq, oldCate);
            newCate.Name = _util.FormatStringName(updateCategoryReq.Name);

            try
            {
                await _categoryRepository.UpdateAsync(newCate);

                return new VoidResponse("Success", 200, "Update Category Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Category Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Category Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<CategoryStatus> updateStatus)
        {
            var category = await _categoryRepository.GetByIdAsync(id, true);

            if (category == null) return new VoidResponse("Error", 404, "Category Not Found!");

            category.Status = updateStatus.Status switch
            {
                CategoryStatus.ACTIVE => StatusConstant.ACTIVE,
                _ => StatusConstant.INACTIVE
            };

            try
            {
                await _categoryRepository.UpdateAsync(category);

                return new VoidResponse("Success", 200, "Update Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Status Fail!, Error Message:\n\n" + ex);
            }

            throw new NotImplementedException();
        }

        //Delete Category
        public async Task<VoidResponse> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null) return new VoidResponse("Error", 404, "Category Not Found!");

            try
            {
                await _categoryRepository.DeleteAsync(id);

                return new VoidResponse("Success", 200, "Delete Category Succesfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Category Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
