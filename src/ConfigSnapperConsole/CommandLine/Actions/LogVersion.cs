using Matiasg19.ConfigSnapperConsole.CommandLine.Models;
using System.Reflection;

namespace Matiasg19.ConfigSnapperConsole.CommandLine.Actions;

public static class LogVersion
{
    public static bool Action(AppVersion version)
    {
        if (version.IsSet)
        {
            Console.WriteLine(Assembly.GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion!);
            return true;
        }
        return false;
    }
}