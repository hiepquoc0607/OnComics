using System.ComponentModel.DataAnnotations;

namespace OnComics.Library.Models.Request.Auth
{
    public class RefreshTokenReq
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime RefreshExpireTime { get; set; }
    }
}
