using OnComics.Application.Models.Request.History;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.History;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<ObjectResponse<IEnumerable<HistoryRes>?>> GetHistoriesAsync(GetHistoryReq getHistoryReq);

        Task<ObjectResponse<History>> CreateHistroyAsync(int accId, CreateHistoryReq createHistoryReq);

        Task<VoidResponse> UpdateHistroyAsync(int id, UpdateHistoryReq updateHistoryReq);

        Task<VoidResponse> DeleteHistoryAsync(int id);

        Task<VoidResponse> DeleteRangeHistoriesAsync(int accId);
    }
}
