using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace SokobanMG.Core
{
    public static class LevelLoader
    {
        
        public static LevelData LoadFromContent(string relativePath)
        {
            using var stream = TitleContainer.OpenStream(relativePath);
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            var level = JsonSerializer.Deserialize<LevelData>(json);
            return level;
        }
    }
}
