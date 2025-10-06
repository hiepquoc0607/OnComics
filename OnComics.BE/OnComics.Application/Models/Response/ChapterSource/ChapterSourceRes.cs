namespace OnComics.Application.Models.Response.ChapterSource
{
    public class ChapterSourceRes
    {
        public int Id { get; set; }

        public int ChapterId { get; set; }

        public string SrcUrl { get; set; } = null!;

        public string ViewUrl { get; set; } = null!;

        public int Arrangement { get; set; }

        public bool IsImage { get; set; }
    }
}
