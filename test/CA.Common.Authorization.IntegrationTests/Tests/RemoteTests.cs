using System.Net;

namespace CA.Common.Authorization.IntegrationTests.Tests
{
    [Collection("TestFixture")]
    public class RemoteTests
    {
        private readonly TestFixture _factory;

        public RemoteTests(TestFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsSuccess_WhenUserHasPermission()
        {
            var client = _factory.RemoteClient;
            var result = await client.GetAsync("api/remote/secure");

            result.EnsureSuccessStatusCode();
            Assert.Equal("Secure Content", result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task Get_ReturnsForbidden_WhenUserHasNoPermission()
        {
            var client = _factory.RemoteClient;
            var result = await client.GetAsync("api/remote/forbidden");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }
    }
}
