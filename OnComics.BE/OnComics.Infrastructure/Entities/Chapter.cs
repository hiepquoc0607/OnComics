namespace OnComics.Infrastructure.Entities;

public partial class Chapter
{
    public Guid Id { get; set; }

    public Guid ComicId { get; set; }

    public int ChapNo { get; set; }

    public string? Name { get; set; }

    public int ReadNum { get; set; }

    public DateOnly ReleaseTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Chaptersource> Chaptersources { get; set; } = new List<Chaptersource>();

    public virtual Comic Comic { get; set; } = null!;

    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
