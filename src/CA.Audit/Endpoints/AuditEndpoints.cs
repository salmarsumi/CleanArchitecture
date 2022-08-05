using CA.Audit.Application.Audit;
using CA.Common.Authorization;
using CA.MediatR;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace CA.Audit.Endpoints
{
    public static class AuditEndpoints
    {
        public static WebApplication MapAuditEndpoints(this WebApplication app)
        {
            // GET /audit
            app.MapGet("/audit", [Authorize(nameof(AppPermissions.ViewAudit))] async (ISender mediator) =>
            {
                RequestResult<IEnumerable<AuditDto>> result = await mediator.Send(new GetAllAuditQuery());

                if (result.Success)
                {
                    return Results.Ok(result.Result);
                }

                return result.AsApiResult();
            });

            // GET /audit/{id}
            app.MapGet("/audit/{id}", [Authorize(nameof(AppPermissions.ViewAudit))] async (ISender mediator, int id) =>
            {
                RequestResult<AuditDetailsDto> result = await mediator.Send(new GetAuditQuery
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
