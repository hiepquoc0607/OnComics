using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Account
{
    public class UpdateAccountReq
    {
        [Required]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        public DateOnly Dob { get; set; }

        public string? ImgUrl { get; set; } = string.Empty;
    }
}