namespace OnComics.Application.Models.Response.Interaction
{
    public class InteractionRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public int CommentId { get; set; }

        public string CommentAuthor { get; set; } = null!;

        public int TypeId { get; set; }

        public DateTime ReactTime { get; set; }
    }
}
