using Microsoft.AspNetCore.Http;

namespace CA.Common.Logging
{
    /// <summary>
    /// Middleware to create a new correlation id for all incoming requests if not present in the request headers.
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initialize a new instance of <see cref="CorrelationIdMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware to execute in the pipeline.</param>
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invoke the current middleware.
        /// </summary>
        /// <param name="context">The Http context of the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(Constants.CORRELATION_HEADER))
            {
                context.Request.Headers.Add(Constants.CORRELATION_HEADER, Guid.NewGuid().ToString());
            }

            try
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            catch { }
        }
    }
}
