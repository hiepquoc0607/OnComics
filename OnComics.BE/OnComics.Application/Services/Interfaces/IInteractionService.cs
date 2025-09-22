using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.InteractionType;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface IInteractionService
    {
        Task<ObjectResponse<IEnumerable<InteractionTypeRes>?>> GetItrTypesAsync(GetItrTypeReq getItrTypeReq);

        Task<ObjectResponse<InteractionTypeRes?>> GetItrTypeByIdAsync(int id);

        Task<ObjectResponse<Interactiontype>> CreateItrTypeAsync(CreateItrTypeReq createItrTypeReq);

        Task<VoidResponse> UpdateItrTypeAsync(int id, UpdateItrTypeReq updateItrTypeReq);

        Task<VoidResponse> UpdateItrTypeStatusAsync(int id, UpdateStatusReq<ItrTypeStatus> updateStatusReq);

        Task<VoidResponse> DeleteItrTypeAsync(int id);
    }
}
