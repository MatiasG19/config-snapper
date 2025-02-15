using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Matiasg19.ConfigSnapper;

public static class RegisterServices
{
    public static void AddConfigSnapper(this IServiceCollection services, IConfiguration configuration)
    {
        var snapperConfig = configuration.GetSection("ConfigSnapper");
        if (snapperConfig is null)
        {
            Console.WriteLine("ConfigSnapper not initialized!");
            return;
        }

        services.AddLogging(builder => builder.AddConsole());
        services.AddOptions<Configuration.ConfigSnapper>()
            .Bind(configuration.GetSection(nameof(Configuration.ConfigSnapper)));
        services.AddSingleton<Snapper>();

        Console.WriteLine("Service registered: ConfigSnapper");
    }

    public static void UseConfigSnapper(this IServiceProvider serviceCollection)
    {
        var snapper = serviceCollection.GetService<Snapper>();
        snapper?.CreateSnapshot();
    }
}
