using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace Uaeglp.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			var _logger = host.Services.GetRequiredService<ILogger<Program>>();
			_logger.LogInformation("Uaeglp starting...");

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				 .ConfigureLogging(logging =>
				 {
					 logging.ClearProviders();
					 logging.SetMinimumLevel(LogLevel.Trace);
					 logging.AddEventLog(
						 new EventLogSettings()
						 {
							 //SourceName = "UAEGLPScoure",
							 //LogName = "UAEGLPApplication",
							 //Filter = (x, y) => y >= LogLevel.Information
						 }
						 );
				 })
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
