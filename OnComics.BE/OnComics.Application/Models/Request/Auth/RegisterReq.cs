using OnComics.Application.Enums.Account;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class RegisterReq
    {
        [Required]
        [MaxLength(100)]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DefaultValue("string")]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DefaultValue("string")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public DateOnly Dob { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
