using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CA.Common.HttpMessageHandlers
{
    /// <summary>
    /// <see cref="IHttpMessageHandlerBuilderFilter"/> implementation that will be used by the HttpClient factory to register the custom request logger.
    /// </summary>
    public class CAHttpMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly ILogger<HttpClientRequestLoggerDelegatingHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of <see cref="CAHttpMessageHandlerBuilderFilter"/>.
        /// </summary>
        /// <param name="logger">The logger instace required by <see cref="HttpClientRequestLoggerDelegatingHandler"/>.</param>
        /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> instance required by <see cref="HttpClientRequestLoggerDelegatingHandler"/>.</param>
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
