namespace Domain.Weather.Entities;

public class WeatherForecast
{
  public Guid Id { get; set; }

  public Guid LocationId { get; set; }

  public required DateOnly Date { get; set; }

  public int TemperatureC { get; set; }

  public string? Summary { get; set; }

  public DateTime CreatedAt { get; set; }

  public DateTime UpdatedAt { get; set; }

  public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}