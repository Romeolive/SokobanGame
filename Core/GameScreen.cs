using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SokobanMG.Core
{
    public class GameScreen
    {
        private Level currentLevel;
        private Dictionary<string, Texture2D> textures;
        private int tileSize = 64;

        private string playerName;
        private SpriteFont font;
        private Texture2D pixel;

        public GameScreen(Level level, Dictionary<string, Texture2D> textures, string playerName, SpriteFont font, Texture2D pixel)
        {
            this.currentLevel = level;
            this.textures = textures;
            this.playerName = playerName;
            this.font = font;
            this.pixel = pixel;
        }

        public void Update(KeyboardState current, KeyboardState previous)
        {
            currentLevel.Update(current, previous);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int offsetX = (1280 - currentLevel.Width * tileSize) / 2;
            int offsetY = (720 - currentLevel.Height * tileSize) / 2;

            currentLevel.Draw(spriteBatch, tileSize, textures, offsetX, offsetY);

            // Рисуем имя игрока сверху
            spriteBatch.Draw(pixel, new Rectangle(10, 10, 300, 40), Color.Black * 0.5f);
            spriteBatch.DrawString(font, playerName, new Vector2(15, 15), Color.White);
        }

        public bool IsLevelCompleted()
        {
            return currentLevel.IsCompleted();
        }
    }
}