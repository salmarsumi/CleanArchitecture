using CA.Api.Application.Interfaces;
using CA.Api.Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Api.IntegrationTets
{
    public class RequestHandlerTestBase<T>
        where T : class
    {
        protected readonly T _handler;

        public RequestHandlerTestBase()
        {
            IApiDbContext context = CreateApiContext();
            _handler = Activator.CreateInstance(typeof(T), context) as T;
        }

        protected ApiDbContext CreateApiContext()
        {
            return new ApiDbContextFactory().CreateDbContext(Array.Empty<string>());
        }
    }
}
