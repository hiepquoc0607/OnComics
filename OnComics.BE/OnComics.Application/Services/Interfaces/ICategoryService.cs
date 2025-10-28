using OnComics.Application.Enums.Category;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Category;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ObjectResponse<IEnumerable<CategoryRes>?>> GetCategoriesAsync(GetCategoryReq getCategoryReq);

        Task<ObjectResponse<CategoryRes>> GetCategoryByIdAsync(Guid id);

        Task<ObjectResponse<Category>> CreateCategoryAsync(CreateCategoryReq createCategoryReq);

        Task<ObjectResponse<IEnumerable<Category>>> CreateRangeCategoriesAsync(List<CreateCategoryReq> categories);

        Task<VoidResponse> UpdateCategoryAsync(Guid id, UpdateCategoryReq updateCategoryReq);

        Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<CategoryStatus> updateStatus);

        Task<VoidResponse> DeleteCategoryAsync(Guid id);
    }
}
