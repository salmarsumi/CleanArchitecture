using CA.Common.Contracts.Audit;
using CA.Common.Services;
using MassTransit;

namespace CA.Identity.Services
{
    public interface IAccessEventPublisher
    {
        Task PublisheAccessEvent(string action, string result, string details, string username = null);
    }

    public class AccessEventPublisher : IAccessEventPublisher
    {
        private readonly ICurrentRequestService _currentRequest;
        private readonly IPublishEndpoint _publishEndpoint;

        public AccessEventPublisher(ICurrentRequestService currentRequest, IPublishEndpoint publishEndpoint = null)
        {
            _currentRequest = currentRequest;
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublisheAccessEvent(string action, string result, string details, string username = null)
        {
            if(_publishEndpoint is not null)
            {
                await _publishEndpoint.Publish<AccessEntry>(new
                {
                    CorrelationId = _currentRequest.GetCorrelationId(),
                    UserId = _currentRequest.GetUserId(),
                    Username = username ?? _currentRequest.GetUsername(),
                    Action = action,
                    Result = result,
                    Details = details,
                    Browser = _currentRequest.GetClientBrowser(),
                    IPAddress = _currentRequest.GetClientIPAddress(),
                    TimeStamp = DateTime.UtcNow
                });
            }
        }
    }
}
