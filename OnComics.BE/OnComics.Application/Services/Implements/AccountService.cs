using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.General;
using OnComics.Application.Models.Response.Account;
using OnComics.Application.Models.Response.Appwrite;
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
        private readonly IAppwriteService _appwriteService;
        private readonly IRedisService _redisService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Util _util;

        private static string cacheKey = "accounts:{id}";

        public AccountService(
            IAccountRepository accountRepository,
            IAppwriteService appwriteService,
            IRedisService redisService,
            IMapper mapper,
            IConfiguration configuration,
            Util util)
        {
            _accountRepository = accountRepository;
            _appwriteService = appwriteService;
            _redisService = redisService;
            _mapper = mapper;
            _configuration = configuration;
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

                var totalData = await _accountRepository.CountRecordAsync(search);
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
        public async Task<ObjectResponse<AccountRes?>> GetAccountByIdAsync(Guid id)
        {
            try
            {
                string key = cacheKey.Replace("{id}", id.ToString());

                var accCache = await _redisService.GetAsync<AccountRes>(key);

                if (accCache is not null)
                {
                    return new ObjectResponse<AccountRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        accCache);
                }
                else
                {
                    var account = await _accountRepository.GetByIdAsync(id, false);

                    if (account == null)
                        return new ObjectResponse<AccountRes?>(
                            (int)HttpStatusCode.NotFound,
                            "Account Not Found!");

                    var data = account.Adapt<AccountRes>();

                    await _redisService.SetAsync<AccountRes>(key, data, TimeSpan.FromMinutes(10));

                    return new ObjectResponse<AccountRes?>(
                        (int)HttpStatusCode.OK,
                        "Fetch Data Successfully!",
                        data);
                }
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
        public async Task<VoidResponse> UpdateAccountAsync(Guid id, UpdateAccountReq updateAccReq)
        {
            try
            {
                bool validDob = _util.CheckDob(updateAccReq.Dob);

                if (!validDob)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Date Of Birth Must Be At Least 13 Year Old!");

                var oldAccount = await _accountRepository.GetByIdAsync(id, true);

                if (oldAccount == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                var newAccount = _mapper.Map(updateAccReq, oldAccount);
                newAccount.Fullname = _util.FormatStringName(newAccount.Fullname);

                await _accountRepository.UpdateAsync(newAccount);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

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

        //Update Profile Picture
        public async Task<VoidResponse> UpdateProfileImageAsync(Guid id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "No File Uploaded!");

                if (!file.ContentType.Contains("image"))
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Invalid Picture File Format!");

                var oldAccount = await _accountRepository.GetByIdAsync(id, true);

                if (oldAccount == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                string? imgUrl = oldAccount.ImgUrl;
                string fileName = oldAccount.Id.ToString();

                var fileRes = new FileRes();

                if (string.IsNullOrEmpty(imgUrl) ||
                    imgUrl.Equals(_configuration["AppReturnUrl:DefaultProfileUrl"]))
                {
                    fileRes = await _appwriteService
                        .CreateProfileFileAsync(file, fileName);
                }

                fileRes = await _appwriteService
                    .UpdateProfileFileAsync(oldAccount.Id.ToString(), file, fileName);

                oldAccount.ImgUrl = fileRes.Url;

                await _accountRepository.UpdateAsync(oldAccount);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Update Profile Picture Successfully!");
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
        public async Task<VoidResponse> UpdatePasswordAsync(Guid id, UpdatePasswordReq updatePasswordReq)
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

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

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
        public async Task<VoidResponse> UpdateStatusAsync(Guid id, UpdateStatusReq<AccountStatus> updateStatusReq)
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

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

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
        public async Task<VoidResponse> DeleteAccountAsync(Guid id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                await _accountRepository.DeleteAsync(account);

                string key = cacheKey.Replace("{id}", id.ToString());

                await _redisService.RemoveAsync(key);

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
