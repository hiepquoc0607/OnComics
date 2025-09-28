using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.ComicRating
{
    public class ComicRatingRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public int ComicId { get; set; }

        public string ComicName { get; set; } = null!;

        public decimal Rating { get; set; }
    }
}
