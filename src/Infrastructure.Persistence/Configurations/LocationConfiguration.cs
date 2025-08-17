using Domain.Weather.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
	public void Configure(EntityTypeBuilder<Location> builder)
	{
		builder.ToTable("Locations");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
		builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
		builder.HasIndex(x => new { x.Name, x.Country }).IsUnique();
	}
}