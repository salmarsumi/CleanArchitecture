using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.UnitTests
{
    public class RoleTests : TestBase
    {
        [Fact]
        public void Evaluate_ThrowsException_WhenUserIsNull()
        {
            // Arrange
            var group = new Group();

            // Assert
            Assert.Throws<ArgumentNullException>(() => group.Evaluate(null));
        }

        [Fact]
        public void Evaluate_ReturnsTrue_WhenUserInGroup()
        {
            // Arrange
            var sub = Guid.NewGuid().ToString();
            var claim = new Claim("sub", sub);

            var group = new Group();
            group.Users = new string[] { sub };

            var user = SetupUser(claim);

            // Act
            var result = group.Evaluate(user);

            // Asser
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ReturnsFalse_WhenUserNotInGroup()
        {
            // Arrange
            var sub = Guid.NewGuid().ToString();
            var claim = new Claim("sub", sub);

            var group = new Group();
            group.Users = new string[] { Guid.NewGuid().ToString() };

            var user = SetupUser(claim);

            // Act
            var result = group.Evaluate(user);

            // Asser
            Assert.False(result);
        }
    }
}
