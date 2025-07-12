using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Serilog;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string[] arguments = Environment.GetCommandLineArgs();
const string appSettings = "appSettings.json";
string appSettingsPath = arguments.Length > 1 ?
    Path.Combine(arguments[1], appSettings) :
    Path.Combine(Directory.GetCurrentDirectory(), appSettings);

var configFile = new ConfigurationBuilder()
    .AddJsonFile(appSettingsPath, optional: false, reloadOnChange: false)
    .Build();

Matiasg19.ConfigSnapper.Configuration.ConfigSnapper snapperConfig = new();
configFile.GetSection(nameof(Matiasg19.ConfigSnapper.Configuration.ConfigSnapper)).Bind(snapperConfig);

builder.Services.AddOptions<Matiasg19.ConfigSnapper.Configuration.ConfigSnapper>()
    .Bind(configFile.GetSection(nameof(Matiasg19.ConfigSnapper.Configuration.ConfigSnapper)));

builder.Logging.ClearProviders();
builder.Services.AddSerilog(config =>
{
    config.ReadFrom.Configuration(configFile);
});

if (snapperConfig.OpenTelemetry)
{
    builder.Services.AddLogging(builder => builder.AddOpenTelemetry());
    var otel = builder.Services.AddOpenTelemetry();
    if (configFile["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
    {
        otel.UseOtlpExporter();
    }
}

builder.Services.AddConfigSnapper();

var host = builder.Build();

host.Start();

host.Services.UseConfigSnapper();