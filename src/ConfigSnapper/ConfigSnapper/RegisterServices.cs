using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Matiasg19.ConfigSnapper;

public static class RegisterServices
{
    public static void AddConfigSnapper(this IServiceCollection services, IConfiguration configuration)
    {
        Configuration.ConfigSnapper snapperConfig = new Configuration.ConfigSnapper();
        configuration.GetSection("ConfigSnapper").Bind(snapperConfig);
        if (snapperConfig is null)
        {
            Console.WriteLine("ConfigSnapper not initialized!");
            return;
        }

        services.AddLogging(builder => builder.AddConsole());

        if (snapperConfig.OpenTelemetry)
            services.AddLogging(builder => builder.AddOpenTelemetry(logging =>
            {
                logging.AddOtlpExporter();
            }));

        services.AddOptions<Configuration.ConfigSnapper>()
            .Bind(configuration.GetSection(nameof(Configuration.ConfigSnapper)));
        services.AddSingleton<Snapper>();

        Console.WriteLine("Service registered: ConfigSnapper");
    }

    public static void UseConfigSnapper(this IServiceProvider serviceCollection, IConfiguration configuration)
    {
        serviceCollection.GetService<Snapper>()?.CreateSnapshot();
    }
}
