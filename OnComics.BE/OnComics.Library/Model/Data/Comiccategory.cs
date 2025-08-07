namespace OnComics.Library.Model.Data;

public partial class Comiccategory
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;
}
