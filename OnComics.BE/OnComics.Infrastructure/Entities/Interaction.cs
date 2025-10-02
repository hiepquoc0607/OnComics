namespace OnComics.Infrastructure.Entities;

public partial class Interaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int CommentId { get; set; }

    public int TypeId { get; set; }

    public DateTime ReactTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comment Comment { get; set; } = null!;

    public virtual Interactiontype Type { get; set; } = null!;
}
