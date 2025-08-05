namespace Matiasg19.ConfigSnapperConsole.CommandLine.Models;

[Option(ArgName = "--version", ArgNameShort = "-v")]
public sealed class AppVersion : AbstractOption { }

[Option(ArgName = "--init", ArgNameShort = "-i")]
public sealed class Init : AbstractOption { }

[Option]
public sealed class PathToAppSettings : AbstractOption
{
    public string Path { get; set; } = string.Empty;
}