namespace OnComics.Application.Models.Response.InteractionType
{
    public class InteractionTypeRes
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ImgUrl { get; set; }

        public string Status { get; set; } = null!;
    }
}
