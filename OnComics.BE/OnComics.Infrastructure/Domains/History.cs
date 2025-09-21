namespace OnComics.Infrastructure.Domains;

public partial class History
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ChapterId { get; set; }

    public DateTime ReadTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Chapter Chapter { get; set; } = null!;
}
