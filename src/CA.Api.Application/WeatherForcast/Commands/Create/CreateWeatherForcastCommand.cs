using CA.Api.Application.Interfaces;
using CA.MediatR;
using MediatR;

namespace CA.Api.Application.WeatherForcast.Commands.Create
{
    public class CreateWeatherForcastCommand : IRequest<RequestResult<int>>, ITransactionalRequest
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
    }

    public class CreateWeatherForcastCommandHandler : IRequestHandler<CreateWeatherForcastCommand, RequestResult<int>>
    {
        private readonly IApiDbContext _context;

        public CreateWeatherForcastCommandHandler(IApiDbContext context)
        {
            _context = context;
        }

        public async Task<RequestResult<int>> Handle(CreateWeatherForcastCommand request, CancellationToken cancellationToken)
        {
            var entity = new CA.Api.Domain.Entities.WeatherForcast
            {
                Date = request.Date,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary
            };

            _context.WeatherForcasts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return RequestResult<int>.Succeeded(entity.Id);
        }
    }
}
