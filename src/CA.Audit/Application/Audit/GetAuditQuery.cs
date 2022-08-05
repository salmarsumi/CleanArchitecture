using CA.MediatR;
using MediatR;

namespace CA.Audit.Application.Audit
{
    /// <summary>
    /// Query
    /// </summary>
    public class GetAuditQuery : IRequest<RequestResult<AuditDetailsDto>>
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Handler
    /// </summary>
    public class GetAuditQueryHandler : IRequestHandler<GetAuditQuery, RequestResult<AuditDetailsDto>>
    {
        private readonly IAuditQueries _query;

        public GetAuditQueryHandler(IAuditQueries query)
        {
            _query = query;
        }

        public async Task<RequestResult<AuditDetailsDto>> Handle(GetAuditQuery request, CancellationToken cancellationToken)
        {
            AuditDetailsDto result = await _query.GetAuditAsync(request.Id, cancellationToken);

            if(result is not null)
            {
                return RequestResult<AuditDetailsDto>.Succeeded(result);
            }

            return RequestResult<AuditDetailsDto>.NotFound(request.Id);
        }
    }
}
