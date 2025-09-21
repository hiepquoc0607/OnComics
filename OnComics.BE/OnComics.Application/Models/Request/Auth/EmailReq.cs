using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class EmailReq
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
