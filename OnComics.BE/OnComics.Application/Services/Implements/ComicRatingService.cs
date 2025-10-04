using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.ComicRating;
using OnComics.Application.Models.Request.ComicRating;
using OnComics.Application.Models.Response.ComicRating;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class ComicRatingService : IComicRatingService
    {
        private readonly IComicRatingRepository _comicRatingRepository;
        private readonly IMapper _mapper;

        public ComicRatingService(
            IComicRatingRepository comicRatingRepository,
            IMapper mapper)
        {
            _comicRatingRepository = comicRatingRepository;
            _mapper = mapper;
        }

        //Get All Ratings
        public async Task<ObjectResponse<IEnumerable<ComicRatingRes>?>> GetRatingsAsync(GetComicRatingReq getComicRatingReq)
        {
            string? searchKey = getComicRatingReq.SearchKey;

            int pageNum = getComicRatingReq.PageNum;
            int pageIndex = getComicRatingReq.PageIndex;

            bool isDecending = getComicRatingReq.IsDescending;

            int searchId = getComicRatingReq.Id;

            bool isComicId = getComicRatingReq.IdType switch
            {
                RatingIdType.ACCOUNT => false,
                _ => true
            };

            Expression<Func<Comicrating, bool>>? search = null;

            int totalData = 0;

            if (isComicId)
            {
                search = r =>
                    (string.IsNullOrEmpty(searchKey) ||
                    EF.Functions.Like(r.Account.Fullname, $"%{searchKey}%") ||
                    EF.Functions.Like(r.Comic.Name, $"%{searchKey}%")) &&
                    r.ComicId == searchId;

                totalData = await _comicRatingRepository.CountRatingByComicIdAsync(searchId);
            }
            else
            {
                search = r =>
                    (string.IsNullOrEmpty(searchKey) ||
                    EF.Functions.Like(r.Account.Fullname, $"%{searchKey}%") ||
                    EF.Functions.Like(r.Comic.Name, $"%{searchKey}%")) &&
                    r.AccountId == searchId;

                totalData = await _comicRatingRepository.CountRatingByAccountIdAsync(searchId);
            }

            Func<IQueryable<Comicrating>, IOrderedQueryable<Comicrating>>? order = r => getComicRatingReq.SortBy switch
            {
                RatingSortOption.ACCOUNT => isDecending
                    ? r.OrderByDescending(r => r.Account.Fullname)
                    : r.OrderBy(r => r.Account.Fullname),
                RatingSortOption.COMIC => isDecending
                    ? r.OrderByDescending(r => r.Comic.Name)
                    : r.OrderBy(r => r.Comic.Name),
                RatingSortOption.RATING => isDecending
                    ? r.OrderByDescending(r => r.Rating)
                    : r.OrderBy(r => r.Rating),
                _ => r.OrderBy(r => r.Id)
            };

            var (ratings, accounts, comics) = await _comicRatingRepository
                .GetRatingsAsync(search, order, pageNum, pageIndex);

            if (ratings == null)
                return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                    (int)HttpStatusCode.NotFound,
                    "Rating Data Is Empty!");

            var data = ratings.Select(d => new ComicRatingRes
            {
                Id = d.Id,
                AccountId = d.AccountId,
                Fullname = accounts[d.AccountId],
                ComicId = d.ComicId,
                ComicName = comics[d.ComicId],
                Rating = d.Rating
            });

            var totalPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return new ObjectResponse<IEnumerable<ComicRatingRes>?>(
                (int)HttpStatusCode.OK,
                "Fetch Data Successfully!",
                data,
                pagination);
        }

        //Create Rating
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

        //Update Rating
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

        //Delete Rating
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
