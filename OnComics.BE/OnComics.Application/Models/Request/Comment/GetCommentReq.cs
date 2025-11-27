using OnComics.Application.Enums.Comment;
using OnComics.Application.Models.Request.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Comment
{
    public class GetCommentReq : GetReq, IValidatableObject
    {
        [DefaultValue(null)]
        public Guid? Id { get; set; }

        [DefaultValue(null)]
        public CmtIdType? IdType { get; set; }

        [DefaultValue(null)]
        public CmtSortOption? SortBy { get; set; }

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
