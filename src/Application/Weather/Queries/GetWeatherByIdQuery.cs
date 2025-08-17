using Application.Common.Interfaces;
using Application.Weather.DTOs;
using BuildingBlocks.Results;
using Domain.Weather.Entities;
using MediatR;

namespace Application.Weather.Queries;

public record GetWeatherByIdQuery(Guid Id) : IRequest<Result<WeatherDto>>;

public class GetWeatherByIdQueryHandler : IRequestHandler<GetWeatherByIdQuery, Result<WeatherDto>>
{
	private readonly IRepository<WeatherForecast> _repo;

	public GetWeatherByIdQueryHandler(IRepository<WeatherForecast> repo)
	{
		_repo = repo;
	}

	public async Task<Result<WeatherDto>> Handle(GetWeatherByIdQuery request, CancellationToken cancellationToken)
	{
		var entity = await _repo.GetByIdAsync(request.Id, cancellationToken);
		if (entity is null) return Result<WeatherDto>.Failure("NotFound", "NOT_FOUND");
		var dto = new WeatherDto(entity.Id, entity.LocationId, entity.Date, entity.TemperatureC, entity.Summary, entity.CreatedAt, entity.UpdatedAt, entity.RowVersion);
		return Result<WeatherDto>.Success(dto);
	}
}