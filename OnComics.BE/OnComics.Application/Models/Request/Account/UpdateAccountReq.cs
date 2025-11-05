using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Account
{
    public class UpdateAccountReq
    {
        [Required]
        [DefaultValue("string")]
        [MaxLength(100)]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [DefaultValue("yyyy-mm-dd")]
        public DateOnly Dob { get; set; }
    }
}