using System.Text.Json.Serialization;

namespace OnComics.Library.Models.Response.Account
{
    public class AccountRes
    {
        public int Id { get; set; }

        public string Fullname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string Gender { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ImgUrl { get; set; }

        public ulong IsGoogle { get; set; }

        public ulong IsVerified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? RefreshToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? TokenExpireTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Fcmtoken { get; set; }

        public string Role { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
