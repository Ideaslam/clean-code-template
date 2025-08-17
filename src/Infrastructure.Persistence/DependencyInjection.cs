using Application.Common.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DependencyInjection
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("SqlServer") ?? throw new InvalidOperationException("Missing connection string 'SqlServer'.");
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(connectionString, sql =>
			{
				sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
				sql.EnableRetryOnFailure(5);
			});
		});

		services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		return services;
	}
}