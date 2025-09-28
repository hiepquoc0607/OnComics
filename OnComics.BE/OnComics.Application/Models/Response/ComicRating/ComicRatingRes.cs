using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.ComicRating
{
    public class ComicRatingRes
    {
        public int Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AccountId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fullname { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ComicId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ComicName { get; set; } = null;

        public decimal Rating { get; set; }
    }
}
