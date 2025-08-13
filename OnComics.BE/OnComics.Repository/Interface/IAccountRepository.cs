using OnComics.Library.Model.Data;
using OnComics.Library.Model.Request.Account;
using OnComics.Library.Model.Response.General;

namespace OnComics.Repository.Interface
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<(IEnumerable<Account>?, Pagination)> GetAccountsAsync(GetAccReq getAccReq);

        Task<Account?> GetAccountByIdAsync(int id, bool isTracking);

        Task CreateAccountAsync(Account acc);

        Task UpdateAccountAsync(Account acc);

        Task RemoveAccountAsync(int id);

        Task<bool> IsEmailExistedAsync(string email);

        Task<int> GetAccountMaxIdAsync();

        Task<int> CountAccountAsync();
    }
}
