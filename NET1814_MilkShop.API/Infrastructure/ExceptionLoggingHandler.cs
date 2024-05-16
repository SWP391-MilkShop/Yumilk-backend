using Microsoft.AspNetCore.Diagnostics;

namespace SWP391_DEMO.Infrastructure
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
            _logger.LogError("Message with TraceId : {TraceId} failed with message: {exceptionMessage}", httpContext.TraceIdentifier, exceptionMessage);
            return ValueTask.FromResult(false);
        }
    }
}
