namespace OnComics.Infrastructure.Entities;

public partial class Favorite
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid ComicId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;
}
