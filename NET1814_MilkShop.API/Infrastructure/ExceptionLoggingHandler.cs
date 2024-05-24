using Microsoft.AspNetCore.Diagnostics;

namespace NET1814_MilkShop.API.Infrastructure
{
    public class ExceptionLoggingHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionLoggingHandler> _logger;

        public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            var exceptionMessage = exception.Message;
            _logger.LogError(
                "Message with TraceId : {TraceId} failed with message: {exceptionMessage}\nInnerException: {InnerException}",
                httpContext.TraceIdentifier,
                exceptionMessage,
                exception.InnerException
            );
            return ValueTask.FromResult(false);
        }
    }
}
