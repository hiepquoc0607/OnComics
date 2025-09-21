using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Account;
using OnComics.Application.Models.Response.Common;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ObjectResponse<IEnumerable<AccountRes>?>> GetAccountsAsync(GetAccountReq getAccReq);

        Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(int id);

        Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccountReq updateAccReq);

        Task<VoidResponse> UpdatePasswordAsync(int id, UpdatePasswordReq updatePasswordReq);

        Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<AccountStatus> updateStatusReq);

        Task<VoidResponse> DeleteAccountAsync(int id);
    }
}
