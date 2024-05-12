using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace SWP391_DEMO.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            //prevent showing the actual exception message to the client
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path,
                Status = httpContext.Response.StatusCode,
                Title = "An error occurred while processing your request",
                Detail = GetHelpLink(exception),
                Type = "Error"
            };
            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken: cancellationToken
            );
            return true;
        }
        /// <summary>
        /// Get help link based on the exception type
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static string GetHelpLink(Exception exception)
        {
            //common exception types and their help links
            return exception switch
            {
                ArgumentException => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                TimeoutException => "https://tools.ietf.org/html/rfc7231#section-6.6.5",
                InvalidOperationException => "https://tools.ietf.org/html/rfc7231#section-6.5.10",
                KeyNotFoundException => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6"
            };
        }
    }
}
