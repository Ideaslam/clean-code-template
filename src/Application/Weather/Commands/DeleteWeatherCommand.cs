using Application.Common.Interfaces;
using BuildingBlocks.Results;
using Domain.Weather.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Caching;

namespace Application.Weather.Commands;

public record DeleteWeatherCommand(Guid Id) : IRequest<Result>;

public class DeleteWeatherCommandHandler : IRequestHandler<DeleteWeatherCommand, Result>
{
	private readonly AppDbContext _db;
	private readonly IUnitOfWork _uow;
	private readonly ICacheService _cache;

	public DeleteWeatherCommandHandler(AppDbContext db, IUnitOfWork uow, ICacheService cache)
	{
		_db = db;
		_uow = uow;
		_cache = cache;
	}

	public async Task<Result> Handle(DeleteWeatherCommand request, CancellationToken cancellationToken)
	{
		var entity = await _db.WeatherForecasts.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
		if (entity is null) return Result.Failure("Not found", "NOT_FOUND");
		_db.WeatherForecasts.Remove(entity);
		await _uow.SaveChangesAsync(cancellationToken);
		await _cache.RemoveByPatternAsync("weather:", cancellationToken);
		return Result.Success();
	}
}