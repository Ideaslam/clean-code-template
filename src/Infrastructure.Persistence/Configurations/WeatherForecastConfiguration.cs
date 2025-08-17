using Domain.Weather.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
	public void Configure(EntityTypeBuilder<WeatherForecast> builder)
	{
		builder.ToTable("WeatherForecasts");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.RowVersion).IsRowVersion();
		builder.Property(x => x.Summary).HasMaxLength(400);
		builder.Property(x => x.Date).HasConversion(
			v => v.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
			v => DateOnly.FromDateTime(DateTime.SpecifyKind(v, DateTimeKind.Utc))
		);
		builder.HasOne<Location>()
			.WithMany()
			.HasForeignKey(x => x.LocationId)
			.OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(x => new { x.LocationId, x.Date }).IsUnique();
	}
}