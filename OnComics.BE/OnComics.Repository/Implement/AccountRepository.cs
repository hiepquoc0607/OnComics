using Microsoft.EntityFrameworkCore;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.Api;
using OnComics.Repository.Interface;
using System.Linq.Expressions;

namespace OnComics.Repository.Implement
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get Accounts
        public async Task<(IEnumerable<Account>?, Pagination)> GetAccountsAsync(GetAccountReq getAccReq)
        {
            string? searchKey = getAccReq.SearchKey;
            bool isDescending = getAccReq.IsDescending;

            int pageNum = getAccReq.PageNum;
            int pageIndex = getAccReq.PageIndex;
            var totalData = await _context.Accounts.AsNoTracking().CountAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getAccReq.PageIndex);

            Expression<Func<Account, bool>>? search = a =>
                (!string.IsNullOrEmpty(searchKey) || EF.Functions.Like(a.Email, $"%{searchKey}%")) &&
                (!string.IsNullOrEmpty(searchKey) || EF.Functions.Like(a.Fullname, $"%{searchKey}%"));

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
                pageSize: pageIndex);

            var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

            return (data, pagination);
        }

        //Get Account By Id
        public async Task<Account?> GetAccountByIdAsync(int id, bool isTracking)
        {
            try
            {
                switch (isTracking)
                {
                    case true:
                        return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
                    default:
                        return await _context.Accounts
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Id == id);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Acount By Email
        public async Task<Account?> GetAccountByEmailAsync(string email, bool isTracking)
        {
            try
            {
                switch (isTracking)
                {
                    case true:
                        return await _context.Accounts.FirstOrDefaultAsync(a => a.Email.Equals(email));
                    default:
                        return await _context.Accounts
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Email.Equals(email));
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Create Account
        public async Task CreateAccountAsync(Account acc)
        {
            await InsertAsync(acc);
        }

        //Update Account
        public async Task UpdateAccountAsync(Account acc)
        {
            await UpdateAsync(acc);
        }

        //Delete Account
        public async Task DeleteAccountAsync(int id)
        {
            await DeleteAsync(id);
        }

        //Count Total Account Record
        public async Task<int> CountAccountAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .CountAsync();
        }

        //Get Highest Account Id Value
        public async Task<int> GetAccountMaxIdAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .MaxAsync(a => a.Id);
        }

        //Check If Email Is Existed
        public async Task<bool> CheckEmailExistedAsync(string email)
        {
            return await _context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Email.Equals(email));
        }
    }
}