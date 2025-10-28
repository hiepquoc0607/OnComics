namespace OnComics.Infrastructure.Entities;

public partial class Interaction
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid CommentId { get; set; }

    public Guid TypeId { get; set; }

    public DateTime ReactTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comment Comment { get; set; } = null!;

    public virtual Interactiontype Type { get; set; } = null!;
}
