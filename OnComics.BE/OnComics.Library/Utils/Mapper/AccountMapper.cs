using Mapster;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Request.Auth;
using OnComics.Library.Models.Response.Account;
using OnComics.Library.Models.Response.Auth;
using OnComics.Library.Utils.Constants;

namespace OnComics.Library.Utils.Mapper
{
    public class AccountMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Account, AccountRes>();

            config.NewConfig<UpdateAccountReq, Account>();

            config.NewConfig<Account, AuthRes>();

            config.NewConfig<RegisterReq, Account>()
                .Map(dest => dest.PasswordHash, src => src.Password)
                .Map(dest => dest.ImgUrl, otp => string.Empty)
                .Map(dest => dest.IsGoogle, otp => false)
                .Map(dest => dest.IsVerified, otp => false)
                .Map(dest => dest.RefreshToken, otp => string.Empty)
                .Map(dest => dest.TokenExpireTime, otp => DateTime.UtcNow)
                .Map(dest => dest.Fcmtoken, otp => string.Empty)
                .Map(dest => dest.Role, otp => RoleConstant.USER)
                .Map(dest => dest.Status, otp => StatusConstant.ACTIVE);
        }
    }
}
