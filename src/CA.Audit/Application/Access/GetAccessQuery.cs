using CA.Audit.Infrastructure;
using CA.MediatR;
using MediatR;

namespace CA.Audit.Application.Access
{
    /// <summary>
    /// Query
    /// </summary>
    public class GetAccessQuery : IRequest<RequestResult<AccessDetailsDto>>
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Handler
    /// </summary>
    public class GetAccessQueryHandler : IRequestHandler<GetAccessQuery, RequestResult<AccessDetailsDto>>
    {
        private readonly IAccessQueries _query;

        public GetAccessQueryHandler(IAccessQueries query)
        {
            _query = query;
        }

        public async Task<RequestResult<AccessDetailsDto>> Handle(GetAccessQuery request, CancellationToken cancellationToken)
        {
            AccessDetailsDto result = await _query.GetAccessAsync(request.Id, cancellationToken);

            if (result is not null)
            {
                return RequestResult<AccessDetailsDto>.Succeeded(result);
            }

            return RequestResult<AccessDetailsDto>.NotFound(request.Id);
        }
    }
}
