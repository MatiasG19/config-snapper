using Microsoft.Extensions.DependencyInjection;

namespace Matiasg19.ConfigSnapper;

public static class RegisterServices
{
    public static void AddConfigSnapper(this IServiceCollection services)
    {
        services.AddSingleton<Snapper>();
        Console.WriteLine("Service registered: ConfigSnapper");
    }

    public static void UseConfigSnapper(this IServiceProvider serviceCollection, bool initOnly)
    {
        var configSnapper = serviceCollection.GetRequiredService<Snapper>();
        if (initOnly)
        {
            configSnapper.Initialize();
            return;
        }

        configSnapper.CreateSnapshot();
    }
}