namespace OnComics.Infrastructure.Entities;

public partial class Interactiontype
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}
