using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Auth
{
    public class AuthRes
    {
        public Guid Id { get; set; }

        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public DateOnly? Dob { get; set; }

        public string Gender { get; set; } = null!;

        public string? ImgUrl { get; set; }

        public bool IsGoogle { get; set; }

        public bool IsVerified { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefeshExpireTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fcmtoken { get; set; }

        public string Role { get; set; } = null!;

        public string Token { get; set; } = null!;

        public int TokenExpiredIn { get; set; }
    }
}
