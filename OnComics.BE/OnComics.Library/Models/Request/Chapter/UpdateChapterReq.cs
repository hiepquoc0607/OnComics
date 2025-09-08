namespace OnComics.Library.Models.Request.Chapter
{
    public class UpdateChapterReq
    {
        public int ComicId { get; set; }

        public int ChapNo { get; set; }

        public string? Name { get; set; }
    }
}
