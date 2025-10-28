using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class InfoQuery
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
