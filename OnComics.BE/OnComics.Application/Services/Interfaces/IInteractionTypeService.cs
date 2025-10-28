using OnComics.Application.Enums.InteractionType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.InteractionType;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.InteractionType;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IInteractionTypeService
    {
        Task<ObjectResponse<IEnumerable<InteractionTypeRes>?>> GetItrTypesAsync(GetItrTypeReq getItrTypeReq);

        Task<ObjectResponse<InteractionTypeRes?>> GetItrTypeByIdAsync(Guid id);

        Task<ObjectResponse<Interactiontype>> CreateItrTypeAsync(CreateItrTypeReq createItrTypeReq);

        Task<VoidResponse> UpdateItrTypeAsync(Guid id, UpdateItrTypeReq updateItrTypeReq);

        Task<VoidResponse> UpdateItrTypeStatusAsync(Guid id, UpdateStatusReq<ItrTypeStatus> updateStatusReq);

        Task<VoidResponse> DeleteItrTypeAsync(Guid id);
    }
}
