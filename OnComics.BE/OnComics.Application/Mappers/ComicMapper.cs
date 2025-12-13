using Mapster;
using Microsoft.Extensions.Configuration;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Comic;
using OnComics.Application.Models.Response.Comic;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class ComicMapper : IRegister
    {
        private string GetDefaultThumnailUrl()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            return configuration["AppDefaultUrl:DefaultThumbnailUrl"] ?? string.Empty;
        }

        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Comic, ComicRes>();

            config.NewConfig<CreateComicReq, Comic>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.ReleaseDate, src => DateOnly.FromDateTime(DateTime.UtcNow))
                .Map(dest => dest.UpdateTime, src => DateTime.UtcNow)
                .Map(dest => dest.ThumbnailUrl, src => GetDefaultThumnailUrl())
                .Map(dest => dest.Rating, src => 0)
                .Map(dest => dest.RateNum, src => 0)
                .Map(dest => dest.ChapNum, src => 0)
                .Map(dest => dest.FavoriteNum, src => 0)
                .Map(dest => dest.DayReadNum, src => 0)
                .Map(dest => dest.WeekReadNum, src => 0)
                .Map(dest => dest.MonthReadNum, src => 0)
                .Map(dest => dest.TotalReadNum, src => 0)
                .Map(dest => dest.Status, src => StatusConstant.ONGOING);

            config.NewConfig<UpdateComicReq, Comic>()
                .Map(dest => dest.UpdateTime, src => DateTime.UtcNow);
        }
    }
}
