using Mapster;
using OnComics.Application.Constants;
using OnComics.Application.Models.Request.Account;
using OnComics.Application.Models.Request.Auth;
using OnComics.Application.Models.Response.Account;
using OnComics.Application.Models.Response.Auth;
using OnComics.Infrastructure.Domains;

namespace OnComics.Application.Mappers
{
    public class AccountMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Account, AccountRes>()
                .Map(dest => dest.Dob, src => DateOnly.FromDateTime(src.Dob));

            config.NewConfig<UpdateAccountReq, Account>()
                .Map(dest => dest.Dob, src => src.Dob.ToDateTime(TimeOnly.MinValue));

            config.NewConfig<Account, AuthRes>()
                .Map(dest => dest.Dob, src => DateOnly.FromDateTime(src.Dob));

            config.NewConfig<RegisterReq, Account>()
                .Map(dest => dest.PasswordHash, src => src.Password)
                .Map(dest => dest.Dob, src => src.Dob.ToDateTime(TimeOnly.MinValue))
                .Map(dest => dest.ImgUrl, otp => string.Empty)
                .Map(dest => dest.IsGoogle, otp => false)
                .Map(dest => dest.IsVerified, otp => false)
                .Map(dest => dest.RefreshToken, otp => string.Empty)
                .Map(dest => dest.RefreshExpireTime, otp => DateTime.UtcNow)
                .Map(dest => dest.Fcmtoken, otp => string.Empty)
                .Map(dest => dest.Role, otp => RoleConstant.USER)
                .Map(dest => dest.Status, otp => StatusConstant.ACTIVE);
        }
    }
}
