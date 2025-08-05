namespace Matiasg19.ConfigSnapper.Configuration;

public class ConfigSnapper
{
    public Dictionary<string, string> SnapshotSourceFiles { get; set; } = [];

    public string SnapshotSourceDirectory { get; set; } = string.Empty;

    public string SnapshotDirectory { get; set; } = string.Empty;

    public bool Watch { get; set; }

    public bool Backup { get; set; }

    public string BackupDirectory { get; set; } = string.Empty;

    public bool OpenTelemetry { get; set; }

    public string GitRemoteUrl { get; set; } = string.Empty;

    public string GitRemoteName { get; set; } = "origin";

    public string GitBranchName { get; set; } = "main";

    public string GitUserName { get; set; } = "ConfigSnapper";

    public string GitUserEmail { get; set; } = "git@configsnapper.com";
}