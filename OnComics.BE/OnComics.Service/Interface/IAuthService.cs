using OnComics.Library.Models.Request.Auth;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.Auth;
using OnComics.Library.Models.Response.General;

namespace OnComics.Service.Interface
{
    public interface IAuthService
    {
        Task<ObjectResponse<AuthRes?>> LoginAsync(LoginReq loginReq);

        Task<ObjectResponse<AccountRes>> RegisterAsync(RegisterReq registerReq);

        Task<ObjectResponse<AuthRes>> RefreshTokenAsync(RefreshTokenReq refreshTokenReq);
    }
}
