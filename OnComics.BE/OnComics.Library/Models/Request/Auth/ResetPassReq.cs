using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Auth
{
    public class ResetPassReq
    {
        [Required]
        [MinLength(8)]
        [DefaultValue("string")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
