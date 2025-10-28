using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Category
{
    public class CategoryRes
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }

        public string Status { get; set; } = null!;
    }
}