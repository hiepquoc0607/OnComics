namespace OnComics.Infrastructure.Entities;

public partial class Favorite
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ComicId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;
}
