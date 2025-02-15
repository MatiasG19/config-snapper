using Matiasg19.ConfigSnapper.Exceptions;
using Microsoft.Extensions.Options;
using System.Diagnostics;
namespace Matiasg19.ConfigSnapper;

public class ConfigSnapper
{
    private Configuration.ConfigSnapper _config;

    public ConfigSnapper(IOptions<Configuration.ConfigSnapper> config)
    {
        _config = config.Value;
    }

    public ConfigSnapper(Configuration.ConfigSnapper config)
    {
        _config = config;
    }

    public void CreateSnapshot()
    {
        Console.WriteLine($"ConfigSnapper starting...");
        string context = $"{_config.SnapshotDirectory}/ConfigSnapperSnapshots";

        // Create snapshot directory
        if (!Directory.Exists(context))
        {
            Directory.CreateDirectory(context);
            Console.WriteLine($"Snapshot directory created: {context}");
        }
        if (!Directory.Exists($"{context}/.git"))
        {
            ExecuteCommand(context, "git", "init");
            Console.WriteLine($"Snapshot directory initialized.");
        }

        // Create snapshot directories and copy configs into them
        foreach (var snapConfig in _config.SnapConfigs)
        {
            if (!File.Exists(snapConfig.Value))
            {
                Console.WriteLine($"Config for {snapConfig.Key} does not exist.");
                continue;
            }

            string dir = $"{context}/{snapConfig.Key}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Console.WriteLine($"Snapshot directory for {snapConfig.Key} initialized.");
            }

            File.Copy(snapConfig.Value, $"{dir}/{Path.GetFileName(snapConfig.Value)}", true);
            Console.WriteLine($"Config for {snapConfig.Key} copied.");
        }

        // Create snapshot
        if (!string.IsNullOrEmpty(ExecuteCommand(context, "git", "status --porcelain")))
        {
            ExecuteCommand(context, "git", "add . ");
            ExecuteCommand(context, "git", "commit -a -m \"Snapshot\"");
            Console.WriteLine($"Snapshot created.");
        }
        else
            Console.WriteLine($"No changes found.");
    }

    private string ExecuteCommand(string workingDirectory, string command, string arguments)
    {
        Process process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.WorkingDirectory = workingDirectory;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        // Start the process
        process.Start();

        // Read the output
        string result = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrWhiteSpace(error))
            throw new CommandExecutionException($"Error executing command: {command} {arguments}");

        return result;
    }
}
