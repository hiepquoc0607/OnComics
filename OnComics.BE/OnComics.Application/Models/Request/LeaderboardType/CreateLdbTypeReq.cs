namespace OnComics.Application.Models.Request.LeaderboardType
{
    public class CreateLdbTypeReq
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
