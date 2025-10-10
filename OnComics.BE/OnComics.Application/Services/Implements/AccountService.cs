using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Account;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace OnComics.Application.Services.Implements
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
            try
            {
                string? searchKey = getAccReq.SearchKey;

                string? status = getAccReq.Status switch
                {
                    AccountStatus.ACTIVE => StatusConstant.ACTIVE,
                    AccountStatus.INACTIVE => StatusConstant.INACTIVE,
                    _ => null
                };

                bool isDescending = getAccReq.IsDescending;

                int pageNum = getAccReq.PageNum;
                int pageIndex = getAccReq.PageIndex;


                Expression<Func<Account, bool>>? search = a =>
                    (string.IsNullOrEmpty(searchKey) || (EF.Functions.Like(a.Email, $"%{searchKey}%") ||
                                                        EF.Functions.Like(a.Fullname, $"%{searchKey}%"))) &&
                    (string.IsNullOrEmpty(status) || a.Status.Equals(status));

                Func<IQueryable<Account>, IOrderedQueryable<Account>>? order = a => getAccReq.SortBy switch
                {
                    AccountSortOption.EMAIL => isDescending
                        ? a.OrderByDescending(a => a.Email)
                        : a.OrderBy(a => a.Email),
                    AccountSortOption.FULLNAME => isDescending
                        ? a.OrderByDescending(a => a.Fullname)
                        : a.OrderBy(a => a.Fullname),
                    AccountSortOption.STATUS => isDescending
                        ? a.OrderByDescending(a => a.Status)
                        : a.OrderBy(a => a.Status),
                    _ => a.OrderBy(a => a.Id)
                };

                var accounts = await _accountRepository.GetAsync(search, order, pageNum, pageIndex);

                if (accounts == null)
                    return new ObjectResponse<IEnumerable<AccountRes>?>(
                        (int)HttpStatusCode.NotFound,
                        "Account Data Empty!");

                var data = accounts.Adapt<IEnumerable<AccountRes>>();

                var totalData = await _accountRepository.CountRecordAsync();
                int totalPage = (int)Math.Ceiling((decimal)totalData / getAccReq.PageIndex);
                var pagination = new Pagination(totalData, pageIndex, pageNum, totalPage);

                return new ObjectResponse<IEnumerable<AccountRes>?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data,
                    pagination);

            }
            catch (Exception ex)
            {
                return new ObjectResponse<IEnumerable<AccountRes>?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Get Account By Id
        public async Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id);

                if (account == null)
                    return new ObjectResponse<AccountRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                var data = account.Adapt<AccountRes>();

                return new ObjectResponse<AccountRes?>(
                    (int)HttpStatusCode.OK,
                    "Fetch Data Successfully!",
                    data);

            }
            catch (Exception ex)
            {
                return new ObjectResponse<AccountRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Account
        public async Task<VoidResponse> UpdateAccountAsync(int id, UpdateAccountReq updateAccReq)
        {
            try
            {
                var oldAccount = await _accountRepository.GetByIdAsync(id, true);

                if (oldAccount == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                var newAccount = _mapper.Map(updateAccReq, oldAccount);
                newAccount.Fullname = _util.FormatStringName(newAccount.Fullname);


                await _accountRepository.UpdateAsync(newAccount);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Account Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Password
        public async Task<VoidResponse> UpdatePasswordAsync(int id, UpdatePasswordReq updatePasswordReq)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                var newPass = _util.HashPassword(updatePasswordReq.NewPassword);

                account.PasswordHash = newPass;
                account.RefreshToken = string.Empty;
                account.RefreshExpireTime = null;

                await _accountRepository.UpdateAsync(account);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update New Password Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Update Status
        public async Task<VoidResponse> UpdateStatusAsync(int id, UpdateStatusReq<AccountStatus> updateStatusReq)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                account.Status = updateStatusReq.Status switch
                {
                    AccountStatus.INACTIVE => StatusConstant.INACTIVE,
                    _ => StatusConstant.ACTIVE
                };

                await _accountRepository.UpdateAsync(account);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update New Status Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Delete Account
        public async Task<VoidResponse> DeleteAccountAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                await _accountRepository.DeleteAsync(id);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Delete Account Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }
    }
}
