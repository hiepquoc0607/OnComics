using OnComics.Application.Models.Request.Interaction;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Interaction;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IInteractionService
    {
        Task<ObjectResponse<IEnumerable<InteractionRes>?>> GetInteractionsAsync(GetInteractionReq getInteractionReq);

        Task<ObjectResponse<InteractionRes?>> GetInteractionByIdAsync(int id);

        Task<ObjectResponse<Interaction>> CreateInteractionAsync(int accId, CreateInteractionReq createInteractionReq);

        Task<VoidResponse> UpdateInteractionAsync(int id, UpdateInteractionReq updateInteractionReq);

        Task<VoidResponse> DeleteInteractionAsync(int id);
    }
}
