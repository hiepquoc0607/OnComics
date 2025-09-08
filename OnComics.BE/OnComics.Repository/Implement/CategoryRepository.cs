using Microsoft.EntityFrameworkCore;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Category;
using OnComics.Library.Models.Response.Api;
using OnComics.Library.Utils.Constants;
using OnComics.Repository.Interface;
using System.Linq.Expressions;

namespace OnComics.Repository.Implement
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get All Category
        public async Task<(IEnumerable<Category>?, Pagination)> GetCategoriesAsync(GetCategoryReq getCategoryReq)
        {
            string? searchKey = getCategoryReq.SearchKey;

            string? status = getCategoryReq.Status switch
            {
                CateStatus.ACTIVE => StatusConstant.ACTIVE,
                CateStatus.INACTIVE => StatusConstant.INACTIVE,
                _ => null
            };

            bool isDescending = getCategoryReq.IsDescending;

            int pageNum = getCategoryReq.PageNum;
            int pageIndex = getCategoryReq.PageIndex;
            var totalData = await _context.Categories.AsNoTracking().CountAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getCategoryReq.PageIndex);

            Expression<Func<Category, bool>>? search = c =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(c.Name, $"%{searchKey}%")) &&
                (string.IsNullOrEmpty(status) || c.Status.Equals(status));

            Func<IQueryable<Category>, IOrderedQueryable<Category>>? order = c => getCategoryReq.SortBy switch
            {
                CateSortOption.NAME => getCategoryReq.IsDescending
                    ? c.OrderByDescending(c => c.Name)
                    : c.OrderBy(c => c.Name),
                CateSortOption.STATUS => getCategoryReq.IsDescending
                    ? c.OrderByDescending(c => c.Status)
                    : c.OrderBy(c => c.Status),
                _ => c.OrderBy(c => c.Id)
            };

            var data = await GetAsync(
                filter: search,
                orderBy: order,
                pageNumber: pageNum,
                pageSize: pageIndex);

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return (data, pagination);
        }

        //Get Category By Id
        public async Task<Category?> GetCategoryByIdAsync(int id, bool isTracking)
        {
            try
            {
                switch (isTracking)
                {
                    case true:
                        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
                    default:
                        return await _context.Categories
                            .AsNoTracking()
                            .FirstOrDefaultAsync(c => c.Id == id);
                }
            }
            catch
            {
                return null;
            }
        }

        //Create Category
        public async Task CreateCategoryAsync(Category category)
        {
            await InsertAsync(category);
        }

        //Bulk (Range) Create Category
        public async Task CreateCategoriesAsync(IEnumerable<Category> categories)
        {
            await InsertRangeAsync(categories);
        }

        //Update Category
        public async Task UpdateCategoryAsync(Category category)
        {
            await UpdateAsync(category);
        }

        //Delete Category
        public async Task DeleteCategoryAsync(int id)
        {
            await DeleteAsync(id);
        }

        //Check If Category Is Existed
        public async Task<bool> CheckCategoryIsExistedAsync(string name)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Name.Equals(name));
        }

        public async Task<string[]> GetCateNamesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => c.Name)
                .ToArrayAsync();
        }
    }
}