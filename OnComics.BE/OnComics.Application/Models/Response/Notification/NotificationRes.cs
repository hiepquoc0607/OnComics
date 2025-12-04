namespace OnComics.Application.Models.Response.Notification
{
    public class NotificationRes
    {
        public Guid Id { get; set; }

        public Guid ChapterId { get; set; }

        public string Content { get; set; } = null!;

        public DateTime SendTime { get; set; }

        public bool IsRead { get; set; }
    }
}
