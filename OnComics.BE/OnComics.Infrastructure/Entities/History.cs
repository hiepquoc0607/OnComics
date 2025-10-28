namespace OnComics.Infrastructure.Entities;

public partial class History
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid ChapterId { get; set; }

    public DateTime ReadTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Chapter Chapter { get; set; } = null!;
}
