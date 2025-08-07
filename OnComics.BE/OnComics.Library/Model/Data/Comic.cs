namespace OnComics.Library.Model.Data;

public partial class Comic
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Author { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public DateTime UpdateTime { get; set; }

    public string ThumbnailUrl { get; set; } = null!;

    public decimal Rating { get; set; }

    public int RateNum { get; set; }

    public int ChapNum { get; set; }

    public int FavoriteNum { get; set; }

    public int DayReadNum { get; set; }

    public int WeekReadNum { get; set; }

    public int MonthReadNum { get; set; }

    public int TotalReadNum { get; set; }

    public ulong IsNovel { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<Comiccategory> Comiccategories { get; set; } = new List<Comiccategory>();

    public virtual ICollection<Comicrating> Comicratings { get; set; } = new List<Comicrating>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Favortite> Favortites { get; set; } = new List<Favortite>();

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();
}
