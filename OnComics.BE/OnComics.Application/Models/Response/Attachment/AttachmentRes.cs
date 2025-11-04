namespace OnComics.Application.Models.Response.Attachment
{
    public class AttachmentRes
    {
        public Guid Id { get; set; }

        public Guid CommentId { get; set; }

        public string StorageUrl { get; set; } = null!;
    }
}
