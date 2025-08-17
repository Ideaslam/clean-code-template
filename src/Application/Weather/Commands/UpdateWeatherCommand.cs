using Application.Common.Interfaces;
using Application.Weather.DTOs;
using BuildingBlocks.Results;
using Domain.Weather.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Caching;

namespace Application.Weather.Commands;

public record UpdateWeatherCommand(Guid Id, UpdateWeatherRequest Request) : IRequest<Result<WeatherDto>>;

public class UpdateWeatherCommandHandler : IRequestHandler<UpdateWeatherCommand, Result<WeatherDto>>
{
	private readonly AppDbContext _db;
	private readonly IUnitOfWork _uow;
	private readonly ICacheService _cache;

	public UpdateWeatherCommandHandler(AppDbContext db, IUnitOfWork uow, ICacheService cache)
	{
		_db = db;
		_uow = uow;
		_cache = cache;
	}

	public async Task<Result<WeatherDto>> Handle(UpdateWeatherCommand request, CancellationToken cancellationToken)
	{
		var entity = await _db.WeatherForecasts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (entity is null) return Result<WeatherDto>.Failure("Not found", "NOT_FOUND");
		if (!entity.RowVersion.SequenceEqual(request.Request.RowVersion))
			return Result<WeatherDto>.Failure("Concurrency conflict", "CONFLICT");

		entity.Date = request.Request.Date;
		entity.TemperatureC = request.Request.TemperatureC;
		entity.Summary = request.Request.Summary;
		entity.UpdatedAt = DateTime.UtcNow;

		await _uow.SaveChangesAsync(cancellationToken);
		await _cache.RemoveByPatternAsync("weather:", cancellationToken);

		var dto = new WeatherDto(entity.Id, entity.LocationId, entity.Date, entity.TemperatureC, entity.Summary, entity.CreatedAt, entity.UpdatedAt, entity.RowVersion);
		return Result<WeatherDto>.Success(dto);
	}
}