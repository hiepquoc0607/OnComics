using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Enums.Favorite;
using OnComics.Application.Models.Request.Favorite;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Models.Response.Favorite;
using OnComics.Application.Services.Interfaces;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IComicRepository _comicRepository;
        private readonly IMapper _mapper;

        public FavoriteService(
            IFavoriteRepository favoriteRepository,
            IComicRepository comicRepository,
            IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _comicRepository = comicRepository;
            _mapper = mapper;
        }

        //Get All Favorites
        public async Task<ObjectResponse<IEnumerable<FavoriteRes>?>> GetFavoritesAsync(GetFavoriteReq getFavoriteReq)
        {
            try
            {
                string? searchKey = getFavoriteReq.SearchKey;

                int pageNum = getFavoriteReq.PageNum;
                int pageIndex = getFavoriteReq.PageIndex;

                bool isDecending = getFavoriteReq.IsDescending;

                Guid? searchId = getFavoriteReq.Id;

                bool? isComicId = getFavoriteReq.IdType switch
                {
                    FavoriteIdType.ACCOUNT => false,
                    FavoriteIdType.COMIC => true,
                    _ => null
                };

                Expression<Func<Favorite, bool>>? search = null;

                int totalData = 0;

                if (searchId.HasValue && isComicId == true)
                {
                    search = f =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(f.Comic.Name, $"%{searchKey}%") ||
                        EF.Functions.Like(f.Account.Fullname, $"%{searchKey}%")) &&
                        f.ComicId == searchId;

                    totalData = await _favoriteRepository.CountFavoriteAsync(searchId.Value, true);
                }
                else if (searchId.HasValue && isComicId == false)
                {
                    search = f =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(f.Comic.Name, $"%{searchKey}%") ||
                        EF.Functions.Like(f.Account.Fullname, $"%{searchKey}%")) &&
                        f.AccountId == searchId;

                    totalData = await _favoriteRepository.CountFavoriteAsync(searchId.Value, false);
                }
                else
                {
                    search = f =>
                        (string.IsNullOrEmpty(searchKey) ||
                        EF.Functions.Like(f.Comic.Name, $"%{searchKey}%") ||
                        EF.Functions.Like(f.Account.Fullname, $"%{searchKey}%"));

                    totalData = await _favoriteRepository.CountRecordAsync(search);
                }

                Func<IQueryable<Favorite>, IOrderedQueryable<Favorite>>? order = f => getFavoriteReq.SortBy switch
                {
                    FavoriteSortOption.ACCOUNT => isDecending
                        ? f.OrderByDescending(f => f.Account.Fullname)
                        : f.OrderBy(f => f.Account.Fullname),
                    FavoriteSortOption.COMIC => isDecending
                        ? f.OrderByDescending(f => f.Account.Fullname)
                        : f.OrderBy(f => f.Account.Fullname),
                    _ => f.OrderBy(f => f.Id)
                };

                var (favorites, accounts, comics) = await _favoriteRepository
                    .GetFavoritesAsync(search, order, pageNum, pageIndex);

                if (favorites == null)
                    return new ObjectResponse<IEnumerable<FavoriteRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Favorite Data Empty!");

                var data = favorites.Select(f => new FavoriteRes
                {
                    Id = f.Id,
                    AccountId = accounts![f.Id].Item1,
                    Fullname = accounts![f.Id].Item2,
                    ComicId = comics![f.Id].Item1,
                    ComicName = comics![f.Id].Item2
                });

                var toatlPage = (int)Math.Ceiling((decimal)totalData / pageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, toatlPage);

                return new ObjectResponse<IEnumerable<FavoriteRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<FavoriteRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Create Favorite
        public async Task<ObjectResponse<Favorite>> CreateFavoriteAsync(Guid accId, CreateFavoriteReq createFavoriteReq)
        {
            try
            {
                Guid comicId = createFavoriteReq.ComicId;

                bool isExisted = await _favoriteRepository
                    .CheckFavoriteExistedAsync(accId, comicId);

                if (isExisted)
                    return new ObjectResponse<Favorite>(
                        (int)HttpStatusCode.BadRequest,
                        "Favorite Is Existed!");

                var comic = await _comicRepository.GetByIdAsync(comicId, true);

                if (comic == null)
                    return new ObjectResponse<Favorite>(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                var newFav = _mapper.Map<Favorite>(createFavoriteReq);
                newFav.AccountId = accId;

                comic.FavoriteNum = comic.FavoriteNum + 1;

                await _favoriteRepository.InsertAsync(newFav, true);

                await _comicRepository.UpdateAsync(comic, true);

                return new ObjectResponse<Favorite>(
                    (int)HttpStatusCode.Created,
                    "Create Favorite Successfully!",
                    newFav);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Favorite>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Favorite
        public async Task<VoidResponse> DeleteFavoriteAsync(Guid id)
        {
            try
            {
                var fav = await _favoriteRepository.GetByIdAsync(id, true);

                if (fav == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Favorite Not Found!");

                var comic = await _comicRepository.GetByIdAsync(fav.ComicId, true);

                if (comic == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Comic Not Found!");

                comic.FavoriteNum = comic.FavoriteNum - 1;

                await _favoriteRepository.DeleteAsync(fav, true);

                await _comicRepository.UpdateAsync(comic, true);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Favorite Successfully!");
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
