using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.ComicRating;
using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Models.Response.ComicRating;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ComicRatingService : IComicRatingService
    {
        private readonly IComicRatingRepository _comicRatingRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IMapper _mapper;

        public ComicRatingService(
            IComicRatingRepository comicRatingRepository,
            IAccountRepository accountRepository,
            IComicRepository comicRepository,
            IMapper mapper)
        {
            _comicRatingRepository = comicRatingRepository;
            _accountRepository = accountRepository;
            _comicRepository = comicRepository;
            _mapper = mapper;
        }

        public async Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsByAccountIdAsync(int accId, GetComicRatingReq getComicRatingReq)
        {
            string? searchKey = getComicRatingReq.SearchKey;

            int pageNum = getComicRatingReq.PageNum;
            int pageIndex = getComicRatingReq.PageIndex;

            bool isDecending = getComicRatingReq.IsDescending;

            Expression<Func<Comicrating, bool>>? seacrh = r =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(r.Comic.Name, $"%{searchKey}%")) &&
                r.AccountId == accId;

            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>> order = r => getComicRatingReq.SortBy switch
            {
                RatingSortOption.NAME => isDecending
                    ? r.OrderByDescending(r => r.Comic.Name)
                    : r.OrderBy(r => r.Comic.Name),
                RatingSortOption.RATING => isDecending
                    ? r.OrderByDescending(r => r.Rating)
                    : r.OrderBy(r => r.Rating),
                _ => r.OrderBy(r => r.Id)
            };

            var ratings = await _comicRatingRepository.GetAsync(seacrh, order, pageNum, pageIndex);

            if (ratings == null)
                return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                    (int)HttpStatusCode.NotFound,
                    "Rating Data Is Empty!");

            int[] comicIds = ratings.Select(r => r.ComicId).ToArray();

            var comicNames = await _comicRepository.GetNamesByIdsAsync(comicIds);

            var data = ratings.Select(d => new ComicRatingRes
            {
                Id = d.Id,
                AccountId = null,
                Fullname = null,
                ComicId = d.ComicId,
                ComicName = comicNames[d.ComicId],
                Rating = d.Rating
            });

            var totalData = await _comicRatingRepository.CountRatingByAccountIdAsync(accId);
            var toatlPage = (int)Math.Ceiling((decimal)totalData / getComicRatingReq.PageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

            return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!",
                data,
                pagination);
        }

        public async Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsByComicIdAsync(int comicId, GetComicRatingReq getComicRatingReq)
        {
            string? searchKey = getComicRatingReq.SearchKey;

            int pageNum = getComicRatingReq.PageNum;
            int pageIndex = getComicRatingReq.PageIndex;

            bool isDecending = getComicRatingReq.IsDescending;

            Expression<Func<Comicrating, bool>>? seacrh = r =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(r.Account.Fullname, $"%{searchKey}%")) &&
                r.ComicId == comicId;

            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>> order = r => getComicRatingReq.SortBy switch
            {
                RatingSortOption.NAME => isDecending
                    ? r.OrderByDescending(r => r.Account.Fullname)
                    : r.OrderBy(r => r.Account.Fullname),
                RatingSortOption.RATING => isDecending
                    ? r.OrderByDescending(r => r.Rating)
                    : r.OrderBy(r => r.Rating),
                _ => r.OrderBy(r => r.Id)
            };

            var ratings = await _comicRatingRepository.GetAsync(seacrh, order, pageNum, pageIndex);

            if (ratings == null)
                return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                    (int)HttpStatusCode.NotFound,
                    "Rating Data Is Empty!");

            int[] accIds = ratings.Select(r => r.AccountId).ToArray();

            var fullnames = await _accountRepository.GetFullnameByIdsAsync(accIds);

            var data = ratings.Select(d => new ComicRatingRes
            {
                Id = d.Id,
                AccountId = d.AccountId,
                Fullname = fullnames[d.Id],
                ComicId = null,
                ComicName = null,
                Rating = d.Rating
            });

            var totalData = await _comicRatingRepository.CountRatingByComicIdAsync(comicId);
            var toatlPage = (int)Math.Ceiling((decimal)totalData / getComicRatingReq.PageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

            return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!",
                data,
                pagination);
        }

        public async Task<ObjectResponse<Comicrating>> CreateRatingAsync(int accId, CreateComicRatingReq createComicRatingReq)
        {
            var rating = await _comicRatingRepository
                .GetRatingByAccIdAndComicIdAsync(accId, createComicRatingReq.ComicId);

            if (rating != null)
                return new ObjectResponse<Comicrating>(
                    (int)HttpStatusCode.BadRequest,
                    "Rating Is Existed!");

            var newRating = _mapper.Map<Comicrating>(createComicRatingReq);
            newRating.Id = accId;

            try
            {
                await _comicRatingRepository.InsertAsync(newRating);

                return new ObjectResponse<Comicrating>(
                    (int)HttpStatusCode.OK,
                    "Create Rating Successfully!",
                    newRating);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Comicrating>(
                    (int)HttpStatusCode.BadRequest,
                    "Create Rating Fail!, Error Message:\n\n" + ex);
            }
        }

        public async Task<VoidResponse> UpdateRatingAsync(int id, UpdateComicRatingReq updateRatingReq)
        {
            var rating = await _comicRatingRepository.GetByIdAsync(id, true);

            if (rating == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Rating Not Found!");

            rating.Rating = (decimal)updateRatingReq.Rating;

            try
            {
                await _comicRatingRepository.UpdateAsync(rating);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Rating Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Update Rating Fail!, Error Message:\n\n" + ex);
            }
        }

        public async Task<VoidResponse> DeleteRatingAsync(int id)
        {
            var rating = await _comicRatingRepository.GetByIdAsync(id);

            if (rating == null)
                return new VoidResponse(
                    (int)HttpStatusCode.NotFound,
                    "Rating Not Found!");

            try
            {
                await _comicRatingRepository.DeleteAsync(rating.Id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Rating Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Delete Rating Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
