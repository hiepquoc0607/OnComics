namespace OnComics.Library.Utils.Helpers
{
    public class MailHelper
    {
        public string SmtpServer { get; set; } = null!;

        public int SmtpPort { get; set; }

        public string SenderEmail { get; set; } = null!;

        public string SenderPassword { get; set; } = null!;
    }
}
