namespace OnComics.Infrastructure.Entities;

public partial class Chaptersource
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string SrcUrl { get; set; } = null!;

    public int Arrangement { get; set; }

    public bool IsImage { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;
}
