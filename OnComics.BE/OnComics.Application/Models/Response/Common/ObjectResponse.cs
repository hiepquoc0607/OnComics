using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Common
{
    public class ObjectResponse<T>
    {
        public ObjectResponse(string status, int statusCode, string message, T? data, Pagination? pagination = null)
        {
            Status = status;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Pagination = pagination;
        }

        public ObjectResponse(string status, int statusCode, string message)
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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pagination? Pagination { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }
}
