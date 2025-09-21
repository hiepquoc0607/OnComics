namespace OnComics.Infrastructure.Domains;

public partial class Comment
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int ComicId { get; set; }

    public string Content { get; set; } = null!;

    public ulong IsMainCmt { get; set; }

    public int? MainCmtId { get; set; }

    public DateTime CmtTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comic Comic { get; set; } = null!;

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}
