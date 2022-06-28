using CA.Api.Infrastructure.Data;
using CA.Common.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace CA.Api.IntegrationTets
{
    internal class ApiDbContextFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {
        public ApiDbContext CreateDbContext(string[] args)
        {
            var domainEventServiceMock = new Mock<IDomainEventService>();
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();

            optionsBuilder
                .UseInMemoryDatabase("ApiDb")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            return new ApiDbContext(optionsBuilder.Options, new FakeCurrentUserService(), domainEventServiceMock.Object);
        }
    }
}
