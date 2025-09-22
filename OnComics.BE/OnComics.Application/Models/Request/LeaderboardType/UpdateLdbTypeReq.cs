using System.ComponentModel.DataAnnotations;

namespace OnComics.Application.Models.Request.LeaderboardType
{
    public class UpdateLdbTypeReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
