namespace OnComics.Library.Models.Request.Comic
{
    public class UpdateComicReq
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Author { get; set; } = null!;

        public DateTime ReleaseDate { get; set; }

        public string ThumbnailUrl { get; set; } = null!;

        public bool IsNovel { get; set; }
    }
}