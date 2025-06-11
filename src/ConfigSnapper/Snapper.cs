using ConfigSnapper.Extensions;
using ConfigSnapper.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Matiasg19.ConfigSnapper;

public class Snapper : IDisposable
{
    private Configuration.ConfigSnapper _config;
    private List<FileSystemWatcher> _configWatchers = [];
    private ILogger<Snapper> _logger;

    private const string ConfigSnapperDirectoryName = "ConfigSnapperSnapshots";
    private const string BackupDirectoryName = "ConfigSnapperBackups";

    public Snapper(IOptions<Configuration.ConfigSnapper> config, ILogger<Snapper> logger)
    {
        _config = config.Value;
        _logger = logger;

        Init();
    }

    public Snapper(Configuration.ConfigSnapper config)
    {
        _config = config;
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        _logger = loggerFactory.CreateLogger<Snapper>();

        Init();
    }

    private void Init()
    {
        if (!GitIsInstalled())
            return;

        if (!CheckConfig())
            return;

        if (_config.Watch)
            InitializeWatchers(_config);
    }

    private bool GitIsInstalled()
    {
        bool result = false;
        try
        {
            result = CommandLineHelper.ExecuteCommand(".", "git", "--version").StartsWith("git version");
        }
        catch
        {
            _logger.LogError($"ConfigSnapper requires git installation!");
        }
        return result;
    }

    private bool CheckConfig()
    {
        if (_config.SnapshotSourceFiles.Count > 0 && !string.IsNullOrEmpty(_config.SnapshotSourceDirectory))
        {
            _logger.LogError("Configuration error. Snapshots can only be created for either files or a directory!");
            return false;
        }
        return true;
    }

    private void InitializeWatchers(Configuration.ConfigSnapper config)
    {
        foreach (var snapConfig in config.SnapshotSourceFiles)
        {
            string fileName = Path.GetFileName(snapConfig.Value);
            var watcher = new FileSystemWatcher
            {
                Path = snapConfig.Value.GetAbsolutePath().Replace(fileName, ""),
                Filter = fileName,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += (o, s) => { CreateSnapshot(); };
            watcher.EnableRaisingEvents = true;
            _configWatchers.Add(watcher);
        }
    }

    public void CreateSnapshot()
    {
        _logger.LogInformation("ConfigSnapper creating snapshot...");

        CreateFileSnapshot();
    }

    private void CreateFileSnapshot()
    {
        if (_config.SnapshotSourceFiles.Count == 0)
            return;

        string context = string.IsNullOrEmpty(_config.SnapshotDirectory) ? 
             Path.Combine(AppContext.BaseDirectory, ConfigSnapperDirectoryName) : 
             Path.Combine(_config.SnapshotDirectory.GetAbsolutePath(), ConfigSnapperDirectoryName);
        InitializeSnapshotDirectory(context);

        foreach (var snapSource in _config.SnapshotSourceFiles)
        {
            string sourcePath = snapSource.Value.GetAbsolutePath();
            if (!File.Exists(snapSource.Value.GetAbsolutePath()))
            {
                _logger.LogWarning($"Source file for {snapSource.Key} does not exist.");
                continue;
            }

            string snapshotPath = Path.Combine(context, snapSource.Key);
            bool snapshotFileDirInitialized = CreateSnapshotFileDirectory(snapSource.Key, snapshotPath);
            if (snapshotFileDirInitialized || !string.IsNullOrEmpty(CommandLineHelper
                .ExecuteCommand(AppContext.BaseDirectory, "git", $"diff --no-index {sourcePath} {Path.Combine(snapshotPath, Path.GetFileName(sourcePath))}")))
            {
                CreateSnapshot(context, snapSource.Key, sourcePath, snapshotPath);
                CreateBackup(sourcePath);
            }
        }
    }

    private void InitializeSnapshotDirectory(string context)
    {
        if (!Directory.Exists(context))
        {
            Directory.CreateDirectory(context);
            _logger.LogInformation($"Snapshot directory created: {context}");
        }
        var v = Path.Combine(context, ".git");
        if (!Directory.Exists(Path.Combine(context, ".git")))
        {
            CommandLineHelper.ExecuteCommand(context, "git", "init");

            if (!string.IsNullOrEmpty(_config.GitRemoteUrl))
            {
                CommandLineHelper.ExecuteCommand(context, "git", "remote add origin " + _config.GitRemoteUrl);
                _logger.LogInformation($"Remote Git remote repository added.");
            }

            _logger.LogInformation($"Snapshot directory initialized.");
        }
    }

    private bool CreateSnapshotFileDirectory(string snapshotSourceName, string snapshotPath)
    {
        if (!Directory.Exists(snapshotPath))
        {
            Directory.CreateDirectory(snapshotPath);
            _logger.LogInformation($"Snapshot directory for {snapshotSourceName} initialized.");
            return true;
        }
        return false;
    }

    private void CreateSnapshot(string context, string sourceName, string sourcePath, string snapshotPath)
    {
        File.Copy(sourcePath, Path.Combine(snapshotPath, Path.GetFileName(sourcePath)), true);
        _logger.LogInformation($"File for {sourceName} copied.");

        if (!string.IsNullOrEmpty(CommandLineHelper.ExecuteCommand(context, "git", "status --porcelain")))
        {
            CommandLineHelper.ExecuteCommand(context, "git", "add .");
            CommandLineHelper.ExecuteCommand(context, "git", $"commit -a -m \"Snapshot for {sourceName}\"");

            PushSnapshotToGitRemote(context);

            _logger.LogInformation($"Snapshot created for {sourceName}");
        }
        else
            _logger.LogInformation($"No changes found for {sourceName}");
    }

    private void CreateBackup(string sourcePath)
    {
        if (!_config.Backup)
            return;

        string fileName = Path.GetFileName(sourcePath);
        string backupPath = _config.BackupDirectory is null ?
            Path.Combine(AppContext.BaseDirectory, $"{fileName}_{BackupDirectoryName}") :
            Path.Combine(_config.BackupDirectory.GetAbsolutePath(), $"{fileName}_{BackupDirectoryName}");

        if (!Directory.Exists(backupPath))
            Directory.CreateDirectory(backupPath);

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string fileExtension = Path.GetExtension(fileName);
        string date = DateTime.Now.ToString("yyyyMMdd_HHmm_ss_fff");
        File.Copy(sourcePath.GetAbsolutePath(), Path.Combine(backupPath, $"{fileNameWithoutExtension}_{date}{fileExtension}"));
    }

    private void PushSnapshotToGitRemote(string context)
    {
        if (!string.IsNullOrEmpty(_config.GitRemoteUrl))
            return;

        var res = CommandLineHelper.ExecuteCommand(context, "git", "push origin " + _config.GitBranch);
        // TODO remove debug
        Console.WriteLine("PushSnapshotToGitRemote result: " + res);
        _logger.LogInformation($"Snapshot pushed to remote repository.");
    }

    public void Dispose()
    {
        foreach (var watcher in _configWatchers)
            watcher.Dispose();
    }
}