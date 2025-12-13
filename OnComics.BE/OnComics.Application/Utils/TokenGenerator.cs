using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OnComics.Infrastructure.Entities;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OnComics.Application.Utils
{
    public static class TokenGenerator
    {
        //Generate Access Token
        public static string GenerateAccessToken(Account account, IConfiguration configuration)
        {
            string jwtKey = configuration["Authentication:Jwt:Key"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity
                ([
                    new Claim (JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                    new Claim (JwtRegisteredClaimNames.Email, account.Email),
                    new Claim(ClaimTypes.Role, account.Role)
                ]),
                Expires = DateTime.UtcNow
                    .AddMinutes(configuration.GetValue<int>("Authentication:Jwt:ExpiresinMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Authentication:Jwt:Issuer"],
                Audience = configuration["Authentication:Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        //Generate Refresh Token
        public static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64]; // 512 bits of entropy

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return Convert.ToBase64String(randomBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

    }
}
