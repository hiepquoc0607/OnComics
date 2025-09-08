namespace OnComics.Library.Models.Request.Chapter
{
    public class CreateChapterReq
    {
        public int ComicId { get; set; }

        public string? Name { get; set; } = string.Empty;
    }
}
