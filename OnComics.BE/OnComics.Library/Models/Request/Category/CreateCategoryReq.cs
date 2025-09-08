using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Category
{
    public class CreateCategoryReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
