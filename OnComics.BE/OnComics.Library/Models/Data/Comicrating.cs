namespace OnComics.Library.Models.Data;

public partial class Comicrating
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ComicId { get; set; }

    public decimal Rating { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;
}
