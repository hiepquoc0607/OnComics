namespace OnComics.Application.Models.Response.History
{
    public class HistoryRes
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public Guid ComicId { get; set; }

        public string ComicName { get; set; } = null!;

        public Guid ChapterId { get; set; }

        public DateTime ReadTime { get; set; }
    }
}
