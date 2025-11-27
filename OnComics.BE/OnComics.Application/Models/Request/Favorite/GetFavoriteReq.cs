using OnComics.Application.Enums.Favorite;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Favorite
{
    public class GetFavoriteReq : GetReq, IValidatableObject
    {
        [DefaultValue(null)]
        public Guid? Id { get; set; }

        [DefaultValue(null)]
        public FavoriteIdType? IdType { get; set; }

        [DefaultValue(null)]
        public FavoriteSortOption? SortBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id.HasValue && IdType == null)
            {
                yield return new ValidationResult(
                    "IdType Is Required When Id Is Provided.",
                    new[] { nameof(IdType) }
                );
            }
        }
    }
}
