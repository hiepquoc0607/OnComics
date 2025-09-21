using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class ResetPassReq
    {
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
