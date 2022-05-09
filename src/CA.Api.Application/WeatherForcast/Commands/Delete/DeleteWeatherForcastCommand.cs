using CA.Api.Application.Interfaces;
using Entities = CA.Api.Domain.Entities;
using CA.MediatR;
using MediatR;
using CA.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using CA.Common.Permissions;

namespace CA.Api.Application.WeatherForcast.Commands.Delete
{
    public class DeleteWeatherForcastCommand : IRequest, ITransactionalRequest, IAuthorizedRequest
    {
        public int Id { get; set; }

        public IEnumerable<string> GetRequiredPermissions()
        {
            return new[] { AppPermissions.DeleteWeather };
        }
    }

    public class DeleteWeatherForcastCommandHandler : IRequestHandler<DeleteWeatherForcastCommand>
    {
        private readonly IApiDbContext _context;

        public DeleteWeatherForcastCommandHandler(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteWeatherForcastCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WeatherForcasts
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .SingleOrDefaultAsync(cancellationToken);

            if(entity is null)
            {
                throw new NotFoundException(nameof(Entities.WeatherForcast), request.Id);
            }

            _context.WeatherForcasts.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
