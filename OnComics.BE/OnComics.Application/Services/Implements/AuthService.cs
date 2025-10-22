using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using OnComics.Application.Constants;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Auth;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Entities;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace OnComics.Application.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Util _util;
        private static readonly DateTime _time = DateTime.UtcNow.AddHours(12);

        public AuthService(
            IAccountRepository accountRepository,
            IMailService mailService,
            IConfiguration configuration,
            IMapper mapper,
            Util util)
        {
            _accountRepository = accountRepository;
            _mailService = mailService;
            _configuration = configuration;
            _mapper = mapper;
            _util = util;
        }

        //Generate JWT Token
        private string GenerateToken(Account account)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role),
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("Authentication:Jwt:Key").Value!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Authentication:Jwt:Issuer"],
                audience: _configuration["Authentication:Jwt:Audience"],
                claims: claims,
                expires: _time,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Generate Refresh Token
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        //Caculate Token Expired Time In Seccond
        private int CaculateExpiredSeccond()
        {
            return (int)(_time - DateTime.Now).TotalSeconds;
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

                if (account.IsGoogle == false)
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Account Authenticate By Google!");

                if (!_util.CompareHashedPassword(loginReq.Password, account.PasswordHash!))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.Unauthorized,
                        "Incorrect Password!");

                var expiredSeccond = CaculateExpiredSeccond();

                account.RefreshToken = GenerateRefreshToken();
                account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                var data = _mapper.Map<AuthRes>(account);
                data.Token = GenerateToken(account);
                data.TokenExpiredIn = expiredSeccond;

                await _accountRepository.UpdateAsync(account);

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
                if (string.IsNullOrWhiteSpace(code))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Missing 'code' In Query String!");

                var clientId = _configuration["Authentication:Google:ClientId"];
                var clientSecret = _configuration["Authentication:Google:ClientSecret"];
                var redirectUri = _configuration["Authentication:Google:ReturnUrl"];

                if (string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(clientSecret) ||
                string.IsNullOrEmpty(redirectUri))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Google Configuration Missing!");

                var tokenResponse = await httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code"
                }));

                var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JObject.Parse(tokenResponseJson);
                var accessToken = tokenData.Value<string>("access_token");

                if (string.IsNullOrEmpty(accessToken))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Failed To Obtain Access Token From Google!");

                var credential = GoogleCredential.FromAccessToken(accessToken);

                var peopleService = new PeopleServiceService(
                    new BaseClientService.Initializer
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = "MyAppName"
                    });

                var request = peopleService.People.Get("people/me");
                request.PersonFields = "names,emailAddresses,photos";

                var person = await request.ExecuteAsync();

                string? fullname = person.Names?.FirstOrDefault()?.DisplayName;
                string? email = person.EmailAddresses?
                        .Select(e => e.Value)
                        .Where(e => !string.IsNullOrEmpty(e))
                        .FirstOrDefault();
                string? imgUrl = person.Photos?
                        .Select(p => p.Url)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .FirstOrDefault();

                if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email))
                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.BadRequest,
                        "Fail To Get Google Person Profile Information!");

                var account = await _accountRepository.GetAccountByEmailAsync(email, true);

                if (account == null)
                {
                    var newAccount = new Account();
                    newAccount.Email = email;
                    newAccount.Fullname = fullname;
                    newAccount.ImgUrl = imgUrl;
                    newAccount.IsGoogle = true;
                    newAccount.IsVerified = true;
                    newAccount.RefreshToken = GenerateRefreshToken();
                    newAccount.RefreshExpireTime = DateTime.UtcNow.AddDays(7);
                    newAccount.Role = RoleConstant.USER;
                    newAccount.Status = StatusConstant.ACTIVE;

                    await _accountRepository.InsertAsync(newAccount);

                    var expiredSeccond = CaculateExpiredSeccond();

                    var data = _mapper.Map<AuthRes>(newAccount);
                    data.Token = GenerateToken(newAccount);
                    data.TokenExpiredIn = expiredSeccond;

                    return new ObjectResponse<AuthRes?>(
                        (int)HttpStatusCode.OK,
                        "Login Successfully!",
                        data);
                }
                else
                {
                    var expiredSeccond = CaculateExpiredSeccond();

                    account.IsVerified = true;
                    account.RefreshToken = GenerateRefreshToken();
                    account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                    var data = _mapper.Map<AuthRes>(account);
                    data.Token = GenerateToken(account);
                    data.TokenExpiredIn = expiredSeccond;

                    await _accountRepository.UpdateAsync(account);

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
                newAccount.RefreshToken = GenerateRefreshToken();

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
        public async Task<ObjectResponse<AuthRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq)
        {
            try
            {
                var account = await _accountRepository
                    .GetByIdAsync(refreshTokenReq.AccountId, true);

                if (account == null)
                    return new ObjectResponse<AuthRes>(
                        (int)HttpStatusCode.NotFound,
                        "Account Not Found!");

                if (account.RefreshToken == null ||
                    account.RefreshExpireTime <= DateTime.UtcNow)
                    return new ObjectResponse<AuthRes>(
                        (int)HttpStatusCode.Unauthorized,
                        "Token Is Expired!");

                var token = GenerateToken(account);
                var newRefreshToken = GenerateRefreshToken();
                var expiredSeccond = CaculateExpiredSeccond();

                account.RefreshToken = newRefreshToken;
                account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

                var data = _mapper.Map<AuthRes>(account);
                data.Token = token;
                data.TokenExpiredIn = expiredSeccond;

                await _accountRepository.UpdateAsync(account);

                return new ObjectResponse<AuthRes>(
                    (int)HttpStatusCode.OK,
                    "Refresh Token Succesfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes>(
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

                account.RefreshToken = GenerateRefreshToken();
                account.RefreshExpireTime = DateTime.UtcNow.AddSeconds(180);

                //Reset Password API URL
                string url = _configuration["AppReturnUrl:ResetPassword"]! +
                    $"AccountId={account.Id}&Token={account.RefreshToken}";

                await _accountRepository.RunTransactionAsync(async () =>
                {
                    await _accountRepository.UpdateAsync(account);

                    await _mailService.SendEmailAsync(email, "OnComics Reset Password Link:", url);
                });

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
                    !account.RefreshToken.Equals(infoQuery.Token))
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "Incorect Token!");

                if (account.RefreshExpireTime <= DateTime.UtcNow)
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "Token Is Expried!");

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
        public async Task<VoidResponse> RequestConfirmEmailAsync(int id)
        {
            try
            {
                var account = await _accountRepository.GetByIdAsync(id);

                if (account == null)
                    return new VoidResponse(
                        (int)HttpStatusCode.NotFound,
                        "Email Not Found!");

                //Confirm Email API URL
                string url = _configuration["AppReturnUrl:ComfirmEmail"]! +
                    $"AccountId={account.Id}&Token={account.RefreshToken}";

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

                if (account.RefreshToken != infoQuery.Token)
                    return new VoidResponse(
                        (int)HttpStatusCode.Unauthorized,
                        "Invalid Request Token!");

                if (account.IsVerified)
                    return new VoidResponse(
                        (int)HttpStatusCode.BadRequest,
                        "Email Is Already Verified!");

                account.IsVerified = true;
                account.RefreshToken = string.Empty;
                account.RefreshExpireTime = null;

                await _accountRepository.UpdateAsync(account);

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
