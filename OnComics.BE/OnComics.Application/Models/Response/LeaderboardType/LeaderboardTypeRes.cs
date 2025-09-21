using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.LeaderboardType
{
    public class LeaderboardTypeRes
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        public string Status { get; set; } = null!;
    }
}
