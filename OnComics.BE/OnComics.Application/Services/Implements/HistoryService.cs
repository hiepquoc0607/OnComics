using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.History;
using OnComics.Application.Models.Request.History;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.History;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IMapper _mapper;

        public HistoryService(
            IHistoryRepository historyRepository,
            IMapper mapper)
        {
            _historyRepository = historyRepository;
            _mapper = mapper;
        }

        //Get All Histories
        public async Task<ObjectResponse<IEnumerable<HistoryRes>?>> GetHistoriesAsync(GetHistoryReq getHistoryReq)
        {
            try
            {
                string? searchKey = getHistoryReq.SearchKey;

                int pageNum = getHistoryReq.PageNum;
                int pageIndex = getHistoryReq.PageIndex;

                bool isDecending = getHistoryReq.IsDescending;

                int? searchId = getHistoryReq.Id;

                bool? isComicId = getHistoryReq.IdType switch
                {
                    HistoryIdType.ACCOUNT => false,
                    HistoryIdType.COMIC => true,
                    _ => null
                };

                Expression<Func<History, bool>>? search = null;

                int totalData = 0;

                if (searchId.HasValue && isComicId == true)
                {
                    search = h =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(h.Account.Fullname, $"%{searchKey}%")) &&
                        h.Chapter.Comic.Id == searchId;

                    totalData = await _historyRepository
                        .CountHistoryAsync(searchId.Value, isComicId.Value);
                }
                else if (searchId.HasValue && isComicId == false)
                {
                    search = h =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(h.Chapter.Comic.Name, $"%{searchKey}%")) &&
                        h.Chapter.Comic.Id == searchId;

                    totalData = await _historyRepository
                        .CountHistoryAsync(searchId.Value, isComicId.Value);
                }
                else
                {
                    search = h =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(h.Account.Fullname, $"%{searchKey}%") ||
                        EF.Functions.Like(h.Chapter.Comic.Name, $"%{searchKey}%")) &&
                        h.Chapter.Comic.Id == searchId;

                    totalData = await _historyRepository.CountRecordAsync();
                }

                Func<IQueryable<History>, IOrderedQueryable<History>>? order = h => getHistoryReq.SortBy switch
                {
                    HistorySortOption.ACCOUNT => isDecending
                        ? h.OrderByDescending(h => h.Account.Fullname)
                        : h.OrderBy(h => h.Account.Fullname),
                    HistorySortOption.COMIC => isDecending
                        ? h.OrderByDescending(h => h.Chapter.Comic.Name)
                        : h.OrderBy(h => h.Chapter.Comic.Name),
                    HistorySortOption.TIME => isDecending
                        ? h.OrderByDescending(h => h.ReadTime)
                        : h.OrderBy(h => h.ReadTime),
                    _ => h.OrderBy(h => h.Id)
                };

                var (histories, accounts, comics) = await _historyRepository
                    .GetHistoriesAsync(search, order, pageNum, pageIndex);

                if (histories == null)
                    return new ObjectResponse<IEnumerable<HistoryRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "History Data Is Empty!");

                var data = histories.Select(h => new HistoryRes
                {
                    Id = h.Id,
                    AccountId = h.AccountId,
                    Fullname = accounts[h.AccountId],
                    ComicId = h.Chapter.Comic.Id,
                    ComicName = comics[h.Chapter.Comic.Id],
                    ChapterId = h.ChapterId,
                    ReadTime = h.ReadTime
                });

                var totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<HistoryRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<HistoryRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create History
        public async Task<ObjectResponse<History>> CreateHistroyAsync(int accId, CreateHistoryReq createHistoryReq)
        {
            try
            {
                bool isExisted = await _historyRepository
                    .CheckHistoryExistedAsync(accId, createHistoryReq.ChapterId);

                if (isExisted)
                    return new ObjectResponse<History>(
                        (int)HttpStatusCode.BadRequest,
                        "History Is Existed!");

                var newHistory = _mapper.Map<History>(createHistoryReq);

                await _historyRepository.InsertAsync(newHistory);

                return new ObjectResponse<History>(
                    (int)HttpStatusCode.OK,
                    "Create Hisotry Successfully!",
                    newHistory);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<History>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update History
        public async Task<VoidResponse> UpdateHistroyAsync(int id, UpdateHistoryReq updateHistoryReq)
        {
            try
            {
                var oldHistory = await _historyRepository.GetByIdAsync(id, true);

                if (oldHistory == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "History Not Found!");

                var newHistory = _mapper.Map(updateHistoryReq, oldHistory);

                await _historyRepository.UpdateAsync(newHistory);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update History Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete History
        public async Task<VoidResponse> DeleteHistoryAsync(int id)
        {
            try
            {
                var history = await _historyRepository.GetByIdAsync(id);

                if (history == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "History Not Found!");

                await _historyRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete History Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Bulk Delete Range Histories By Account Id
        public async Task<VoidResponse> DeleteRangeHistoriesAsync(int accId)
        {
            try
            {
                var histories = await _historyRepository
                    .GetHistoriesByAccountIdAsync(accId);

                if (histories == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account's History Data Empty!");

                await _historyRepository.BulkDeleteRangeAsync(histories);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Histories Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
