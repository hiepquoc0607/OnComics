using OnComics.Application.Enums.Account;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class RegisterReq
    {
        [Required]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public DateOnly Dob { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
