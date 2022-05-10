using Microsoft.AspNetCore.Http;

namespace CA.Common.Logging
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(Constants.CORRELATION_HEADER))
            {
                context.Request.Headers.Add(Constants.CORRELATION_HEADER, Guid.NewGuid().ToString());
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
