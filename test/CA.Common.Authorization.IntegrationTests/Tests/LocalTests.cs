using System.Net;

namespace CA.Common.Authorization.IntegrationTests.Tests
{
    [Collection("TestFixture")]
    public class LocalTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _factory;

        public LocalTests(TestFixture factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsSuccess_WhenUserHasPermission()
        {
            var client = _factory.LocalClient;
            var result = await client.GetAsync("api/local/secure");

            result.EnsureSuccessStatusCode();
            Assert.Equal("Secure Content", result.Content.ReadAsStringAsync().Result);
        }

        [Fact]
        public async Task Get_ReturnsForbidden_WhenUserHasNoPermission()
        {
            var client = _factory.LocalClient;
            var result = await client.GetAsync("/api/local/forbidden");

            Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        }
    }
}
