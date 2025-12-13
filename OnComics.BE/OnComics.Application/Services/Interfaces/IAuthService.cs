using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Auth;
using OnComics.Application.Models.Response.Common;
using OnComics.Infrastructure.Entities;

namespace OnComics.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ObjectResponse<AuthRes?>> LoginAsync(LoginReq loginReq);

        Task<ObjectResponse<AuthRes?>> GoogleCallbackAsync(string code, HttpClient httpClient);

        Task<ObjectResponse<Account>> RegisterAsync(RegisterReq registerReq);

        Task<ObjectResponse<TokenRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq);

        Task<VoidResponse> LogoutAsync(Guid id);

        Task<VoidResponse> RequestResetPasswordAsync(string email);

        Task<VoidResponse> ResetPasswordAsync(InfoQuery infoQuery, ResetPassReq resetPassReq);

        Task<VoidResponse> RequestConfirmEmailAsync(Guid id);

        Task<VoidResponse> ConfirmEmailAsync(InfoQuery infoQuery);
    }
}
