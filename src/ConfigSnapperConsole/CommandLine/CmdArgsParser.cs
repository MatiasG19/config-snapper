using Matiasg19.ConfigSnapperConsole.CommandLine.Models;
using System.Reflection;

namespace Matiasg19.ConfigSnapperConsole.CommandLine;

public class CmdArgsParser
{
    private readonly string[] arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
    private readonly Dictionary<string, AbstractOption> availableOptions = [];

    public void RegisterActions(params AbstractOption[] options)
    {
        foreach(var option in options) 
        {
            availableOptions.Add(option.GetType().GetCustomAttribute<Option>()!.ArgName, option);
            availableOptions.TryAdd(option.GetType().GetCustomAttribute<Option>()!.ArgNameShort, option);
        }
    }

    public void Parse()
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            availableOptions.TryGetValue(arguments[i], out AbstractOption? option);
            bool emptyArg = false;
            if (option is null)
            {
                availableOptions.TryGetValue("", out option);
                emptyArg = true;
            }
            if (option is not null)
            {
                option.IsSet = true;
                var attribute = option.GetType().GetCustomAttribute<Option>()!;
                var properties = option.GetType().GetProperties().Where(p => p.Name != "IsSet").ToArray();
                if (properties.Length > 0)
                {
                    if (properties.Length != arguments.Length - i - (emptyArg ? 0 : 1))
                        throw new ArgumentException("Arguments count does not match option!");

                    if (emptyArg == false) i++;
                    int index = 0;
                    for (; i < arguments.Length; i++)
                    {
                        properties[index].SetValue(option, Convert.ChangeType(arguments[i], properties[index].PropertyType));
                    }
                }

                if (attribute.Only)
                    break;
            }
        }
    }
}