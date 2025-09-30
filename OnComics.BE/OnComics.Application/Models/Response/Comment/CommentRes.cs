using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Comment
{
    public class CommentRes
    {
        public int Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AccountId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fullname { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ComicId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ComicName { get; set; } = null;

        public string Content { get; set; } = null!;

        public bool IsMainCmt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MainCmtId { get; set; } = null;

        public DateTime CmtTime { get; set; }

        public int InteractionNum { get; set; }
    }
}
