using Mapster;
using MapsterMapper;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.General;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Interface;
using OnComics.Service.Interface;

namespace OnComics.Service.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public AccountService(
            IAccountRepository accountRepository,
            IMapper mapper,
            Util util)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _util = util;
        }

        //Get All Account
        public async Task<ObjectResponse<IEnumerable<AccountRes>?>> GetAccountsAsync(GetAccountReq getAccReq)
        {
            var (accounts, pagination) = await _accountRepository.GetAccountsAsync(getAccReq);

            if (accounts == null) return new ObjectResponse<IEnumerable<AccountRes>?>("Error", 404, "Account Data Empty!");

            var data = accounts.Adapt<IEnumerable<AccountRes>>();

            return new ObjectResponse<IEnumerable<AccountRes>?>("Success", 200, "Fetch Data Successfully!", data, pagination);
        }

        //Get Account By Id
        public async Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id, false);

            if (account == null) return new ObjectResponse<AccountRes?>("Error", 404, "Account Not Found!");

            var data = account.Adapt<AccountRes>();

            return new ObjectResponse<AccountRes?>("Success", 200, "Fetch Data Successfully!", data);
        }

        //Update Account
        public async Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccountReq updateAccReq)
        {
            var oldAccount = await _accountRepository.GetAccountByIdAsync(id, true);

            if (oldAccount == null) return new VoidResponse("Error", 404, "Account Not Found!");

            var newAccount = _mapper.Map(updateAccReq, oldAccount);
            newAccount.Fullname = _util.FormatStringName(newAccount.Fullname);

            try
            {
                await _accountRepository.UpdateAccountAsync(newAccount);

                return new VoidResponse("Success", 200, "Update Account Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Update Account Fail!, Error Message:\n\n" + ex);
            }
        }

        //Delete Account
        public async Task<VoidResponse> DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id, false);

            if (account == null) return new VoidResponse("Error", 404, "Account Not Found!");

            try
            {
                await _accountRepository.RemoveAccountAsync(id);

                return new VoidResponse("Success", 200, "Delete Account Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Delete Account Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
