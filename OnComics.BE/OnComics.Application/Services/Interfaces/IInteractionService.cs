using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Interaction;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IInteractionService
    {
        Task<ObjectResponse<IEnumerable<InteractionRes>?>> GetInteractionsAsync(GetInteractionReq getInteractionReq);

        Task<ObjectResponse<InteractionRes?>> GetInteractionByIdAsync(Guid id);

        Task<ObjectResponse<Interaction>> CreateInteractionAsync(
            Guid accId,
            CreateInteractionReq createInteractionReq);

        Task<VoidResponse> UpdateInteractionAsync(Guid id, UpdateInteractionReq updateInteractionReq);

        Task<VoidResponse> DeleteInteractionAsync(Guid id);
    }
}
