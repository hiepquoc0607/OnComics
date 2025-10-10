using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OnComics.Application.Models.Response.Common
{
    public class VoidResponse
    {
        public VoidResponse(int statusCode, string message)
        {
            Status = statusCode switch
            {
                >= 200 and < 300 => "Success",
                _ => "Error"
            };
            StatusCode = statusCode;
            Message = message;
        }

        //Exception Rsponse
        public VoidResponse(int statusCode, string errorType, string message)
        {
            Status = "Error";
            StatusCode = statusCode;
            ErrorType = errorType;
            Message = message;
        }

        public string Status { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(200)]
        public int StatusCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorType { get; set; } = null;

        public string Message { get; set; } = string.Empty;
    }
}
