namespace OnComics.Infrastructure.Domains;

public partial class Chapter
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public int ChapNo { get; set; }

    public string? Name { get; set; }

    public int ReadNum { get; set; }

    public DateTime ReleaseTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;

    public virtual ICollection<History> Histories { get; set; } = new List<History>();
}
