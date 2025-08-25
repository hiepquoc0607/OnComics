namespace OnComics.Library.Models.Data;

public partial class Chaptersource
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public string SrcUrl { get; set; } = null!;

    public int Arrangement { get; set; }

    public bool IsEditable { get; set; }

    public virtual Comic Comic { get; set; } = null!;
}
