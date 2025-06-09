using Matiasg19.ConfigSnapper.Exceptions;
using System.Diagnostics;

namespace ConfigSnapper.Helpers;

internal static class CommandLineHelper
{
    internal static string ExecuteCommand(string workingDirectory, string command, string arguments)
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