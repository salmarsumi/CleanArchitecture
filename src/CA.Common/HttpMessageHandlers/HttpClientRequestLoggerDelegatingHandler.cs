using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CA.Common.HttpMessageHandlers
{
    public interface IHttpClientRequestLoggerExecludedPaths
    {
        IEnumerable<string> GetExeclidedPaths();
    }

    public class HttpClientRequestLoggerDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HttpClientRequestLoggerDelegatingHandler> _logger;
        private readonly IHttpClientRequestLoggerExecludedPaths _execludedPaths;

        public HttpClientRequestLoggerDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<HttpClientRequestLoggerDelegatingHandler> logger,
            IHttpClientRequestLoggerExecludedPaths execludedPaths = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _execludedPaths = execludedPaths;
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
            if (request.Headers.Contains("Request-Id"))
            {
                correlationId = request.Headers.First(h => h.Key == "Request-Id").Value.First();
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
            }

            string userId = "UnAuthenticated";
            string username = "UnAuthenticated";
            var httpContext = _httpContextAccessor.HttpContext;
            
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

            using (_logger.BeginScope("{CorrelationId} {UserId} {Username}", correlationId, userId, username))
            {
                if (caught == default)
                {
                    var execludedPaths = _execludedPaths?.GetExeclidedPaths();
                    if(execludedPaths is null)
                    {
                        execludedPaths = new[] { "/live", "/ready" };
                    }

                    LogLevel logLevel;
                    if(httpContext is not null && execludedPaths.Any(x => httpContext.Request.Path.Value.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        logLevel = LogLevel.Trace;
                    }
                    else
                    {
                        logLevel = LogLevel.Information;
                    }

                    _logger.Log(
                        logLevel,
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
