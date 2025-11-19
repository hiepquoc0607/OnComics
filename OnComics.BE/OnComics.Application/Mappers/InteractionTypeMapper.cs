using Mapster;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Models.Response.InteractionType;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Mappers
{
    public class InteractionTypeMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Interactiontype, InteractionTypeRes>();

            config.NewConfig<CreateItrTypeReq, Interactiontype>()
                .Map(dest => dest.Id, src => Guid.NewGuid())
                .Map(dest => dest.Status, src => StatusConstant.ACTIVE);

            config.NewConfig<UpdateItrTypeReq, Interactiontype>();
        }
    }
}
