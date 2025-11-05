using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class RefreshTokenReq
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        [Required]
        public DateTime RefreshExpireTime { get; set; }
    }
}
