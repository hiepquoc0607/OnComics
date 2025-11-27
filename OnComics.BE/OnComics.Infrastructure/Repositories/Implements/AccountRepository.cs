using Microsoft.EntityFrameworkCore;
using OnComics.Infrastructure.Entities;
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
        public async Task<Account?> GetAccountByEmailAsync(string email, bool isTracking)
        {
            switch (isTracking)
            {
                case true:
                    return await _context.Accounts
                        .Where(a => a.Email.Equals(email))
                        .FirstOrDefaultAsync();
                case false:
                    return await _context.Accounts
                        .AsNoTracking()
                        .Where(a => a.Email.Equals(email))
                        .FirstOrDefaultAsync();
            }
        }

        //Check If Account Is Existed By Idd
        public async Task<bool> CheckAccIdExistedAsync(Guid id)
        {
            return await _context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Id == id);
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
