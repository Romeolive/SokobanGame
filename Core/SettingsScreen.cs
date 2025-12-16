using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SokobanMG.Core
{
    public class SettingsScreen
    {
        private List<Texture2D> backgrounds;
        private int currentIndex = 0;

        public Texture2D CurrentBackground => backgrounds[currentIndex];

        public SettingsScreen(List<Texture2D> backgrounds)
        {
            this.backgrounds = backgrounds;
        }

        public void Update(MouseState mouse)
        {
            if (mouse.LeftButton != ButtonState.Pressed) return;

            currentIndex++;
            if (currentIndex >= backgrounds.Count)
                currentIndex = 0;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(
                font,
                "Click to change menu background",
                new Vector2(400, 300),
                Color.White
            );
        }
    }
}