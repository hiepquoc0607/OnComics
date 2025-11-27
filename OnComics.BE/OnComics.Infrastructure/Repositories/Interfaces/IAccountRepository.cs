using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetAccountByEmailAsync(string email, bool isTracking);

        Task<bool> CheckAccIdExistedAsync(Guid id);

        Task<bool> CheckEmailExistedAsync(string email);
    }
}
