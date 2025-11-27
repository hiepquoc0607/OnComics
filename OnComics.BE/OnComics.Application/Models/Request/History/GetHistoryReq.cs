using OnComics.Application.Enums.History;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.History
{
    public class GetHistoryReq : GetReq, IValidatableObject
    {
        [DefaultValue(null)]
        public Guid? Id { get; set; }

        [DefaultValue(null)]
        public HistoryIdType? IdType { get; set; }

        [DefaultValue(null)]
        public HistorySortOption? SortBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id.HasValue && !IdType.HasValue)
            {
                yield return new ValidationResult(
                    "IdType Is Required When Id Is Provided.",
                    new[] { nameof(IdType) }
                );
            }
        }
    }
}
