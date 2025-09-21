using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Auth;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ObjectResponse<AuthRes?>> LoginAsync(LoginReq loginReq);

        Task<ObjectResponse<Account>> RegisterAsync(RegisterReq registerReq);

        Task<ObjectResponse<AuthRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq);

        Task<VoidResponse> RequestResetPasswordAsync(string email);

        Task<VoidResponse> ResetPasswordAsync(InfoQuery infoQuery, ResetPassReq resetPassReq);

        Task<VoidResponse> RequestConfirmEmailAsync(int id);

        Task<VoidResponse> ConfirmEmailAsync(InfoQuery infoQuery);
    }
}
