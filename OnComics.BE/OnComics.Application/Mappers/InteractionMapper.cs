using Mapster;
using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Models.Response.Interaction;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class InteractionMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Interaction, InteractionRes>();

            config.NewConfig<CreateInteractionReq, Interaction>()
                .Map(dest => dest.ReactTime, src => DateTime.UtcNow);

            config.NewConfig<UpdateInteractionReq, Interaction>()
                .Map(dest => dest.ReactTime, src => DateTime.UtcNow);
        }
    }
}
