using CA.Common.Authorization.PolicyRuntime;
using System.Security.Claims;

namespace CA.Common.Authorization.UnitTests
{
    public class PolicyTests : TestBase
    {
        [Fact]
        public void Evaluate_ThrowsException_WhenUserIsNull()
        {
            // Arrange
            var policy = new Policy(new Group[] { }, new Permission[] { });

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => policy.EvaluateAsync(null));
        }

        [Fact]
        public async Task Evaluate_ReturnsGroupsAndPermissions_WhenUserHasPolicies()
        {
            // Arrange
            var sub = Guid.NewGuid().ToString();
            var claim = new Claim("sub", sub);

            var roles = new Group[]
            {
                new Group
                {
                    Name = "Test Role1",
                    Users = new string[] { sub }
                },
                new Group
                {
                    Name = "Test Role2",
                    Users = new string[] { sub }
                }
            };

            var permissions = new Permission[]
            {
                new Permission
                {
                    Name = "Test Permission1",
                    Groups = new string[] { "Test Role1" }
                },
                new Permission
                {
                    Name = "Test Permission2",
                    Groups = new string[] { "Test Role2" }
                }
            };

            var policy = new Policy(roles, permissions);

            var user = SetupUser(claim);

            // Act
            var result = await policy.EvaluateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Groups.Count());
            Assert.Equal(2, result.Permissions.Count());
        }

        [Fact]
        public async Task Evaluate_ReturnsEmptyGroupsAndPermissions_WhenUserHasNoPolicies()
        {
            // Arrange
            var sub = Guid.NewGuid().ToString();
            var claim = new Claim("sub", sub);

            var roles = new Group[]
            {
                new Group
                {
                    Name = "Test Role1",
                    Users = new string[] { Guid.NewGuid().ToString() }
                },
                new Group
                {
                    Name = "Test Role2",
                    Users = new string[] { Guid.NewGuid().ToString() }
                }
            };

            var permissions = new Permission[]
            {
                new Permission
                {
                    Name = "Test Permission1",
                    Groups = new string[] { "Test Role1" }
                },
                new Permission
                {
                    Name = "Test Permission2",
                    Groups = new string[] { "Test Role2" }
                }
            };

            var policy = new Policy(roles, permissions);

            var user = SetupUser(claim);

            // Act
            var result = await policy.EvaluateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Groups);
            Assert.Empty(result.Permissions);
        }
    }
}
