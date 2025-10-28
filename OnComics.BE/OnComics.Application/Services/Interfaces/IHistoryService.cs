using OnComics.Application.Models.Request.History;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.History;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<ObjectResponse<IEnumerable<HistoryRes>?>> GetHistoriesAsync(GetHistoryReq getHistoryReq);

        Task<ObjectResponse<History>> CreateHistroyAsync(Guid accId, CreateHistoryReq createHistoryReq);

        Task<VoidResponse> UpdateHistroyAsync(Guid id, UpdateHistoryReq updateHistoryReq);

        Task<VoidResponse> DeleteHistoryAsync(Guid id);

        Task<VoidResponse> DeleteRangeHistoriesAsync(Guid accId);
    }
}
