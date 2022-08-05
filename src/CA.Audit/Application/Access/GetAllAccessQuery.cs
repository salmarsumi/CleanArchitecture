using CA.MediatR;
using MediatR;

namespace CA.Audit.Application.Access
{
    /// <summary>
    /// Query
    /// </summary>
    public class GetAllAccessQuery : IRequest<RequestResult<IEnumerable<AccessDto>>>
    {
    }

    /// <summary>
    /// Handler
    /// </summary>
    public class GetAllAcccessQueryHandler : IRequestHandler<GetAllAccessQuery, RequestResult<IEnumerable<AccessDto>>>
    {
        private readonly IAccessQueries _query;

        public GetAllAcccessQueryHandler(IAccessQueries query)
        {
            _query = query;
        }

        public async Task<RequestResult<IEnumerable<AccessDto>>> Handle(GetAllAccessQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<AccessDto> result = await _query.GetAllAccessAsync(cancellationToken);
            return RequestResult<IEnumerable<AccessDto>>.Succeeded(result);
        }
    }
}
