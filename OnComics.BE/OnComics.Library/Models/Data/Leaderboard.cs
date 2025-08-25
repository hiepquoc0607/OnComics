namespace OnComics.Library.Models.Data;

public partial class Leaderboard
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public int TypeId { get; set; }

    public int OrderNum { get; set; }

    public virtual Comic Comic { get; set; } = null!;

    public virtual Leaderboardtype Type { get; set; } = null!;
}
