using Application.Common.Interfaces;
using Application.Weather.DTOs;
using BuildingBlocks.Results;
using Domain.Weather.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Caching;

namespace Application.Weather.Commands;

public record CreateWeatherCommand(CreateWeatherRequest Request) : IRequest<Result<WeatherDto>>;

public class CreateWeatherCommandHandler : IRequestHandler<CreateWeatherCommand, Result<WeatherDto>>
{
	private readonly AppDbContext _db;
	private readonly IUnitOfWork _uow;
	private readonly ICacheService _cache;

	public CreateWeatherCommandHandler(AppDbContext db, IUnitOfWork uow, ICacheService cache)
	{
		_db = db;
		_uow = uow;
		_cache = cache;
	}

	public async Task<Result<WeatherDto>> Handle(CreateWeatherCommand request, CancellationToken cancellationToken)
	{
		var locExists = await _db.Locations.AnyAsync(l => l.Id == request.Request.LocationId, cancellationToken);
		if (!locExists) return Result<WeatherDto>.Failure("Location not found", "NOT_FOUND");

		var entity = new WeatherForecast
		{
			Id = Guid.NewGuid(),
			LocationId = request.Request.LocationId,
			Date = request.Request.Date,
			TemperatureC = request.Request.TemperatureC,
			Summary = request.Request.Summary,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
		await _db.WeatherForecasts.AddAsync(entity, cancellationToken);
		await _uow.SaveChangesAsync(cancellationToken);

		await _cache.RemoveByPatternAsync("weather:", cancellationToken);

		var dto = new WeatherDto(entity.Id, entity.LocationId, entity.Date, entity.TemperatureC, entity.Summary, entity.CreatedAt, entity.UpdatedAt, entity.RowVersion);
		return Result<WeatherDto>.Success(dto);
	}
}