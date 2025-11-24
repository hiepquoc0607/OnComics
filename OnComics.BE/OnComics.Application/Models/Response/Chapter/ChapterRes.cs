using OnComics.Application.Models.Response.ChapterSource;
using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Chapter
{
    public class ChapterRes
    {
        public Guid Id { get; set; }

        public int ChapNo { get; set; }

        public string? Name { get; set; } = string.Empty;

        public int ReadNum { get; set; }

        public DateOnly ReleaseTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ChapterSourceRes>? Chaptersources { get; set; } = null;
    }
}