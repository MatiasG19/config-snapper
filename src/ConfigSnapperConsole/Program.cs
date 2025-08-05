using Matiasg19.ConfigSnapper;
using Matiasg19.ConfigSnapperConsole.CommandLine;
using Matiasg19.ConfigSnapperConsole.CommandLine.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using Serilog;
using System.Reflection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

CmdArgsParser cmdParser = new();
var version = new AppVersion();
var initOnly = new Init();
var path = new PathToAppSettings();

cmdParser.RegisterAction(version);
cmdParser.RegisterAction(initOnly);
cmdParser.RegisterAction(path);

cmdParser.Parse();

if (version.IsSet)
{
    var assembly = Assembly.GetEntryAssembly()!;
    var internalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!;
    Console.WriteLine(internalVersion.Substring(0, internalVersion.IndexOf("+")));
    return;
}

const string appSettings = "appSettings.json";
string appSettingsPath = "";
if (path.IsSet)
{
    appSettingsPath = Path.Combine(path.Path, appSettings);
}
else
    appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), appSettings);

Console.WriteLine($"Using appSettings form {appSettingsPath}");

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

host.Services.UseConfigSnapper(initOnly.IsSet);