using CA.Common.HttpMessageHandlers;
using Microsoft.Extensions.Http;

namespace CA.WebAngular
{
    internal class HttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILogger<HttpClientRequestLoggerDelegatingHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpMessageHandlerBuilderFilter(ILogger<HttpClientRequestLoggerDelegatingHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            return (builder) =>
            {
                // Run other configuration first, we want to decorate.
                next(builder);
                builder.AdditionalHandlers.Add(new HttpClientRequestLoggerDelegatingHandler(_httpContextAccessor, _logger));
            };
        }
    }
}