namespace OnComics.Infrastructure.Entities;

public partial class Attachment
{
    public Guid Id { get; set; }

    public Guid ComicId { get; set; }

    public string StrorageUrl { get; set; } = null!;

    public virtual Comment Comic { get; set; } = null!;
}
