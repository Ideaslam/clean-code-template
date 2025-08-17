namespace Application.Weather.DTOs;

public record WeatherDto
(
	Guid Id,
	Guid LocationId,
	DateOnly Date,
	int TemperatureC,
	string? Summary,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	byte[] RowVersion
);

public record CreateWeatherRequest(Guid LocationId, DateOnly Date, int TemperatureC, string? Summary);
public record UpdateWeatherRequest(DateOnly Date, int TemperatureC, string? Summary, byte[] RowVersion);