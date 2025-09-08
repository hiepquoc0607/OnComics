using Mapster;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Category;
using OnComics.Library.Models.Response.Category;
using OnComics.Library.Utils.Constants;

namespace OnComics.Library.Utils.Mapper
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
