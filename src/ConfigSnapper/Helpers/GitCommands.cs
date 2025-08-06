using ConfigSnapper.Helpers;
using Microsoft.Extensions.Logging;

namespace Matiasg19.ConfigSnapper.Helpers;

public class GitCommands(string context, ILogger logger)
{
    public void SetContext(string newContext)
    {
        context = newContext;
    }

    public string Version()
    {
        return CommandLineHelper.ExecuteCommand(context, "git", "--version");
    }

    public bool IsInstalled()
    {
        try
        {
            return Version().StartsWith("git version");
        }
        catch
        {
            return false;
        }
    }

    public void CreateGitignore(string gitignore)
    {
        string filePath = Path.Combine(context, ".gitignore");
        if (!File.Exists(filePath))
        {
            logger.LogInformation("Create Gitignore.");
            File.WriteAllText(filePath, gitignore);
        }
    }

    public bool RepositoryExists()
    {
        return Directory.Exists(Path.Combine(context, ".git"));
    }

    public void Initialize(string branchName)
    {
        logger.LogInformation("Initialize git repository.");
        CommandLineHelper.ExecuteCommand(context, "git", $"init -b {branchName}");
    }

    public bool UncommittedChangesExist()
    {
        return !string.IsNullOrEmpty(CommandLineHelper.ExecuteCommand(context, "git", "status --porcelain"));
    }

    public void CommitAll(string message)
    {
        logger.LogInformation($"Commit all changes to git.");
        CommandLineHelper.ExecuteCommand(context, "git", "add .");
        CommandLineHelper.ExecuteCommand(context, "git", $"commit -a -m \"{message}\"");
    }

    public bool CommitAndPush(string message, string branchName, string remoteName)
    {
        if (UncommittedChangesExist())
        {
            CommitAll(message);
            Push(branchName, remoteName);
            return true;
        }
        return false;
    }

    public void Push(string branchName, string remoteName)
    {
        if (!RemoteExists(remoteName))
            return;

        logger.LogInformation("Push to remote git repository.");
        CommandLineHelper.ExecuteCommand(context, "git", $"push -u {remoteName} {branchName}");
    }

    public void RenameCurrentBranch(string newName)
    {
        CommandLineHelper.ExecuteCommand(context, "git", $"branch -M {newName}");
    }

    public string Diff(string path1, string path2)
    {
        return CommandLineHelper.ExecuteCommand(context, "git", $"diff --no-index {path1} {path2}");
    }

    public bool RemoteExists(string remoteName)
    {
        return CommandLineHelper.ExecuteCommand(context, "git", $"remote").Contains(remoteName);
    }

    public bool AddRemote(string remoteName, string remoteUrl)
    {
        if (RemoteExists(remoteName))
        {
            logger.LogInformation($"Git remote with name {remoteName} already exists");
            return false;
        }

        logger.LogInformation("Add git remote repository.");
        CommandLineHelper.ExecuteCommand(context, "git", $"remote add {remoteName} {remoteUrl}");
        return true;
    }

    public void AddSafeDirectory()
    {
        logger.LogInformation($"Add {context} to git safe directories.");
        CommandLineHelper.ExecuteCommand(context, "git", $"config --add safe.directory {context}");
    }

    public void SetUserNameAndEmail(string name, string email)
    {
        logger.LogInformation($"Set git user {name} and {email}");
        CommandLineHelper.ExecuteCommand(context, "git", $"config user.name \"{name}\"");
        CommandLineHelper.ExecuteCommand(context, "git", $"config user.email \"{email}\"");
    }
}