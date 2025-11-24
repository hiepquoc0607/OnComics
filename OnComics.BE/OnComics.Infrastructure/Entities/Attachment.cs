namespace OnComics.Infrastructure.Entities;

public partial class Attachment
{
    public Guid Id { get; set; }

    public Guid CommentId { get; set; }

    public string SrcUrl { get; set; } = null!;

    public virtual Comment Comment { get; set; } = null!;
}
