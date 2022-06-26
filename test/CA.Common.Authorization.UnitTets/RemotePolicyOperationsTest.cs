using CA.Common.Authorization.Client;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Security.Claims;

namespace CA.Common.Authorization.UnitTests
{
    public class RemotePolicyOperationsTest : TestBase
    {
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;

        public RemotePolicyOperationsTest()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _httpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task EvaluateAsync_ThrowsException_WhenUserIsNull_()
        {
            // Arrange
            _httpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var remotePolicyOperations = new RemotePolicyOperations(_httpClientFactory.Object, _cacheMock.Object);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => remotePolicyOperations.EvaluateAsync(null));
            Assert.Equal("user", exception.ParamName);
        }

        [Fact]
        public async void EvaluateAsync_ThrowsException_WhenCreateHttpClientReturnsNull()
        {
            // Arrange
            var sub = Guid.NewGuid().ToString();
            Claim claim = new ("sub", sub);
            RemotePolicyOperations remotePolicyOperations = new(_httpClientFactory.Object, _cacheMock.Object);
            ClaimsPrincipal user = SetupUser(claim);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => remotePolicyOperations.EvaluateAsync(user));
        }
    }
}
