namespace OnComics.Application.Models.Response.Interaction
{
    public class InteractionRes
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public Guid CommentId { get; set; }

        public string CommentAuthor { get; set; } = null!;

        public Guid TypeId { get; set; }

        public DateTime ReactTime { get; set; }
    }
}
