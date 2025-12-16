using System.Collections.Generic;

namespace SokobanMG.Core
{
    public class LevelManager
    {
        public Level CurrentLevel { get; private set; }
        private List<string> levelFiles;

        public LevelManager(List<string> levelFiles)
        {
            this.levelFiles = levelFiles;
        }

        public void LoadLevel(int index)
        {
            if (index < 0 || index >= levelFiles.Count) return;
            var data = LevelLoader.LoadFromContent(levelFiles[index]);
            CurrentLevel = new Level(data);
        }
    }
}
