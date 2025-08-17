using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Presentation.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
	.ConfigureFromAppSettings(builder.Configuration)
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddPresentation(builder.Configuration)
	.AddApplication()
	.AddInfrastructure(builder.Configuration)
	.AddPersistence(builder.Configuration);

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseProblemDetails();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");

app.Run();
