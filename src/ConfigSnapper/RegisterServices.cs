using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;

namespace Matiasg19.ConfigSnapper;

public static class RegisterServices
{
    private static string appSettings = "appSettings.json";

    public static void AddConfigSnapper(this IServiceCollection services, string? pathToAppSettings = null)
    {
        Configuration.ConfigSnapper snapperConfig = new Configuration.ConfigSnapper();

        string appSettingsPath = pathToAppSettings is not null ? 
            Path.Combine(pathToAppSettings, appSettings) :
            appSettings;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true)
            .Build();

        configuration.GetSection(nameof(Configuration.ConfigSnapper)).Bind(snapperConfig);

        services.AddOptions<Configuration.ConfigSnapper>()
            .Bind(configuration.GetSection(nameof(Configuration.ConfigSnapper)));

        services.AddLogging(builder => builder.AddConsole());

        if (snapperConfig.OpenTelemetry)
        {
            services.AddLogging(builder => builder.AddOpenTelemetry());
            var otel = services.AddOpenTelemetry();
            if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
            {
                otel.UseOtlpExporter();
            }
        }

        services.AddSingleton<Snapper>();

        Console.WriteLine("Service registered: ConfigSnapper");
    }

    public static void UseConfigSnapper(this IServiceProvider serviceCollection, IConfiguration configuration)
    {
        serviceCollection.GetRequiredService<Snapper>().CreateSnapshot();
    }
}