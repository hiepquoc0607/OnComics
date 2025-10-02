namespace OnComics.Application.Models.Response.Favorite
{
    public class FavoriteRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public int ComicId { get; set; }

        public string ComicName { get; set; } = null!;
    }
}
