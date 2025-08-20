using Microsoft.EntityFrameworkCore;
using OnComics.Library.Model.Data;
using OnComics.Library.Model.Request.Account;
using OnComics.Library.Model.Response.General;
using OnComics.Repository.Interface;
using System.Linq.Expressions;

namespace OnComics.Repository.Implement
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Account>?, Pagination)> GetAccountsAsync(GetAccReq getAccReq)
        {
            string? searchKey = getAccReq.SearchKey;
            bool isDescending = getAccReq.IsDescending;
            int pageNum = getAccReq.PageNum;
            int pageIndex = getAccReq.PageIndex;
            var totalData = await _context.Accounts.AsNoTracking().CountAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getAccReq.PageIndex);

            Expression<Func<Account, bool>>? search = a =>
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(a.Email, $"%{searchKey}%")) &&
                (string.IsNullOrEmpty(searchKey) || EF.Functions.Like(a.Fullname, $"%{searchKey}%"));

            Func<IQueryable<Account>, IOrderedQueryable<Account>>? order = a => getAccReq.SortBy switch
            {
                AccSortOption.EMAIL => getAccReq.IsDescending
                                ? a.OrderByDescending(a => a.Email)
                                : a.OrderBy(a => a.Email),
                AccSortOption.FULLNAME => getAccReq.IsDescending
                              ? a.OrderByDescending(a => a.Fullname)
                              : a.OrderBy(a => a.Fullname),
                AccSortOption.STATUS => getAccReq.IsDescending
                              ? a.OrderByDescending(a => a.Status)
                              : a.OrderBy(a => a.Status),
                _ => a.OrderBy(a => a.Id)
            };

            var data = await GetAsync(
                filter: search,
                orderBy: order,
                pageNumber: pageNum,
                pageSize: pageIndex,
                true);

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return (data, pagination);
        }

        public async Task<Account?> GetAccountByIdAsync(int id, bool isTracking)
        {
            try
            {
                switch (isTracking)
                {
                    case true:
                        return await _context.Accounts.SingleOrDefaultAsync(a => a.Id == id);
                    default:
                        return await _context.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            try
            {
                return await _context.Accounts.AsNoTracking().SingleOrDefaultAsync(a => a.Email.Equals(email));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task CreateAccountAsync(Account acc)
        {
            await InsertAsync(acc);
        }

        public async Task UpdateAccountAsync(Account acc)
        {
            await UpdateAsync(acc);
        }

        public async Task RemoveAccountAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<int> CountAccountAsync()
        {
            return await _context.Accounts.AsNoTracking().CountAsync();
        }

        public async Task<int> GetAccountMaxIdAsync()
        {
            return await _context.Accounts.AsNoTracking().MaxAsync(a => a.Id);
        }

        public async Task<bool> IsEmailExistedAsync(string email)
        {
            return await _context.Accounts.AsNoTracking().AnyAsync(a => a.Email.Equals(email));
        }
    }
}
