namespace OnComics.Application.Models.Response.History
{
    public class HistoryRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string Fullname { get; set; } = null!;

        public int ComicId { get; set; }

        public string ComicName { get; set; } = null!;

        public int ChapterId { get; set; }

        public DateTime ReadTime { get; set; }
    }
}
