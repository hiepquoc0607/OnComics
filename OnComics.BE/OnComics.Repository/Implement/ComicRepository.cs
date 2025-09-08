using Microsoft.EntityFrameworkCore;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Comic;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Models.Response.Category;
using OnComics.Library.Models.Response.Comic;
using OnComics.Library.Utils.Constants;
using OnComics.Repository.Interface;
using System.Linq.Expressions;

namespace OnComics.Repository.Implement
{
    public class ComicRepository : GenericRepository<Comic>, IComicRepository
    {
        public ComicRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Comics
        public async Task<(IEnumerable<Comic>?, Pagination)> GetComicsAsync(GetComicReq getComicReq)
        {
            string? searchKey = getComicReq.SearchKey;

            string? status = getComicReq.Status switch
            {
                ComicStatus.ONGOING => StatusConstant.ONGOING,
                ComicStatus.UPCOMING => StatusConstant.UPCOMING,
                ComicStatus.FINISHED => StatusConstant.FINISHED,
                _ => null
            };

            bool isDescending = getComicReq.IsDescending;

            int pageNum = getComicReq.PageNum;
            int pageIndex = getComicReq.PageIndex;
            var totalData = await _context.Comics.AsNoTracking().CountAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getComicReq.PageIndex);

            Expression<Func<Comic, bool>>? search = c =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(c.Name, $"%{searchKey}%")) &&
                (string.IsNullOrEmpty(status) || c.Status.Equals(status));

            Func<IQueryable<Comic>, IOrderedQueryable<Comic>>? order = a => getComicReq.SortBy switch
            {
                ComicSortOptions.NAME => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.Name)
                    : a.OrderBy(a => a.Name),
                ComicSortOptions.AUTHOR => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.Author)
                    : a.OrderBy(a => a.Author),
                ComicSortOptions.RELEASE => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.ReleaseDate)
                    : a.OrderBy(a => a.ReleaseDate),
                ComicSortOptions.UPDATE => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.UpdateTime)
                    : a.OrderBy(a => a.UpdateTime),
                ComicSortOptions.RATING => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.Rating)
                    : a.OrderBy(a => a.Rating),
                ComicSortOptions.FAVORITE => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.FavoriteNum)
                    : a.OrderBy(a => a.FavoriteNum),
                ComicSortOptions.DATE => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.DayReadNum)
                    : a.OrderBy(a => a.DayReadNum),
                ComicSortOptions.WEEK => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.WeekReadNum)
                    : a.OrderBy(a => a.WeekReadNum),
                ComicSortOptions.MONTH => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.MonthReadNum)
                    : a.OrderBy(a => a.MonthReadNum),
                ComicSortOptions.TOTAL => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.TotalReadNum)
                    : a.OrderBy(a => a.TotalReadNum),
                ComicSortOptions.NOVEL => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.IsNovel)
                    : a.OrderBy(a => a.IsNovel),
                ComicSortOptions.STATUS => getComicReq.IsDescending
                    ? a.OrderByDescending(a => a.Status)
                    : a.OrderBy(a => a.Status),
                _ => a.OrderBy(a => a.Id)
            };

            var data = await GetAsync(
                filter: search,
                orderBy: order,
                pageNumber: pageNum,
                pageSize: pageIndex);

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return (data, pagination);
        }

        //Get Comic By Id
        public async Task<Comic?> GetComicByIdAsync(int id)
        {
            try
            {
                return await _context.Comics.FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Comic By Id With Category
        public async Task<ComicRes?> GetComicByIdNoTrackingAsync(int id)
        {
            try
            {
                return await _context.Comics
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Include(c => c.Chapters)
                .Include(c => c.Comiccategories)
                    .ThenInclude(cc => cc.Category)
                .Select(c => new ComicRes
                {
                    Id = id,
                    Name = c.Name,
                    Description = c.Description,
                    Author = c.Author,
                    Category = c.Comiccategories
                        .Select(cc => new CateNameRes
                        {
                            Name = cc.Category.Name
                        })
                        .ToList(),
                    ReleaseDate = c.ReleaseDate,
                    UpdateTime = c.UpdateTime,
                    ThumbnailUrl = c.ThumbnailUrl,
                    Rating = c.Rating,
                    RateNum = c.RateNum,
                    ChapNum = c.ChapNum,
                    FavoriteNum = c.FavoriteNum,
                    DayReadNum = c.DayReadNum,
                    WeekReadNum = c.WeekReadNum,
                    MonthReadNum = c.MonthReadNum,
                    TotalReadNum = c.TotalReadNum,
                    IsNovel = c.IsNovel,
                    Status = c.Status
                })
                .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Create Comic
        public async Task CreateComicAsync(Comic comic)
        {
            await InsertAsync(comic);
        }

        //Update Comic
        public async Task UpdateComicAsync(Comic comic)
        {
            await UpdateAsync(comic);
        }

        //Delete Comic
        public async Task DeleteComicAsync(int id)
        {
            await DeleteAsync(id);
        }

        //Check If Comic Is Existed
        public async Task<bool> CheckComicExistedAsync(string name, string author)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c =>
                    EF.Functions.Like(c.Name, $"%{name}%") &&
                    EF.Functions.Like(c.Author, $"%{author}%"));
        }

        //Check If Comic Is Existed By Id
        public async Task<bool> CheckComicIdAsync(int id)
        {
            return await _context.Comics
                .AsNoTracking()
                .AnyAsync(c => c.Id == id);
        }

        //Get All Comic Ids
        public async Task<int[]> GetComicIdsAsync()
        {
            return await _context.Comics
                .AsNoTracking()
                .Select(c => c.Id)
                .ToArrayAsync();
        }
    }
}