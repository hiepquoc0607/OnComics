using OnComics.Application.Enums.Interaction;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;

namespace OnComics.Application.Models.Request.Interaction
{
    public class GetInteractionReq : GetReq
    {
        [DefaultValue(null)]
        public Guid? AccountId { get; set; }

        [DefaultValue(null)]
        public InteractionSortOption? SortBy { get; set; }
    }
}
