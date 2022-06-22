using CA.Api.Application.Interfaces;
using Entities = CA.Api.Domain.Entities;
using CA.MediatR;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CA.Common.ResponseTypes;

namespace CA.Api.Application.WeatherForcast.Commands.Delete
{
    public class DeleteWeatherForcastCommand : IRequest<RequestResult<Unit>>, ITransactionalRequest
    {
        public int Id { get; set; }
    }

    public class DeleteWeatherForcastCommandHandler : IRequestHandler<DeleteWeatherForcastCommand, RequestResult<Unit>>
    {
        private readonly IApiDbContext _context;

        public DeleteWeatherForcastCommandHandler(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<RequestResult<Unit>> Handle(DeleteWeatherForcastCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if(entity is null)
            {
                return RequestResult<Unit>.NotFound(new JsonErrorResponse
                {
                    ExceptionType = "Not Found",
                    Key = request.Id,
                    Name = nameof(Entities.WeatherForcast)
                });
            }

            _context.WeatherForcasts.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return RequestResult<Unit>.Succeeded(Unit.Value);
        }
    }
}
