using System.Text.Json.Serialization;

namespace Matiasg19.ConfigSnapper.Configuration;

public class ConfigSnapper
{
    [JsonRequired]
    public required string SnapshotDirectory;

    [JsonRequired]
    public required Dictionary<string, string> SnapConfigs;
}
