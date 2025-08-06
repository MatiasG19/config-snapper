using Matiasg19.ConfigSnapperConsole.CommandLine.Models;
using System.Reflection;

namespace Matiasg19.ConfigSnapperConsole.CommandLine;

public class CmdArgsParser
{
    private readonly string[] arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
    private readonly Dictionary<string, AbstractOption> registeredOptions = [];

    public void RegisterOptions(params AbstractOption[] options)
    {
        foreach (var option in options)
        {
            var attribute = option.GetType().GetCustomAttribute<Option>()!;
            registeredOptions.Add(attribute.ArgName, option);
            registeredOptions.TryAdd(attribute.ArgNameShort, option);
        }
    }

    public void Parse()
    {
        for (int i = 0; i < arguments.Length; i++)
        {
            registeredOptions.TryGetValue(arguments[i], out AbstractOption? option);
            bool emptyArg = false;
            if (option is null)
            {
                registeredOptions.TryGetValue("", out option);
                emptyArg = true;
            }
            if (option is null)
            {
                Console.Error.WriteLine($"Argument {arguments[i]} is not valid!");
                Environment.Exit(1);
            }

            option.IsSet = true;
            var attribute = option.GetType().GetCustomAttribute<Option>()!;
            var properties = option.GetType().GetProperties().Where(p => p.Name != "IsSet").ToArray();
            // TODO Fix mapping of multiple option arguments
            if (properties.Length > 0)
            {
                if (properties.Length != arguments.Length - i - (emptyArg ? 0 : 1))
                {
                    Console.Error.WriteLine("Argument count does not match option!");
                    Environment.Exit(1);
                }

                if (emptyArg == false) i++;
                int index = 0;
                for (; i < arguments.Length; i++)
                {
                    properties[index].SetValue(option, Convert.ChangeType(arguments[i], properties[index].PropertyType));
                }
            }
            if (attribute.Only)
            {
                if (i > 0)
                {
                    Console.Error.WriteLine($"Argument {arguments[i]} cannot be combined with other options!");
                    Environment.Exit(1);
                }
                break;
            }

        }
    }
}