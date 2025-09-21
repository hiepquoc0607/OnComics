namespace OnComics.Infrastructure.Domains;

public partial class Account
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public bool IsGoogle { get; set; }

    public bool IsVerified { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshExpireTime { get; set; }

    public string? Fcmtoken { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Comicrating> Comicratings { get; set; } = new List<Comicrating>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}
