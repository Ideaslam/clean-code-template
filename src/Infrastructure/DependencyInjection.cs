using Infrastructure.Caching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		// Caching
		var redisEnabled = configuration.GetValue("Redis:Enabled", true);
		if (redisEnabled)
		{
			var cs = configuration.GetValue<string>("Redis:ConnectionString");
			if (!string.IsNullOrWhiteSpace(cs))
				services.AddStackExchangeRedisCache(o => o.Configuration = cs);
			else
				services.AddDistributedMemoryCache();
		}
		else
		{
			services.AddDistributedMemoryCache();
		}
		services.AddMemoryCache();
		services.AddSingleton<ICacheService, CacheService>();

		// HttpClients with Polly
		var retryPolicy = HttpPolicyExtensions
			.HandleTransientHttpError()
			.OrResult(r => (int)r.StatusCode == 429)
			.WaitAndRetryAsync(3, retry => TimeSpan.FromMilliseconds(200 * (retry + 1)));

		services.AddHttpClient("WeatherProvider", client =>
		{
			var baseUrl = configuration["ExternalServices:WeatherProvider:BaseUrl"] ?? "https://example-weather-provider";
			client.BaseAddress = new Uri(baseUrl);
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}).AddPolicyHandler(retryPolicy);

		// OpenTelemetry (optional)
		var otelEnabled = configuration.GetValue("OpenTelemetry:Enabled", false);
		if (otelEnabled)
		{
			services.AddOpenTelemetry().WithTracing(b =>
			{
				b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("CleanArchTemplate.Api"));
				b.AddAspNetCoreInstrumentation();
				b.AddHttpClientInstrumentation();
			});
		}

		return services;
	}
}