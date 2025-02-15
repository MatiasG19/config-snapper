using ConfigSnapper.Extensions;
using ConfigSnapper.Helpers;
using Microsoft.Extensions.Options;

namespace Matiasg19.ConfigSnapper;

public class Snapper
{
    private Configuration.ConfigSnapper _config;
    private List<FileSystemWatcher> _configWatchers = [];

    public Snapper(IOptions<Configuration.ConfigSnapper> config)
    {
        _config = config.Value;
        if (_config.Watch)
            InitializeWatchers(_config);
    }

    public Snapper(Configuration.ConfigSnapper config)
    {
        _config = config;
        if (_config.Watch)
            InitializeWatchers(_config);
    }

    private void InitializeWatchers(Configuration.ConfigSnapper config)
    {
        foreach (var snapConfig in config.SnapshotSources)
        {
            string fileName = Path.GetFileName(snapConfig.Value);
            var watcher = new FileSystemWatcher
            {
                Path = snapConfig.Value.GetAbsolutePath().Replace(fileName, ""),
                Filter = fileName,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += (o, s) =>
            {
                CreateSnapshot();
                CreateBackup(snapConfig.Value);
            };
            watcher.EnableRaisingEvents = true;
            _configWatchers.Add(watcher);
        }
    }

    public void CreateSnapshot()
    {
        Console.WriteLine($"ConfigSnapper starting...");

        string context = _config.SnapshotDirectory is null ?
            $"{AppContext.BaseDirectory}/ConfigSnapperSnapshots" :
            $"{_config.SnapshotDirectory.GetAbsolutePath()}/ConfigSnapperSnapshots";

        // Create snapshot directory
        if (!Directory.Exists(context))
        {
            Directory.CreateDirectory(context);
            Console.WriteLine($"Snapshot directory created: {context}");
        }
        if (!Directory.Exists($"{context}/.git"))
        {
            CommandLineHelper.ExecuteCommand(context, "git", "init");
            Console.WriteLine($"Snapshot directory initialized.");
        }

        // Create snapshot directories and copy configs into them
        foreach (var snapConfig in _config.SnapshotSources)
        {
            string sourcePath = snapConfig.Value.GetAbsolutePath();
            string fileName = Path.GetFileName(sourcePath);
            if (!File.Exists(snapConfig.Value.GetAbsolutePath()))
            {
                Console.WriteLine($"Config for {snapConfig.Key} does not exist.");
                continue;
            }

            string snapshotPath = $"{context}/{snapConfig.Key}";
            bool snapshotDirCreated = false;
            if (!Directory.Exists(snapshotPath))
            {
                Directory.CreateDirectory(snapshotPath);
                snapshotDirCreated = true;
                Console.WriteLine($"Snapshot directory for {snapConfig.Key} initialized.");
            }

            if (snapshotDirCreated || !string.IsNullOrEmpty(CommandLineHelper.ExecuteCommand(AppContext.BaseDirectory, "git", $"diff --no-index {sourcePath} {snapshotPath}/{fileName}")))
            {
                File.Copy(sourcePath, $"{snapshotPath}/{Path.GetFileName(sourcePath)}", true);
                Console.WriteLine($"Config for {snapConfig.Key} copied.");
                CreateBackup(sourcePath);
            }
        }

        // Create snapshot
        if (!string.IsNullOrEmpty(CommandLineHelper.ExecuteCommand(context, "git", "status --porcelain")))
        {
            CommandLineHelper.ExecuteCommand(context, "git", "add .");
            CommandLineHelper.ExecuteCommand(context, "git", "commit -a -m \"Snapshot\"");
            Console.WriteLine($"Snapshot created.");
        }
        else
            Console.WriteLine($"No changes found.");
    }

    private void CreateBackup(string sourcePath)
    {
        if (!_config.Backup)
            return;

        string fileName = Path.GetFileName(sourcePath);
        string backupPath = _config.BackupDirectory is null ?
            $"{AppContext.BaseDirectory}/{fileName}_ConfigSnapperBackups" :
            $"{_config.BackupDirectory.GetAbsolutePath()}/{fileName}_ConfigSnapperBackups";

        if (!Directory.Exists(backupPath))
            Directory.CreateDirectory(backupPath);

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string fileExtension = Path.GetExtension(fileName);
        string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        File.Copy(sourcePath.GetAbsolutePath(), $"{backupPath}/{fileNameWithoutExtension}_{date}{fileExtension}");
    }
}
