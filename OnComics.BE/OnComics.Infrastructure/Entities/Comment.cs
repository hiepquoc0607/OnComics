namespace OnComics.Infrastructure.Entities;

public partial class Comment
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public Guid ComicId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsEdited { get; set; }

    public bool IsMainCmt { get; set; }

    public Guid? MainCmtId { get; set; }

    public DateTime CmtTime { get; set; }

    public int InteractionNum { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Comic Comic { get; set; } = null!;

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}
