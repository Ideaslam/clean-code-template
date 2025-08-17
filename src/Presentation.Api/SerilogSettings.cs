using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Presentation.Api;

public static class SerilogSettings
{
	public static LoggerConfiguration ConfigureFromAppSettings(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
	{
		var esUri = configuration["Serilog:Elasticsearch:Uri"];
		if (!string.IsNullOrWhiteSpace(esUri))
		{
			loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUri))
			{
				AutoRegisterTemplate = true,
				IndexFormat = $"cleanarch-logs-{{0:yyyy.MM.dd}}"
			});
		}
		return loggerConfiguration;
	}
}