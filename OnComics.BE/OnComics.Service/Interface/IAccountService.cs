using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.General;

namespace OnComics.Service.Interface
{
    public interface IAccountService
    {
        Task<ObjectResponse<IEnumerable<AccountRes>?>> GetAccountsAsync(GetAccountReq getAccReq);

        Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(int id);

        //Task<ObjectResponse<AccRes?>> GetAccountByEmailAsync(string email);

        Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccountReq updateAccReq);

        Task<VoidResponse> DeleteAccountAsync(int id);
    }
}
