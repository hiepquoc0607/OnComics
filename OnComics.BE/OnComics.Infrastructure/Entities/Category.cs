namespace OnComics.Infrastructure.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Comiccategory> Comiccategories { get; set; } = new List<Comiccategory>();
}
