using Mapster;
using OnComics.Application.Models.Request.History;
using OnComics.Application.Models.Response.History;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class HistoryMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<History, HistoryRes>();

            config.NewConfig<CreateHistoryReq, History>()
                .Map(dest => dest.ReadTime, src => DateTime.UtcNow);

            config.NewConfig<UpdateHistoryReq, History>()
                .Map(dest => dest.ReadTime, src => DateTime.UtcNow);
        }
    }
}
