using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Category;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Category;

namespace OnComics.Service.Interface
{
    public interface ICategoryService
    {
        Task<ObjectResponse<IEnumerable<CategoryRes>?>> GetCategoriesAsync(GetCategoryReq getCategoryReq);

        Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(int id);

        Task<ObjectResponse<Category>> CreateCategoryAsync(CreateCategoryReq createCategoryReq);

        Task<ObjectResponse<IEnumerable<Category>>> CreateCategoriesAsync(List<CreateCategoryReq> categories);

        Task<VoidResponse> UpdateCategoryAsync(int id, UpdateCategoryReq updateCategoryReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<CateStatus> updateStatus);

        Task<VoidResponse> DeleteCategoryAsync(int id);
    }
}