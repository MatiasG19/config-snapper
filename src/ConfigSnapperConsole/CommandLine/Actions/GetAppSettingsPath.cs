using Matiasg19.ConfigSnapperConsole.CommandLine.Models;

namespace Matiasg19.ConfigSnapperConsole.CommandLine.Actions;

public static class GetAppSettingsPath
{
    public static string Action(PathToAppSettings path)
    {
        const string appSettings = "appSettings.json";
        string appSettingsPath;
        if (path.IsSet)
        {
            appSettingsPath = Path.Combine(path.Path, appSettings);
        }
        else
            appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), appSettings);

        if (!File.Exists(appSettingsPath))
        {
            Console.Error.WriteLine($"appSettings.json does not exist at {appSettingsPath}");
            Environment.Exit(1);
        }

        Console.WriteLine($"Using appSettings form {appSettingsPath}");
        return appSettingsPath;
    }
}
