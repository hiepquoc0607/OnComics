using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.InteractionType
{
    public class UpdateItrTypeReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? ImgUrl { get; set; }
    }
}
