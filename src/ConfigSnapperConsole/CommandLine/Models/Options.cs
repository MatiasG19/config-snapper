namespace Matiasg19.ConfigSnapperConsole.CommandLine.Models;

[Option(ArgName = "--version", ArgNameShort = "-v")]
public sealed class AppVersion : AbstractOption { }

[Option]
public sealed class PathToAppSettings : AbstractOption
{
    public string Path { get; set; } = string.Empty;
}