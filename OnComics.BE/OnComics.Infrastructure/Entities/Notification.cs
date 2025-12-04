namespace OnComics.Infrastructure.Entities;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid ChapterId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SendTime { get; set; }

    public bool IsRead { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;
}
