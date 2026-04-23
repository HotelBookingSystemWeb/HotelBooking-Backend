using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace HotelBookingWeb.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.ContentType = "application/json";

                var statusCode = HttpStatusCode.InternalServerError;

                // 🔥 Custom error mapping
                if (ex.Message.Contains("Room not found"))
                    statusCode = HttpStatusCode.NotFound;
                else if (ex.Message.Contains("Invalid Dates"))
                    statusCode = HttpStatusCode.BadRequest;
                else if (ex.Message.Contains("Unauthorized"))
                    statusCode = HttpStatusCode.Unauthorized;

                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    success = false,
                    statusCode = context.Response.StatusCode,
                    message = ex.Message,
                    time = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}