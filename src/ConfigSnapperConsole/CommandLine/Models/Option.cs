namespace Matiasg19.ConfigSnapperConsole.CommandLine.Models;

public class Option : Attribute
{
    public string ArgName { get; set; } = "";
    public string ArgNameShort { get; set; } = "";
    public bool Only { get; set; } = false;
}

public class AbstractOption
{
    public bool IsSet { get; set; }
}