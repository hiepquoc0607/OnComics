using OnComics.Application.Models.Response.Category;
using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Comic
{
    public class ComicRes
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<CateNameRes>? Categories { get; set; } = null;

        public DateOnly ReleaseDate { get; set; }

        public DateTime UpdateTime { get; set; }

        public string ThumbnailUrl { get; set; } = null!;

        public decimal Rating { get; set; }

        public int RateNum { get; set; }

        public int ChapNum { get; set; }

        public int FavoriteNum { get; set; }

        public int DayReadNum { get; set; }

        public int WeekReadNum { get; set; }

        public int MonthReadNum { get; set; }

        public int TotalReadNum { get; set; }

        public bool IsNovel { get; set; }

        public string Status { get; set; } = null!;
    }
}