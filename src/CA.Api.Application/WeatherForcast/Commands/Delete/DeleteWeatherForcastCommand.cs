﻿using Entities = CA.Api.Domain.Entities;
using CA.MediatR;
using MediatR;
using CA.Common.ResponseTypes;
using CA.Api.Domain.Interfaces;
using CA.Api.Domain.Events;

namespace CA.Api.Application.WeatherForcast.Commands.Delete
{
    public class DeleteWeatherForcastCommand : IRequest<RequestResult<Unit>>, ITransactionalRequest
    {
        public int Id { get; set; }
    }

    public class DeleteWeatherForcastCommandHandler : IRequestHandler<DeleteWeatherForcastCommand, RequestResult<Unit>>
    {
        private readonly IWeatherForecastRepository _repository;

        public DeleteWeatherForcastCommandHandler(IWeatherForecastRepository repository)
        {
            _repository = repository;
        }

        public async Task<RequestResult<Unit>> Handle(DeleteWeatherForcastCommand request, CancellationToken cancellationToken)
        {
            Entities.WeatherForecast entity = await _repository.GetAsync(request.Id, cancellationToken);

            if (entity is null)
            {
                return RequestResult<Unit>.NotFound(new JsonErrorResponse
                {
                    ExceptionType = "Not Found",
                    Key = request.Id,
                    Name = nameof(Entities.WeatherForecast)
                });
            }

            entity.AddDomainEvent(new WeatherForecastDeletedEvent(entity));

            _repository.Remove(entity);

            await _repository.SaveChangesAsync(cancellationToken);

            return RequestResult<Unit>.Succeeded(Unit.Value);
        }
    }
}
