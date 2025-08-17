using Application.Common.Interfaces;
using Application.Weather.DTOs;
using BuildingBlocks.Pagination;
using Domain.Weather.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Weather.Queries;

public record ListWeatherQuery(Guid? LocationId, DateOnly? From, DateOnly? To, string? Search, int Page = 1, int PageSize = 20) : IRequest<PagedResult<WeatherDto>>;

public class ListWeatherQueryHandler : IRequestHandler<ListWeatherQuery, PagedResult<WeatherDto>>
{
	private readonly AppDbContext _db;

	public ListWeatherQueryHandler(AppDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<WeatherDto>> Handle(ListWeatherQuery request, CancellationToken cancellationToken)
	{
		var query = _db.WeatherForecasts.AsNoTracking().AsQueryable();
		if (request.LocationId.HasValue) query = query.Where(x => x.LocationId == request.LocationId);
		if (request.From.HasValue) query = query.Where(x => x.Date >= request.From.Value);
		if (request.To.HasValue) query = query.Where(x => x.Date <= request.To.Value);
		if (!string.IsNullOrWhiteSpace(request.Search)) query = query.Where(x => x.Summary!.Contains(request.Search));

		var total = await query.LongCountAsync(cancellationToken);
		var items = await query
			.OrderByDescending(x => x.Date)
			.Skip((request.Page - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(e => new WeatherDto(e.Id, e.LocationId, e.Date, e.TemperatureC, e.Summary, e.CreatedAt, e.UpdatedAt, e.RowVersion))
			.ToListAsync(cancellationToken);

		return new PagedResult<WeatherDto>
		{
			Items = items,
			Page = request.Page,
			PageSize = request.PageSize,
			TotalItems = total
		};
	}
}