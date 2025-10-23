namespace OnComics.Infrastructure.Entities;

public partial class Attachment
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public string FileId { get; set; } = null!;

    public string StrorageUrl { get; set; } = null!;

    public virtual Comment Comic { get; set; } = null!;
}
