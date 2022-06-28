using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CA.Common.HttpMessageHandlers
{
    public class CAHttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILogger<HttpClientRequestLoggerDelegatingHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CAHttpMessageHandlerBuilderFilter(ILogger<HttpClientRequestLoggerDelegatingHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            if (next is null)
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
