using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.Api;

public static class DependencyInjection
{
	public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers().AddNewtonsoftJson();

		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
		services.AddAuthorization();

		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
		}).AddApiExplorer(setup =>
		{
			setup.GroupNameFormat = "VVV";
			setup.SubstituteApiVersionInUrl = true;
		});

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddProblemDetails();

		services.AddHealthChecks();
		return services;
	}
}