using OnComics.Infrastructure.Domains;

namespace OnComics.Infrastructure.Repositories.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetAccountByEmailAsync(string email, bool isTracking = false);

        Task<Dictionary<int, string>> GetFullnameByIdsAsync(int[] ids);

        Task<bool> CheckEmailExistedAsync(string email);
    }
}
