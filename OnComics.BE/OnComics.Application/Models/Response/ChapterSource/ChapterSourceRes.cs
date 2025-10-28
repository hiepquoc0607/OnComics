namespace OnComics.Application.Models.Response.ChapterSource
{
    public class ChapterSourceRes
    {
        public Guid Id { get; set; }

        public Guid ChapterId { get; set; }

        public string SrcUrl { get; set; } = null!;

        public string ViewUrl { get; set; } = null!;

        public int Arrangement { get; set; }

        public bool IsImage { get; set; }
    }
}
