using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Chapter
{
    public class ChapterRes
    {
        public int Id { get; set; }

        public int ChapNo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Name { get; set; } = string.Empty;

        public int ReadNum { get; set; }

        public DateTime ReleaseTime { get; set; }
    }
}