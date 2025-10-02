using OnComics.Application.Enums.Interaction;
using OnComics.Application.Models.Request.Common;

namespace OnComics.Application.Models.Request.Interaction
{
    public class GetInteractionReq : GetReq
    {
        public int? AccountId { get; set; }

        public InteractionSortOption? SortBy { get; set; }
    }
}
