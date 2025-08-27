using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.General;

namespace OnComics.Repository.Interface
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<(IEnumerable<Account>?, Pagination)> GetAccountsAsync(GetAccountReq getAccReq);

        Task<Account?> GetAccountByIdAsync(int id, bool isTracking);

        Task<Account?> GetAccountByEmailAsync(string email, bool isTracking);

        Task CreateAccountAsync(Account acc);

        Task UpdateAccountAsync(Account acc);

        Task RemoveAccountAsync(int id);

        Task<bool> IsEmailExistedAsync(string email);

        Task<int> GetAccountMaxIdAsync();

        Task<int> CountAccountAsync();
    }
}
