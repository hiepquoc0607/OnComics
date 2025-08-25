using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Auth
{
    public enum Gender
    {
        MALE,
        FEMALE
    }

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
        public DateTime Dob { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}
