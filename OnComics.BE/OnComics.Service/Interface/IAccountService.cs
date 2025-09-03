using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Request.General;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.Api;

namespace OnComics.Service.Interface
{
    public interface IAccountService
    {
        Task<ObjectResponse<IEnumerable<AccountRes>?>> GetAccountsAsync(GetAccountReq getAccReq);

        Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(int id);

        Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccountReq updateAccReq);

        Task<VoidResponse> UpdatePasswordAsync(int id, UpdatePasswordReq updatePasswordReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<AccStatus> updateStatusReq);

        Task<VoidResponse> DeleteAccountAsync(int id);
    }
}