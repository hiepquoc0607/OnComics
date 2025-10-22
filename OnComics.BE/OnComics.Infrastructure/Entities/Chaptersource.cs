namespace OnComics.Infrastructure.Entities;

public partial class Chaptersource
{
    public int Id { get; set; }

    public int ChapterId { get; set; }

    public string FileId { get; set; } = null!;

    public string SrcUrl { get; set; } = null!;

    public string? ViewUrl { get; set; }

    public int Arrangement { get; set; }

    public bool IsImage { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;
}
