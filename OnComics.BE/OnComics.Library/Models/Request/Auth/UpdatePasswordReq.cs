using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Auth
{
    public class UpdatePasswordReq
    {
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
