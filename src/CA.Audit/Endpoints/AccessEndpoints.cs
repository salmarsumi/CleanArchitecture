using CA.Audit.Application.Access;
using CA.Common.Authorization;
using CA.MediatR;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CA.Audit.Endpoints
{
    public static class AccessEndpoints
    {
        public static WebApplication MapAccessEndpoints(this WebApplication app)
        {
            // GET /access
            app.MapGet("/access", [Authorize(nameof(AppPermissions.ViewAccess))] async (ISender mediator) =>
            {
                RequestResult<IEnumerable<AccessDto>> result = await mediator.Send(new GetAllAccessQuery());

                if (result.Success)
                {
                    return Results.Ok(result.Result);
                }

                return result.AsApiResult();
            });

            // GET /audit/{id}
            app.MapGet("/access/{id}", [Authorize(nameof(AppPermissions.ViewAccess))] async (ISender mediator, int id) =>
            {
                RequestResult<AccessDetailsDto> result = await mediator.Send(new GetAccessQuery
                {
                    Id = id
                });

                if (result.Success)
                {
                    return Results.Ok(result.Result);
                }

                return result.AsApiResult();
            });

            return app;
        }
    }
}
