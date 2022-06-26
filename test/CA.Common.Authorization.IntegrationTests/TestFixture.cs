using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using CA.Common.Authorization.Client;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CA.Common.Authorization.IntegrationTests
{
    public class TestFixture
    {
        private readonly TestWebApplicationFactory<StartupLocal> _localFactory;
        private readonly TestWebApplicationFactory<StartupRemote> _remoteFactory;
        private HttpClient _localClient;
        private HttpClient _remoteClient;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IDistributedCache> _cacheMock;

        public TestFixture()
        {
            _localFactory = new TestWebApplicationFactory<StartupLocal>();
            _remoteFactory = new TestWebApplicationFactory<StartupRemote>();

            _localClient = _localFactory.CreateClient();

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(LocalClient);

            _cacheMock = new Mock<IDistributedCache>();

            _remoteClient = _remoteFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IPolicyOperations>(provider =>
                        new RemotePolicyOperations(_httpClientFactoryMock.Object,
                            _cacheMock.Object));
                });
            })
            .CreateClient();
        }

        public HttpClient LocalClient => _localClient;
        public HttpClient RemoteClient => _remoteClient;
    }
}
