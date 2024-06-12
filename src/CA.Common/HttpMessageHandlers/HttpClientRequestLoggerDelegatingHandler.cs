using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CA.Common.HttpMessageHandlers
{
    /// <summary>
    /// Replaces the default chatty HttpClient request logger.
    /// </summary>
    public class HttpClientRequestLoggerDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpClientRequestLoggerDelegatingHandler> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="HttpClientRequestLoggerDelegatingHandler"/>.
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor instance.</param>
        /// <param name="logger">Logger instance.</param>
        public HttpClientRequestLoggerDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<HttpClientRequestLoggerDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch watch = default;
            HttpResponseMessage response = default;
            Exception caught = default;
            try
            {
                watch = Stopwatch.StartNew();
                response = await base.SendAsync(request, cancellationToken);
                watch.Stop();
            }
            catch (Exception ex)
            {
                caught = ex;
            }

            var correlationId = string.Empty;
            if (request.Headers.Contains(Constants.CORRELATION_HEADER))
            {
                correlationId = request.Headers.GetValues(Constants.CORRELATION_HEADER).First();
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
            }

            string userId = "UnAuthenticated";
            string username = "UnAuthenticated";
            var httpContext = _httpContextAccessor.HttpContext;
            
            // Extract user infromation from the Http context.
            if(httpContext is not null)
            {
                var isAuthenticated = httpContext.User.Identity.IsAuthenticated;
                if (isAuthenticated)
                {
                    userId = "User id not found";
                    username = "Username not found";
                }
                
                if (httpContext.User.HasClaim(x => x.Type == "sub"))
                {
                    userId = httpContext.User.FindFirst("sub").Value;
                }

                if (httpContext.User.HasClaim(x => x.Type == "email"))
                {
                    username = httpContext.User.FindFirst("email").Value;
                }
            }

            // Start a logger scope to add common properties to the log entries.
            using (_logger.BeginScope("{CorrelationId} {UserId} {Username}", correlationId, userId, username))
            {
                if (caught == default)
                {
                    _logger.LogInformation(
                        "HTTPCLIENT request {HttpMethod} {Uri} responded in {elapsed} ms - {StatusCode}",
                        request.Method,
                        request.RequestUri,
                        watch.Elapsed.TotalMilliseconds,
                        ((int)response.StatusCode));
                }
                else
                {
                    _logger.LogError(caught, caught.Message);
                    throw caught;
                }
            }

            return response;
        }
    }
}
