namespace OnComics.Infrastructure.Entities;

public partial class Comiccategory
{
    public Guid Id { get; set; }

    public Guid ComicId { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;
}
