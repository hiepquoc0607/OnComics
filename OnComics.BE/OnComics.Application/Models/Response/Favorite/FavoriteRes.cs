namespace OnComics.Application.Models.Response.Favorite
{
    public class FavoriteRes
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public Guid ComicId { get; set; }

        public string ComicName { get; set; } = null!;
    }
}
