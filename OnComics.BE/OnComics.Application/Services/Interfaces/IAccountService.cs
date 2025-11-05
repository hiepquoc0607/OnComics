using Microsoft.AspNetCore.Http;
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

        Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(Guid id);

        Task<VoidResponse> UpdateAccountAsync(Guid id, UpdateAccountReq updateAccReq);

        Task<VoidResponse> UpdateProfileImageAsync(Guid id, IFormFile file);

        Task<VoidResponse> UpdatePasswordAsync(Guid id, UpdatePasswordReq updatePasswordReq);

        Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<AccountStatus> updateStatusReq);

        Task<VoidResponse> DeleteAccountAsync(Guid id);
    }
}
