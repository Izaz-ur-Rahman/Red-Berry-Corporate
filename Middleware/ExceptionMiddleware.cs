using System.Text.Json;
using RedBerryCorporate.Helpers;

namespace RedBerryCorporate.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                context.Response.ContentType = "application/json";

                var response = new ApiErrorResponse
                {
                    //Message = "Internal Server Error.",
                    Message = ex.ToString(),
                    TraceId = context.TraceIdentifier
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}