using MapsterMapper;
using Microsoft.Extensions.Configuration;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Auth;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace OnComics.Application.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMailService _mailService;
        private readonly IGoogleService _googleService;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Util _util;

        private static string cacheKey = "accounts:{id}";

        public AuthService(
            IAccountRepository accountRepository,
            IMailService mailService,
            IGoogleService googleService,
            IRedisService redisService,
            IConfiguration configuration,
            IMapper mapper, Util util)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _googleService = googleService;
            _redisService = redisService;
            _configuration = configuration;
            _mapper = mapper;
            _util = util;
        }

        //Login
        public async Task<ObjectResponse<AuthRes?>> LoginAsync(LoginReq loginReq)
        {
            try
            {
                var email = loginReq.Email.ToLowerInvariant();

                var account = await _accountRepository.GetAccountByEmailAsync(email, true);

                if (account == null)
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.NotFound,
                        "Email Not Found!");

                if (account.IsGoogle == true)
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Account Authenticate By Google!");

                if (!_util.CompareHashedPassword(loginReq.Password, account.PasswordHash!))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.Unauthorized,
                        "Incorrect Password!");

                account.RefreshToken = TokenGenerator.GenerateRefreshToken();
                account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                var data = _mapper.Map<AuthRes>(account);
                data.AccessToken = TokenGenerator.GenerateAccessToken(account, _configuration);
                data.AccessExpiresInMinutes = _configuration.GetValue<int>("Authentication:Jwt:ExpiresinMinutes");

                await _accountRepository.UpdateAsync(account);

                string key = cacheKey.Replace("{id}", account.Id.ToString());

                await _redisService.RemoveAsync(key);

                return new ObjectResponse<AuthRes?>(
                    (int)HttpStatusCode.OK,
                    "Login Successfully!",
                    data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Google Callback
        public async Task<ObjectResponse<AuthRes?>> GoogleCallbackAsync(string code, HttpClient httpClient)
        {
            try
            {
                var googleProfile = await _googleService.GetGoogleProfileAsync(code, httpClient);

                string? fullname = googleProfile.Fullname;
                string? email = googleProfile.Email;
                string? imgUrl = googleProfile.PictureUrl;

                if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Fail To Get Google Person Profile Information!");

                var account = await _accountRepository.GetAccountByEmailAsync(email, true);

                if (account == null)
                {
                    var newAccount = _mapper.Map<Account>(googleProfile);
                    newAccount.Dob = null;
                    newAccount.RefreshToken = TokenGenerator.GenerateRefreshToken();
                    newAccount.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                    await _accountRepository.InsertAsync(newAccount);

                    var data = _mapper.Map<AuthRes>(newAccount);
                    data.AccessToken = TokenGenerator.GenerateAccessToken(newAccount, _configuration);
                    data.AccessExpiresInMinutes = _configuration.GetValue<int>("Authentication:Jwt:ExpiredInMinute");

                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.OK,
                        "Login Successfully!",
                        data);
                }
                else
                {
                    account.IsVerified = true;
                    account.RefreshToken = TokenGenerator.GenerateRefreshToken();
                    account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                    var data = _mapper.Map<AuthRes>(account);
                    data.AccessToken = TokenGenerator.GenerateAccessToken(account, _configuration);
                    data.AccessExpiresInMinutes = _configuration.GetValue<int>("Authentication:Jwt:ExpiredInMinute");

                    await _accountRepository.UpdateAsync(account);

                    string key = cacheKey.Replace("{id}", account.Id.ToString());

                    await _redisService.RemoveAsync(key);

                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.OK,
                        "Login Successfully!",
                        data);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes?>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Register
        public async Task<ObjectResponse<Account>> RegisterAsync(RegisterReq registerReq)
        {
            try
            {
                var passError = _util.CheckPasswordErrorType(registerReq.Password);

                var passErrorMessage = new Dictionary<string, string>
                {
                    { "Number", "Password Must Have At Least One Number Digit!" },
                    { "Lower", "Password Must Have At Least One Lowercase Digit!" },
                    { "Upper", "Password Must Have At Least One Uppercase Digit!" },
                    { "Special", "Password Must Have At Least One Special  Digit (!, @, #, $,...)!" },
                };

                if (passErrorMessage.TryGetValue(passError, out var message))
                    return new ObjectResponse<Account>(
                        (int)HttpStatusCode.BadRequest,
                        message);

                var isEmailExisted = await _accountRepository
                    .CheckEmailExistedAsync(registerReq.Email);

                if (isEmailExisted)
                    return new ObjectResponse<Account>(
                        (int)HttpStatusCode.BadRequest,
                        "Email Has Already Been Used!");

                var newAccount = _mapper.Map<Account>(registerReq);
                newAccount.Fullname = _util.FormatStringName(newAccount.Fullname);
                newAccount.Gender = registerReq.Gender switch
                {
                    Gender.MALE => "Male",
                    _ => "Female"
                };
                newAccount.PasswordHash = _util.HashPassword(registerReq.Password);

                await _accountRepository.InsertAsync(newAccount);

                return new ObjectResponse<Account>(
                    (int)HttpStatusCode.Created,
                    "Register Successfully!",
                    newAccount);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Account>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Refresh Login Token
        public async Task<ObjectResponse<TokenRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq)
        {
            try
            {
                var account = await _accountRepository
                    .GetAccountByTokenAsync(refreshTokenReq.RefreshToken);

                if (account == null)
                    return new ObjectResponse<TokenRes>(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                if (account.RefreshExpireTime <= DateTime.UtcNow)
                    return new ObjectResponse<TokenRes>(
                        (int)HttpStatusCode.Unauthorized,
                        "Invalid Refresh RefreshToken!");

                var token = TokenGenerator.GenerateAccessToken(account, _configuration);
                var newRefreshToken = TokenGenerator.GenerateRefreshToken();

                account.RefreshToken = newRefreshToken;
                account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                await _accountRepository.UpdateAsync(account);

                string key = cacheKey.Replace("{id}", account.Id.ToString());

                await _redisService.RemoveAsync(key);

                var tokenRes = new TokenRes()
                {
                    AccessToken = token,
                    AccessExpiresInMinutes = _configuration.GetValue<int>("Authentication:Jwt:ExpiredInMinute"),
                    RefreshToken = newRefreshToken,
                    RefreshExpiresInUtc = (DateTime)account.RefreshExpireTime,
                };

                return new ObjectResponse<TokenRes>(
                    (int)HttpStatusCode.OK,
                    "Refresh Token Succesfully!",
                    tokenRes);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<TokenRes>(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Logout
        public async Task<VoidResponse> LogoutAsync(Guid id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                account.RefreshToken = null;
                account.RefreshExpireTime = null;

                await _accountRepository.UpdateAsync(account);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Logout Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Reset (Forgot) Password
        public async Task<VoidResponse> RequestResetPasswordAsync(string email)
        {
            try
            {
                var account = await _accountRepository.GetAccountByEmailAsync(email, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Email Not Found!");

                account.RefreshToken = TokenGenerator.GenerateRefreshToken();
                account.RefreshExpireTime = DateTime.UtcNow.AddSeconds(180);

                //Reset Password API URL
                string url = _configuration["AppDefaultUrl:ResetPassword"]! +
                    $"Id={account.Id}&RefreshToken={account.RefreshToken}";

                await _accountRepository.UpdateAsync(account);

                await _mailService.SendEmailAsync(email, "OnComics Reset Password Link:", url);

                string key = cacheKey.Replace("{id}", account.Id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Send Reset Password Email Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Reset Password
        public async Task<VoidResponse> ResetPasswordAsync(InfoQuery infoQuery, ResetPassReq resetPassReq)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(infoQuery.AccountId, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Email Not Found!");

                if (!string.IsNullOrEmpty(account.RefreshToken) &&
                    !account.RefreshToken.Equals(infoQuery.RefreshToken))
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "Incorect RefreshToken!");

                if (account.RefreshExpireTime <= DateTime.UtcNow)
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "RefreshToken Is Expried!");

                var passError = _util.CheckPasswordErrorType(resetPassReq.NewPassword);

                var passErrorMessage = new Dictionary<string, string>
                {
                    { "Number", "Password Must Have At Least One Number Digit!" },
                    { "Lower", "Password Must Have At Least One Lowercase Digit!" },
                    { "Upper", "Password Must Have At Least One Uppercase Digit!" },
                    { "Special", "Password Must Have At Least One Special  Digit (!, @, #, $,...)!" },
                };

                if (passErrorMessage.TryGetValue(passError, out var message))
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        message);

                account.PasswordHash = _util.HashPassword(resetPassReq.NewPassword);
                account.RefreshToken = String.Empty;
                account.RefreshExpireTime = null;

                await _accountRepository.UpdateAsync(account);

                var key = cacheKey.Replace("{id}", account.Id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Change Password Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Send Confirm Email Request
        public async Task<VoidResponse> RequestConfirmEmailAsync(Guid id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id, false);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Email Not Found!");

                //Confirm Email API URL
                string url = _configuration["AppDefaultUrl:ComfirmEmail"]! +
                    $"Id={account.Id}&RefreshToken={account.RefreshToken}";

                await _mailService.SendEmailAsync(
                    account.Email,
                    "OnComics Email Verification Link:",
                    url);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Send Confirm Email Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse(
                    (int)HttpStatusCode.InternalServerError,
                    ex.GetType().FullName!,
                    ex.Message);
            }
        }

        //Confirm Email
        public async Task<VoidResponse> ConfirmEmailAsync(InfoQuery infoQuery)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(infoQuery.AccountId, true);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                if (account.RefreshToken != infoQuery.RefreshToken)
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "Invalid Request RefreshToken!");

                if (account.IsVerified)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Email Is Already Verified!");

                account.IsVerified = true;
                account.RefreshToken = string.Empty;
                account.RefreshExpireTime = null;

                await _accountRepository.UpdateAsync(account);

                string key = cacheKey.Replace("{id}", account.Id.ToString());

                await _redisService.RemoveAsync(key);

                return new VoidResponse(
                    (int)HttpStatusCode.OK,
                    "Confirm Email Successfully!");
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
