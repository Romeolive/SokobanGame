using System.IO;
using System.Text.Json;



using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;

namespace SokobanMG.Core
{
    public static class LevelLoader
    {
        // path пример: "Content/Maps/level1.json" или просто "Maps/level1.json" в этом проекте мы будем использовать "Content/Maps/level1.json"
        public static LevelData LoadFromContent(string relativePath)
        {
            // TitleContainer.OpenStream корректно работает в MonoGame для чтения встроенных/внешних ресурсов
            using var stream = TitleContainer.OpenStream(relativePath);
            using var reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            var level = JsonSerializer.Deserialize<LevelData>(json);
            return level;
        }
    }
}
