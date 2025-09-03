using Mapster;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Response.Comic;
using OnComics.Library.Utils.Constants;

namespace OnComics.Library.Utils.Mapper
{
    public class ComicMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Comic, ComicRes>();

            config.NewConfig<CreateComicReq, Comic>()
                .Map(dest => dest.ReleaseDate, src => DateTime.UtcNow)
                .Map(dest => dest.UpdateTime, src => DateTime.UtcNow)
                .Map(dest => dest.Rating, src => 0)
                .Map(dest => dest.RateNum, src => 0)
                .Map(dest => dest.ChapNum, src => 0)
                .Map(dest => dest.FavoriteNum, src => 0)
                .Map(dest => dest.DayReadNum, src => 0)
                .Map(dest => dest.WeekReadNum, src => 0)
                .Map(dest => dest.MonthReadNum, src => 0)
                .Map(dest => dest.TotalReadNum, src => 0)
                .Map(dest => dest.Status, src => StatusConstant.ONGOING);

            config.NewConfig<UpdateComicReq, Comic>();
        }
    }
}
