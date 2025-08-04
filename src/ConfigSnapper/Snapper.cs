using ConfigSnapper.Extensions;
using Matiasg19.ConfigSnapper.Exceptions;
using Matiasg19.ConfigSnapper.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Matiasg19.ConfigSnapper;

public class Snapper : IDisposable
{
    private Configuration.ConfigSnapper _config;
    private List<FileSystemWatcher> _configWatchers = [];
    private ILogger<Snapper> _logger;
    private GitCommands git;

    private const string ConfigSnapperDirectoryName = "ConfigSnapperSnapshots";
    private const string BackupDirectoryName = "ConfigSnapperBackups";

    public Snapper(IOptions<Configuration.ConfigSnapper> config, ILogger<Snapper> logger)
    {
        _config = config.Value;
        _logger = logger;
        git = new("", _logger);

        Init();
    }

    public Snapper(Configuration.ConfigSnapper config)
    {
        _config = config;
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        _logger = loggerFactory.CreateLogger<Snapper>();
        git = new("", _logger);

        Init();
    }

    private void Init()
    {
        bool errors = false;
        if (!GitIsInstalled())
            errors = true;

        if (!CheckConfig())
            errors = true;

        if (errors == true)
            throw new ConfigSnapperException("Initialization failed! See error logs for details.");

        if (_config.Watch)
            InitializeWatchers(_config);
    }

    private bool GitIsInstalled()
    {
        if (git.IsInstalled())
            return true;

        _logger.LogError($"ConfigSnapper requires git installation!");
        return false;
    }

    private bool CheckConfig()
    {
        if (_config.SnapshotSourceFiles.Count > 0 && _config.SnapshotSourceDirectory.IsNotEmpty())
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
        _logger.LogInformation("ConfigSnapper create snapshot.");

        CreateDirectorySnapshot();
        CreateFileSnapshot();
    }

    private void CreateDirectorySnapshot()
    {
        if (_config.SnapshotSourceDirectory.IsEmpty())
            return;

        string context = _config.SnapshotSourceDirectory;

        InitializeGit(context);

        string directoryName = Path.GetFileName(context);
        if (git.CommitAndPush($"Snapshot for directory {directoryName}", _config.GitBranchName, _config.GitRemoteName))
        {
            _logger.LogInformation($"Snapshot created for directory {directoryName}");
        }
        else
            _logger.LogInformation($"No changes found for directory {directoryName}");
    }

    private void CreateFileSnapshot()
    {
        if (_config.SnapshotSourceFiles.Count == 0)
            return;

        string context = _config.SnapshotDirectory.IsEmpty() ?
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
            if (snapshotFileDirInitialized || git.Diff(sourcePath,
                Path.Combine(snapshotPath, Path.GetFileName(sourcePath))).IsNotEmpty())
            {
                CreateFileSnapshot(snapSource.Key, sourcePath, snapshotPath);
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

        InitializeGit(context);
    }

    private void InitializeGit(string context)
    {
        git.SetContext(context);
        bool gitRepoExists = git.RepositoryExists();
        if (!gitRepoExists)
        {
            _logger.LogInformation("Initialize Snapshot directory.");
            git.CreateGitignore(Constants.Resources.Gitignore);
            git.AddSafeDirectory();
            git.Initilize(_config.GitBranchName);
        }

        if (!gitRepoExists && _config.GitRemoteUrl.IsNotEmpty() && _config.GitRemoteName.IsNotEmpty())
        {
            git.AddRemote(_config.GitRemoteName, _config.GitRemoteUrl);
        }
    }

    private bool CreateSnapshotFileDirectory(string snapshotSourceName, string snapshotPath)
    {
        if (!Directory.Exists(snapshotPath))
        {
            _logger.LogInformation($"Initialize Snapshot directory for {snapshotSourceName}.");
            Directory.CreateDirectory(snapshotPath);
            return true;
        }
        return false;
    }

    private void CreateFileSnapshot(string sourceName, string sourcePath, string snapshotPath)
    {
        File.Copy(sourcePath, Path.Combine(snapshotPath, Path.GetFileName(sourcePath)), true);
        _logger.LogInformation($"File for {sourceName} copied.");

        if (git.CommitAndPush($"Snapshot for {sourceName}", _config.GitBranchName, _config.GitRemoteName))
            _logger.LogInformation($"Snapshot created for {sourceName}");
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

    public void Dispose()
    {
        foreach (var watcher in _configWatchers)
            watcher.Dispose();
    }
}