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
        string context = _config.SnapshotDirectory + "/ConfigSnapperSnapshots";

        // Create snapshot directory
        if (!Directory.Exists(context))
        {
            Console.WriteLine($"Snapshot directory does not exist.");
            Directory.CreateDirectory(context);
            ExecuteCommand(context, "git", "init");
            Console.WriteLine($"Snapshot directory created: {context}");
        }

        // Create snapshot directories and copy configs into them
        foreach (var snapConfig in _config.SnapConfigs)
        {
            string dir = $"{context}/{snapConfig.Key}";
            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"Snapshot for {snapConfig.Key} does not exist.");
                Directory.CreateDirectory(dir);
                File.Copy(snapConfig.Value, $"{dir}/{Path.GetFileName(snapConfig.Value)}");
                Console.WriteLine($"Snapshot for {snapConfig.Key} initialized.");
            }
            else
            {
                Console.WriteLine($"Snapshot for {snapConfig.Key} created.");
                File.Copy(snapConfig.Value, $"{dir}/{Path.GetFileName(snapConfig.Value)}", true);
            }
        }

        // Create snapshot
        if (!string.IsNullOrWhiteSpace(ExecuteCommand(context, "git", "status --porcelain")))
        {
            ExecuteCommand(context, "git", "add . ");
            ExecuteCommand(context, "git", "commit -a -m \"Snapshot\"");
            Console.WriteLine($"Snapshot created.");
        }
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
