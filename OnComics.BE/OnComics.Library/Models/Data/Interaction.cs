namespace OnComics.Library.Models.Data;

public partial class Interaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int CommentId { get; set; }

    public int TypeId { get; set; }

    public DateTime ReactTime { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Comment Comment { get; set; } = null!;

    public virtual Leaderboardtype Type { get; set; } = null!;
}
