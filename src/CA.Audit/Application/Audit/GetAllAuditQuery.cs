using CA.MediatR;
using MediatR;

namespace CA.Audit.Application.Audit
{
    /// <summary>
    /// Query
    /// </summary>
    public class GetAllAuditQuery : IRequest<RequestResult<IEnumerable<AuditDto>>>
    {
    }

    /// <summary>
    /// Handler
    /// </summary>
    public class GetAllAuditQueryHandler : IRequestHandler<GetAllAuditQuery, RequestResult<IEnumerable<AuditDto>>>
    {
        private readonly IAuditQueries _query;

        public GetAllAuditQueryHandler(IAuditQueries query)
        {
            _query = query;
        }

        public async Task<RequestResult<IEnumerable<AuditDto>>> Handle(GetAllAuditQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<AuditDto> result = await _query.GetAllAuditAsync(cancellationToken);
            return RequestResult<IEnumerable<AuditDto>>.Succeeded(result);
        }
    }
}
