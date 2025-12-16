using System.Text.Json.Serialization;

namespace SokobanMG.Core;


public class PlayerData
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "Player"; 
}

public class LevelData
{
    [JsonPropertyName("width")]  public int Width { get; set; }
    [JsonPropertyName("height")] public int Height { get; set; }

    [JsonPropertyName("player")] public PlayerData Player { get; set; }

    [JsonPropertyName("map")] public string[] Map { get; set; }
}