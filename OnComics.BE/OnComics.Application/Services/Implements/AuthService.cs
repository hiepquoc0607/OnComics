using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Auth;
using OnComics.Application.Models.Response.Common;
using OnComics.Application.Services.Interfaces;
using OnComics.Application.Utils;
using OnComics.Infrastructure.Domains;
using OnComics.Infrastructure.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
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

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
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
            var email = loginReq.Email.ToLowerInvariant();

            var account = await _accountRepository.GetAccountByEmailAsync(email, true);

            if (account == null) return new ObjectResponse<AuthRes?>("Error", 404, "Email Not Found!");

            if (!_util.CompareHashedPassword(loginReq.Password, account.PasswordHash))
                return new ObjectResponse<AuthRes?>("Error", 401, "Incorrect Password!");

            var expiredSeccond = CaculateExpiredSeccond();

            account.RefreshToken = GenerateRefreshToken();
            account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

            var data = _mapper.Map<AuthRes>(account);
            data.Token = GenerateToken(account);
            data.TokenExpiredIn = expiredSeccond;

            try
            {
                await _accountRepository.UpdateAsync(account);

                return new ObjectResponse<AuthRes?>("Success", 200, "Login Successfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes?>("Error", 400, "Login Fail!, Error Message:\n\n" + ex);
            }
        }

        //Register
        public async Task<ObjectResponse<Account>> RegisterAsync(RegisterReq registerReq)
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
                return new ObjectResponse<Account>("Error", 400, message);

            var isEmailExisted = await _accountRepository.CheckEmailExistedAsync(registerReq.Email);

            if (isEmailExisted) return new ObjectResponse<Account>("Error", 400, "Email Has Already Been Used!");

            var newAccount = _mapper.Map<Account>(registerReq);
            newAccount.Fullname = _util.FormatStringName(newAccount.Fullname);
            newAccount.Gender = registerReq.Gender switch
            {
                Gender.MALE => "Male",
                _ => "Female"
            };
            newAccount.PasswordHash = _util.HashPassword(registerReq.Password);
            newAccount.RefreshToken = GenerateRefreshToken();

            try
            {
                await _accountRepository.InsertAsync(newAccount);

                return new ObjectResponse<Account>("Success", 200, "Register Successfully!", newAccount);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<Account>("Error", 400, "Register Fail!, Message Error:\n\n" + ex);
            }
        }

        //Refresh Login Token
        public async Task<ObjectResponse<AuthRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq)
        {
            var account = await _accountRepository.GetByIdAsync(refreshTokenReq.AccountId, true);

            if (account == null) return new ObjectResponse<AuthRes>("Error", 400, "Account Not Found!");

            if (account.RefreshToken == null || account.RefreshExpireTime <= DateTime.UtcNow)
                return new ObjectResponse<AuthRes>("Error", 401, "Token Is Expired!");

            var token = GenerateToken(account);
            var newRefreshToken = GenerateRefreshToken();
            var expiredSeccond = CaculateExpiredSeccond();

            account.RefreshToken = newRefreshToken;
            account.RefreshExpireTime = DateTime.UtcNow.AddDays(7);

            var data = _mapper.Map<AuthRes>(account);
            data.Token = token;
            data.TokenExpiredIn = expiredSeccond;

            try
            {
                await _accountRepository.UpdateAsync(account);

                return new ObjectResponse<AuthRes>("Success", 200, "Refresh Token Succesfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes>("Error", 400, "Refresh Token Fail!, Error Message:\n\n" + ex);
            }
        }

        //Reset (Forgot) Password
        public async Task<VoidResponse> RequestResetPasswordAsync(string email)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email, true);

            if (account == null) return new VoidResponse("Error", 404, "Email Not Found!");

            account.RefreshToken = GenerateRefreshToken();
            account.RefreshExpireTime = DateTime.UtcNow.AddSeconds(180);

            //Reset Password API URL
            string url = _configuration["AppReturnUrl:ResetPassword"]! + $"AccountId={account.Id}&Token={account.RefreshToken}";

            try
            {
                await _accountRepository.RunTransactionAsync(async () =>
                {
                    await _accountRepository.UpdateAsync(account);

                    await _mailService.SendEmailAsync(email, "OnComics Reset Password Link:", url);
                });

                return new VoidResponse("Success", 200, "Send Reset Password Email Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Request Reset Password Fail!, Error Message:\n\n" + ex);
            }
        }

        //Reset Password
        public async Task<VoidResponse> ResetPasswordAsync(InfoQuery infoQuery, ResetPassReq resetPassReq)
        {
            var account = await _accountRepository.GetByIdAsync(infoQuery.AccountId, true);

            if (account == null) return new VoidResponse("Error", 404, "Email Not Found!");

            if (!string.IsNullOrEmpty(account.RefreshToken) &&
                !account.RefreshToken.Equals(infoQuery.Token))
                return new VoidResponse("Error", 401, "Incorect Token!");

            if (account.RefreshExpireTime <= DateTime.UtcNow)
                return new VoidResponse("Error", 401, "Token Is Expried!");

            var passError = _util.CheckPasswordErrorType(resetPassReq.NewPassword);

            var passErrorMessage = new Dictionary<string, string>
            {
                { "Number", "Password Must Have At Least One Number Digit!" },
                { "Lower", "Password Must Have At Least One Lowercase Digit!" },
                { "Upper", "Password Must Have At Least One Uppercase Digit!" },
                { "Special", "Password Must Have At Least One Special  Digit (!, @, #, $,...)!" },
            };

            if (passErrorMessage.TryGetValue(passError, out var message))
                return new VoidResponse("Error", 400, message);

            account.PasswordHash = _util.HashPassword(resetPassReq.NewPassword);
            account.RefreshToken = String.Empty;
            account.RefreshExpireTime = null;

            try
            {
                await _accountRepository.UpdateAsync(account);

                return new VoidResponse("Success", 200, "Change Password Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Change Password Fail!, Error Message:\n\n" + ex);
            }
        }

        //Send Confirm Email Request
        public async Task<VoidResponse> RequestConfirmEmailAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            if (account == null) return new VoidResponse("Error", 404, "Email Not Found!");

            //Confirm Email API URL
            string url = _configuration["AppReturnUrl:ComfirmEmail"]! + $"AccountId={account.Id}&Token={account.RefreshToken}";

            try
            {
                await _mailService.SendEmailAsync(account.Email, "OnComics Email Verification Link:", url);

                return new VoidResponse("Success", 200, "Send Confirm Email Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Request Confirm Email Fail!, Error Message:\n\n" + ex);
            }
        }

        //Confirm Email
        public async Task<VoidResponse> ConfirmEmailAsync(InfoQuery infoQuery)
        {
            var account = await _accountRepository.GetByIdAsync(infoQuery.AccountId, true);

            if (account == null) return new VoidResponse("Error", 404, "Account Not Found!");

            if (account.RefreshToken != infoQuery.Token)
                return new VoidResponse("Error", 401, "Invalid Request Token!");

            if (account.IsVerified) return new VoidResponse("Error", 400, "Email Is Already Verified!");

            account.IsVerified = true;
            account.RefreshToken = string.Empty;
            account.RefreshExpireTime = null;

            try
            {
                await _accountRepository.UpdateAsync(account);

                return new VoidResponse("Success", 200, "Confirm Email Successfully!");
            }
            catch (Exception ex)
            {
                return new VoidResponse("Error", 400, "Confirm Email Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
