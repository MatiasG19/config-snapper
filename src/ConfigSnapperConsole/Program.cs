using Matiasg19.ConfigSnapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var configFile = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json", optional: true, reloadOnChange: false)
    .Build();

builder.Logging.ClearProviders();
builder.Services.AddSerilog(config =>
{
    config.ReadFrom.Configuration(configFile);
});

string[] arguments = Environment.GetCommandLineArgs();
builder.Services.AddConfigSnapper(arguments.Length > 1 ? arguments[1] : null);

var host = builder.Build();

host.Start();

host.Services.UseConfigSnapper(builder.Configuration);