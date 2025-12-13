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
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        //Get Accocunt By Refresh Token
        public async Task<Account?> GetAccountByTokenAsync(string token)
        {
            try
            {
                return await _context.Accounts
                    .Where(a =>
                        !string.IsNullOrEmpty(a.RefreshToken) &&
                        a.RefreshToken.Equals(token))
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Check If Account Is Existed By Idd
        public async Task<bool> CheckAccIdExistedAsync(Guid id)
        {
            try
            {
                return await _context.Accounts
                    .AsNoTracking()
                    .AnyAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Check If Email is Existed
        public async Task<bool> CheckEmailExistedAsync(string email)
        {
            try
            {
                return await _context.Accounts
                    .AsNoTracking()
                    .AnyAsync(a => a.Email.Equals(email));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
