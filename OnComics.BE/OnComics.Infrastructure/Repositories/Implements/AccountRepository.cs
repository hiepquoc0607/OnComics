using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Persistence;
using OnComics.Infrastructure.Repositories.Interfaces;

namespace OnComics.Infrastructure.Repositories.Implements
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(OnComicsDatabaseContext context) : base(context)
        {
        }

        //Get Account By Email
        public async Task<Account?> GetAccountByEmailAsync(string email, bool isTracking = false)
        {
            switch (isTracking)
            {
                case true:
                    return await _context.Accounts
                        .Where(a => a.Email.Equals(email))
                        .FirstOrDefaultAsync();
                default:
                    return await _context.Accounts
                        .AsNoTracking()
                        .Where(a => a.Email.Equals(email))
                        .FirstOrDefaultAsync();
            }
        }

        //Get Fullnames By Each Account Ids
        public async Task<Dictionary<int, string>> GetFullnameByIdsAsync(int[] ids)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Where(a => ids.Contains(a.Id))
                .ToDictionaryAsync(
                    a => a.Id,
                    a => a.Fullname);
        }

        //Check If Email is Existed
        public async Task<bool> CheckEmailExistedAsync(string email)
        {
            return await _context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Email.Equals(email));
        }
    }
}
