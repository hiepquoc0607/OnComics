using OnComics.Application.Models.Response.Category;
using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Comic
{
    public class ComicRes
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<CateNameRes> Category { get; set; } = new List<CateNameRes>();

        public DateTime ReleaseDate { get; set; }

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