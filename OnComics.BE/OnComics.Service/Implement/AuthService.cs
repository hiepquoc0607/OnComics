using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Auth;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.Auth;
using OnComics.Library.Models.Response.General;
using OnComics.Library.Utils.Constants;
using OnComics.Library.Utils.Utils;
using OnComics.Repository.Interface;
using OnComics.Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnComics.Service.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly Util _util;
        private static readonly DateTime _time = DateTime.UtcNow.AddHours(12);

        public AuthService(
            IAccountRepository accountRepository,
            IConfiguration configuration,
            IMapper mapper,
            Util util)
        {
            _accountRepository = accountRepository;
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

            var account = await _accountRepository.GetAccountByEmailAsync(email);

            if (account == null) return new ObjectResponse<AuthRes?>("Error", 404, "Email Not Found!");

            if (!_util.CompareHashedPassword(loginReq.Password, account.PasswordHash))
                return new ObjectResponse<AuthRes?>("Error", 401, "Incorrect Password!");

            var expiredSeccond = CaculateExpiredSeccond();

            account.RefreshToken = GenerateRefreshToken();
            account.TokenExpireTime = DateTime.UtcNow.AddDays(7);

            var data = _mapper.Map<AuthRes>(account);
            data.Token = GenerateToken(account);
            data.TokenExpiredIn = expiredSeccond;

            try
            {
                await _accountRepository.UpdateAccountAsync(account);

                return new ObjectResponse<AuthRes?>("Success", 200, "Login Successfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes?>("Error", 400, "Login Fail!, Error Message:\n\n" + ex);
            }
        }

        //Register
        public async Task<ObjectResponse<AccountRes>> RegisterAsync(RegisterReq registerReq)
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
                return new ObjectResponse<AccountRes>("Error", 400, message);

            var isEmailExisted = await _accountRepository.IsEmailExistedAsync(registerReq.Email);

            if (isEmailExisted) return new ObjectResponse<AccountRes>("Error", 400, "Email Has Already Been Used!");

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
                await _accountRepository.CreateAccountAsync(newAccount);

                var newAcc = await _accountRepository.GetAccountByIdAsync(newAccount.Id, false);

                if (newAcc == null) return new ObjectResponse<AccountRes>("Error", 400, "Register Fail!");

                var data = _mapper.Map<AccountRes>(newAcc);

                return new ObjectResponse<AccountRes>("Success", 200, "Register Successfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AccountRes>("Error", 400, "Register Fail!, Message Error:\n\n" + ex);
            }
        }

        public async Task<ObjectResponse<AuthRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq)
        {
            var account = await _accountRepository.GetAccountByIdAsync(refreshTokenReq.AccountId, true);

            if (account == null) return new ObjectResponse<AuthRes>("Error", 400, "Account Not Found!");

            if (account.RefreshToken == null || account.TokenExpireTime <= DateTime.UtcNow)
                return new ObjectResponse<AuthRes>("Error", 401, "Token Is Expired!");

            var token = GenerateToken(account);
            var newRefreshToken = GenerateRefreshToken();
            var expiredSeccond = CaculateExpiredSeccond();

            account.RefreshToken = newRefreshToken;
            account.TokenExpireTime = DateTime.UtcNow.AddDays(7);

            var data = _mapper.Map<AuthRes>(account);
            data.Token = token;
            data.TokenExpiredIn = expiredSeccond;

            try
            {
                await _accountRepository.UpdateAccountAsync(account);

                return new ObjectResponse<AuthRes>("Success", 200, "Refresh Token Succesfully!", data);
            }
            catch (Exception ex)
            {
                return new ObjectResponse<AuthRes>("Error", 400, "Refresh Token Fail!, Error Message:\n\n" + ex);
            }
        }
    }
}
