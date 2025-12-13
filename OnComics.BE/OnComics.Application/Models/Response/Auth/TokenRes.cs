namespace OnComics.Application.Models.Response.Auth
{
    public class TokenRes
    {
        public string AccessToken { get; set; } = string.Empty;

        public int AccessExpiresInMinutes { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshExpiresInUtc { get; set; }
    }
}
