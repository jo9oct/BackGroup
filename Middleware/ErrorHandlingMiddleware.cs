using System.Net;
using System.Text.Json;

namespace LibraryWebAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }
 private static Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                context.Response.ContentType = "application/json";
                // Default to 500 Internal Server Error if not specified
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // You can customize the status code based on exception type
                // if (exception is MyCustomNotFoundException)
                // {
                //     context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                // }

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "An internal server error occurred. Please try again later.",
                    // detailed = exception.Message // Only include in development
                };

                // For development, you might want to include more details
                // if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                // {
                //     response = new { statusCode = context.Response.StatusCode, message = exception.Message, stackTrace = exception.StackTrace };
                // }


                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
  
