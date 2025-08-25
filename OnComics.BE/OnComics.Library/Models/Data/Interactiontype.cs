namespace OnComics.Library.Models.Data;

public partial class Interactiontype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public string Status { get; set; } = null!;
}
