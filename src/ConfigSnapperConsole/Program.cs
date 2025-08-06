using Matiasg19.ConfigSnapper;
using Matiasg19.ConfigSnapperConsole.CommandLine;
using Matiasg19.ConfigSnapperConsole.CommandLine.Actions;
using Matiasg19.ConfigSnapperConsole.CommandLine.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

CmdArgsParser cmdParser = new();
var version = new AppVersion();
var initOnly = new Init();
var path = new PathToAppSettings();

cmdParser.RegisterOptions(version, initOnly, path);
cmdParser.Parse();

if (LogVersion.Action(version))
    return;

var configFile = new ConfigurationBuilder()
    .AddJsonFile(GetAppSettingsPath.Action(path), optional: false, reloadOnChange: false)
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

host.Services.UseConfigSnapper(initOnly.IsSet);