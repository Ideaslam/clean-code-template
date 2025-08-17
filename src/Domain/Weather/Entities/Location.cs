namespace Domain.Weather.Entities;

public class Location
{
  public Guid Id { get; set; }

  public required string Name { get; set; }

  public required string Country { get; set; }

  public double Lat { get; set; }

  public double Lng { get; set; }
}