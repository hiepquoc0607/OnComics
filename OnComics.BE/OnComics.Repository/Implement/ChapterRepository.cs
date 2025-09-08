using Microsoft.EntityFrameworkCore;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Chapter;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Utils.Constants;
using OnComics.Repository.Interface;
using System.Linq.Expressions;

namespace OnComics.Repository.Implement
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        public ChapterRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Chapters By Comic Id
        public async Task<(IEnumerable<Chapter>?, Pagination)> GetChaptersAsync(GetChapterReq getChapterReq)
        {
            string? status = getChapterReq.Status switch
            {
                ChapStatus.ACTIVE => StatusConstant.ACTIVE,
                ChapStatus.INACTIVE => StatusConstant.INACTIVE,
                ChapStatus.ERROR => StatusConstant.ERROR,
                _ => null
            };

            bool isDescending = getChapterReq.IsDescending;

            int pageNum = getChapterReq.PageNum;
            int pageIndex = getChapterReq.PageIndex;
            var totalData = await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == getChapterReq.ComicId)
                .CountAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getChapterReq.PageIndex);

            var query = _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == getChapterReq.ComicId)
                .AsQueryable();

            Expression<Func<Chapter, bool>>? search = c =>
                c.ComicId == getChapterReq.ComicId &&
                (string.IsNullOrEmpty(status) || c.Status.Equals(status));

            Func<IQueryable<Chapter>, IOrderedQueryable<Chapter>>? order = c => getChapterReq.IsDescending switch
            {
                true => c.OrderByDescending(c => c.ChapNo),
                _ => c.OrderBy(c => c.ChapNo),
            };

            var data = await GetAsync(
                filter: search,
                orderBy: order,
                pageNumber: pageNum,
                pageSize: pageIndex);

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return (data, pagination);
        }

        //Get Chapter By Id
        public async Task<Chapter?> GetChapterByIdAsync(int id, bool isTracking)
        {
            try
            {
                switch (isTracking)
                {
                    case true:
                        return await _context.Chapters.FirstOrDefaultAsync(c => c.Id == id);
                    default:
                        return await _context.Chapters
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == id);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Create Chapter
        public async Task CreateChapterAsync(Chapter chapter)
        {
            await InsertAsync(chapter);
        }

        // Bulk (Range) Create Chapters
        public async Task CreateChaptersAsync(IEnumerable<Chapter> chapters)
        {
            await InsertRangeAsync(chapters);
        }

        //Update Chapter
        public async Task UpdateChapterAsync(Chapter chapter)
        {
            await UpdateAsync(chapter);
        }

        //Delete Chapter
        public async Task DeleteChapterAsync(int id)
        {
            await DeleteAsync(id);
        }

        //Get Max Chapter ChapNo By ComicId
        public async Task<int> GetMaxChapNoByComicIdAsync(int id)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Where(c => c.ComicId == id)
                .MaxAsync(c => c.ChapNo);
        }

        //Get Max Chapter ChapNo For Each ComicIds
        public async Task<Dictionary<int, int>> GetMaxChapNosByComicIdsAsync(int[] ids)
        {
            return await _context.Chapters
                .AsNoTracking()
                .GroupBy(c => c.ComicId)
                .ToDictionaryAsync(
                    c => c.Key,
                    c => c.Max(ct => ct.ChapNo));
        }
    }
}