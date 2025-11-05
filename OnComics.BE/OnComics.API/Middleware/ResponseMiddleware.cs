using System.Text.Json;

namespace OnComics.API.Middleware
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Check If The Response Status Is 403
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Error",
                    statusCode = 403,
                    message = "Authentication Required!"
                }));
            }

            // Check If The Response Status Is 401
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "Error",
                    statusCode = 401,
                    message = "Access Denied!"
                }));
            }
        }
    }
}
