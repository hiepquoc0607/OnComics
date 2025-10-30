namespace OnComics.Application.Models.Response.Appwrite
{
    public class FileRes
    {
        public string Id { get; set; } = string.Empty;

        public string CreatedAt { get; set; } = string.Empty;

        public string UpdatedAt { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Signature { get; set; } = string.Empty;

        public string MimeType { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public long SizeOriginal { get; set; }

        public long ChunksTotal { get; set; }

        public long ChunksUploaded { get; set; }
    }
}
