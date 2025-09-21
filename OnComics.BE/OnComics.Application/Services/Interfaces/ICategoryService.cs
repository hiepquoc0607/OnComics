using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Category;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ObjectResponse<IEnumerable<CategoryRes>?>> GetCategoriesAsync(GetCategoryReq getCategoryReq);

        Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(int id);

        Task<ObjectResponse<Category>> CreateCategoryAsync(CreateCategoryReq createCategoryReq);

        Task<ObjectResponse<IEnumerable<Category>>> CreateCategoriesAsync(List<CreateCategoryReq> categories);

        Task<VoidResponse> UpdateCategoryAsync(int id, UpdateCategoryReq updateCategoryReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<CategoryStatus> updateStatus);

        Task<VoidResponse> DeleteCategoryAsync(int id);
    }
}
