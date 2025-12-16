using System;
using System.IO;
using System.Text.Json;

namespace SokobanMG.Core
{
    public static class PlayerDataLoader
    {
        private static readonly string SaveDirectory =
            Path.Combine(AppContext.BaseDirectory, "Save");

        private static readonly string PlayerFilePath =
            Path.Combine(SaveDirectory, "player.json");

        public static string LoadPlayerName()
        {
            try
            {
                if (!File.Exists(PlayerFilePath))
                {
                    Directory.CreateDirectory(SaveDirectory);
                    SavePlayerName("Player");
                    return "Player";
                }

                string json = File.ReadAllText(PlayerFilePath);
                var data = JsonSerializer.Deserialize<PlayerData>(json);

                return string.IsNullOrWhiteSpace(data?.Name)
                    ? "Player"
                    : data.Name;
            }
            catch
            {
                return "Player";
            }
        }

        public static void SavePlayerName(string name)
        {
            Directory.CreateDirectory(SaveDirectory);

            var data = new PlayerData { Name = name };
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(PlayerFilePath, json);
        }

        private class PlayerData
        {
            public string Name { get; set; } = "Player";
        }
    }
}