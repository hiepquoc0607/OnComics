using Mapster;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Category;
using OnComics.Application.Models.Response.Category;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class CategoryMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Category, CategoryRes>();

            config.NewConfig<CreateCategoryReq, Category>()
                .Map(dest => dest.Status, src => StatusConstant.ACTIVE);

            config.NewConfig<UpdateCategoryReq, Category>();
        }
    }
}
