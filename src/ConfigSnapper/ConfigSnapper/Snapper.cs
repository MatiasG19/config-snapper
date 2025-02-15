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

            watcher.Changed += (o, s) => CreateSnapshot();
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
        if (!string.IsNullOrEmpty(CommandLineHelper.ExecuteCommand(context, "git", "status --porcelain")))
        {
            CommandLineHelper.ExecuteCommand(context, "git", "add . ");
            CommandLineHelper.ExecuteCommand(context, "git", "commit -a -m \"Snapshot\"");
            Console.WriteLine($"Snapshot created.");
        }
        else
            Console.WriteLine($"No changes found.");
    }
}
