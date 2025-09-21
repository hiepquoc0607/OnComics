using OnComics.Application.Enums.LeaderboardType;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Request.LeaderboardType;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.LeaderboardType;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface ILeaderboardTypeService
    {
        Task<ObjectResponse<IEnumerable<LeaderboardTypeRes>?>> GetLdbTypesAsync(GetLdbTypeReq getLdbTypeReq);

        Task<ObjectResponse<LeaderboardTypeRes?>> GetLbdTypeByIdAsync(int id);

        Task<ObjectResponse<Leaderboardtype>> CreateLdbTypeAsync(CreateLdbTypeReq createLdbTypeReq);

        Task<VoidResponse> UpdateLdbTypeAsync(int id, UpdateLdbTypeReq updateLdbTypeReq);

        Task<VoidResponse> UpdateLdbTypeStatusAsync(int id, UpdateStatusReq<LdbTypeStatus> updateStatusReq);

        Task<VoidResponse> DeleteLbdTypeAsync(int id);
    }
}
