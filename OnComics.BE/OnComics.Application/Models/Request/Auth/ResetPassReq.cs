using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class ResetPassReq
    {
        [Required]
        [DefaultValue("string")]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DefaultValue("string")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
