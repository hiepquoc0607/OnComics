using Mapster;
using OnComics.Library.Model.Data;
using OnComics.Library.Model.Request.Account;
using OnComics.Library.Model.Response.Account;

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
