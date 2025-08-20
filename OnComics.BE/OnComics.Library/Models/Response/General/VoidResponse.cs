using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnComics.Library.Models.Response.General
{
    public class VoidResponse
    {
        public VoidResponse(string status, int statusCode, string message)
        {
            Status = status;
            StatusCode = statusCode;
            Message = message;
        }

        public string Status { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(200)]
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
