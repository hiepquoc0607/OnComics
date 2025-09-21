namespace OnComics.Infrastructure.Domains;

public partial class Leaderboard
{
    public int Id { get; set; }

    public int ComicId { get; set; }

    public int TypeId { get; set; }

    public int RankNo { get; set; }

    public virtual Comic Comic { get; set; } = null!;

    public virtual Leaderboardtype Type { get; set; } = null!;
}
