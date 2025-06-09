using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Matiasg19.ConfigSnapper.Configuration;

public class ConfigSnapper
{
    [Required]
    [JsonRequired]
    public Dictionary<string, string> SnapshotSources { get; set; } = [];

    public string? SnapshotDirectory { get; set; }

    public bool Watch { get; set; }

    public bool Backup { get; set; }

    public string? BackupDirectory { get; set; }

    public bool OpenTelemetry { get; set; }

    public string? GitRemoteUrl { get; set; }

    public string GitBranch { get; set; } = "main";
}