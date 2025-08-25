using Mapster;
using OnComics.Library.Models.Data;
using OnComics.Library.Models.Request.Account;
using OnComics.Library.Models.Response.Account;

namespace OnComics.Library.Utils.Mapper
{
    public class AccountMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Account, AccRes>();

            config.NewConfig<Account, UpdateAccReq>();
        }
    }
}
