using Microsoft.AspNetCore.Http;

namespace CA.Common.Logging
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string REQUESTIDHEADER = "Request-Id";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(REQUESTIDHEADER))
            {
                context.Request.Headers.Add(REQUESTIDHEADER, Guid.NewGuid().ToString());
            }

            if (!context.Response.Headers.ContainsKey(REQUESTIDHEADER))
            {
                context.Response.Headers.Add(REQUESTIDHEADER, context.Request.Headers[REQUESTIDHEADER]);
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
