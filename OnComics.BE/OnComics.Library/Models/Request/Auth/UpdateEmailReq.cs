using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Auth
{
    public class UpdateEmailReq
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
