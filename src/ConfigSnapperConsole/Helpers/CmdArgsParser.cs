namespace Matiasg19.ConfigSnapperConsole.Helpers;

public class CmdArgsParser
{
    private readonly string[] arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
    private readonly Dictionary<string, Option> _actionMap = [];
    private readonly Dictionary<string, Option> _actionMapShort = [];

    public void RegisterAction(Option option)
    {
        _actionMap.Add(option.ArgName, option);
        _actionMapShort.Add(option.ArgNameShort, option);
    }

    public void Parse()
    {
        foreach (var arg in arguments)
        {
            if (arg.StartsWith("--"))
            {
                _actionMap.TryGetValue(arg.Substring(2), out Option? option);
                if (option is not null)
                {
                    option.Action();
                    break;
                }
            }
            else if (arg.StartsWith("-"))
            {
                _actionMapShort.TryGetValue(arg.Substring(2), out Option? option);
                if (option is not null)
                {
                    option.Action();
                    break;
                }
            }
        }
    }
}

public class Option
{
    public string Name { get; set; } = "";
    public string ArgName { get; set; } = "";
    public string ArgNameShort { get; set; } = "";
    public bool Parameters { get; set; }
    public Action Action { get; set; } = () => { };
}