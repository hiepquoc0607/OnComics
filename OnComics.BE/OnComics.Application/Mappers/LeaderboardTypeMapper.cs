using Mapster;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.LeaderboardType;
using OnComics.Application.Models.Response.LeaderboardType;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Mappers
{
    public class LeaderboardTypeMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Leaderboardtype, LeaderboardTypeRes>();

            config.NewConfig<CreateLdbTypeReq, Leaderboardtype>()
                .Map(dest => dest.Status, src => StatusConstant.ACTIVE);

            config.NewConfig<UpdateLdbTypeReq, Leaderboardtype>();
        }
    }
}
