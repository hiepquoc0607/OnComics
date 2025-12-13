using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.Auth
{
    public class RefreshTokenReq
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
