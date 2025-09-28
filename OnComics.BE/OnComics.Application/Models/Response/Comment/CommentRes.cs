namespace OnComics.Application.Models.Response.Comment
{
    public class CommentRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int ComicId { get; set; }

        public string Content { get; set; } = null!;

        public ulong IsMainCmt { get; set; }

        public int? MainCmtId { get; set; }

        public DateTime CmtTime { get; set; }
    }
}
