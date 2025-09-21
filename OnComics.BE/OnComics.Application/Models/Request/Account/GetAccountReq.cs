using OnComics.Application.Enums.Account;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.Account
{
    public class GetAccountReq : GetReq
    {
        [DefaultValue(null)]
        public AccountSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public AccountStatus? Status { get; set; }
    }
}
