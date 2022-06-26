using CA.Common.Authorization.PolicyRuntime;

namespace SMD.Security.Authorization.UnitTests
{
    public class PermissionTests
    {
        [Fact]
        public void Evaluate_ThrowsException_WhenUserIsNull()
        {
            // Arrange
            var permission = new Permission();

            // Assert
            Assert.Throws<ArgumentNullException>(() => permission.Evaluate(null));
        }

        [Fact]
        public void Evaluate_ReturnsTrue_WhenGroupInPermission()
        {
            // Arrange
            var group = Guid.NewGuid().ToString();

            var permission = new Permission();
            permission.Groups = new string[] { group };

            // Act
            var result = permission.Evaluate(new string[] { group });

            // Asser
            Assert.True(result);
        }

        [Fact]
        public void Evaluate_ReturnsFalse_WhenGroupNotInPermission()
        {
            // Arrange
            var group = Guid.NewGuid().ToString();

            var permission = new Permission();
            permission.Groups = new string[] { group };

            // Act
            var result = permission.Evaluate(new string[] { Guid.NewGuid().ToString() });

            // Asser
            Assert.False(result);
        }
    }
}
