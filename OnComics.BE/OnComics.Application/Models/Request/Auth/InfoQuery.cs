using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class InfoQuery
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
