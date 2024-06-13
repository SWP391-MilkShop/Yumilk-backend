using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NET1814_MilkShop.API.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            httpContext.Response.StatusCode = GetStatusCode(exception);
            //prevent showing the actual exception message to the client
            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path,
                Status = httpContext.Response.StatusCode,
                Title = "An error occurred while processing your request",
                Detail = GetHelpLink(exception) + $"\n Exception: {exception.Message}\nInnerException: {exception.InnerException}",
                Type = "Error" //cai nay nen de documentation link
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
                UnauthorizedAccessException => "https://tools.ietf.org/html/rfc7235#section-3.1",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6"
            };
        }

        /// <summary>
        /// Get status code based on the exception type
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                TimeoutException => StatusCodes.Status504GatewayTimeout,
                InvalidOperationException => StatusCodes.Status500InternalServerError,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
