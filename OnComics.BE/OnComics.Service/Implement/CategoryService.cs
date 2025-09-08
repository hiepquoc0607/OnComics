using Mapster;
using MapsterMapper;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Category;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Category;
using OnComics.Library.Utils.Constants;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Interface;
using OnComics.Service.Interface;

namespace OnComics.Service.Implement
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
            var (categories, pagination) = await _categoryRepository.GetCategoriesAsync(getCategoryReq);

            if (categories == null)
                return new ObjectResponse<IEnumerable<CategoryRes>?>("Error", 404, "Category Data Is Empty!");

            var data = categories.Adapt<IEnumerable<CategoryRes>>();

            return new ObjectResponse<IEnumerable<CategoryRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Category By Id
        public async Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id, false);

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
                await _categoryRepository.CreateCategoryAsync(newCate);

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
                await _categoryRepository.CreateCategoriesAsync(newCategories);

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
            var oldCate = await _categoryRepository.GetCategoryByIdAsync(id, true);

            if (oldCate == null) return new VoidResponse("Error", 404, "Category Not Found!");

            var newCate = _mapper.Map(updateCategoryReq, oldCate);
            newCate.Name = _util.FormatStringName(updateCategoryReq.Name);

            try
            {
                await _categoryRepository.UpdateCategoryAsync(newCate);

                return new VoidResponse("Success", 200, "Update Category Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Category Fail!, Error Message:\n\n" + ex);
            }
        }

        //Update Category Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<CateStatus> updateStatus)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id, true);

            if (category == null) return new VoidResponse("Error", 404, "Category Not Found!");

            category.Status = updateStatus.Status switch
            {
                CateStatus.ACTIVE => StatusConstant.ACTIVE,
                _ => StatusConstant.INACTIVE
            };

            try
            {
                await _categoryRepository.UpdateCategoryAsync(category);

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
            var category = await _categoryRepository.GetCategoryByIdAsync(id, false);

            if (category == null) return new VoidResponse("Error", 404, "Category Not Found!");

            try
            {
                await _categoryRepository.DeleteCategoryAsync(id);

                return new VoidResponse("Success", 200, "Delete Category Succesfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Category Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}