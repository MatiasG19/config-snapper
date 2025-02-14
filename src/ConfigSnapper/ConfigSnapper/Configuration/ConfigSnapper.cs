using System.Text.Json.Serialization;

namespace Matiasg19.ConfigSnapper.Configuration;

public class ConfigSnapper
{
    [JsonRequired]
    public string SnapshotDirectory { get; set; } = "";

    [JsonRequired]
    public Dictionary<string, string> SnapConfigs { get; set; } = [];
}
