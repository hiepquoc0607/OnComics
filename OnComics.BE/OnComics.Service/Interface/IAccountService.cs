using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.General;

namespace OnComics.Service.Interface
{
    public interface IAccountService
    {
        Task<ObjectResponse<IEnumerable<AccRes>?>> GetAccountsAsync(GetAccReq getAccReq);

        Task<ObjectResponse<AccRes?>> GetAccountByIdAsync(int id);

        //Task<ObjectResponse<AccRes?>> GetAccountByEmailAsync(string email);

        Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccReq updateAccReq);

        Task<VoidResponse> DeleteAccountAsync(int id);
    }
}
