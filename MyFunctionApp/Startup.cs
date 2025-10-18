using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

[assembly: FunctionsStartup(typeof(MyFunctionApp.Startup))]

namespace MyFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                // Uncomment and add your Application Insights key if needed
                //.WriteTo.ApplicationInsights("<InstrumentationKey>", TelemetryConverter.Traces)
                .Enrich.FromLogContext()
                .CreateLogger();

            builder.Services.AddSingleton<ILoggerProvider>(new SerilogLoggerProvider(logger, dispose: true));
        }
    }
}
