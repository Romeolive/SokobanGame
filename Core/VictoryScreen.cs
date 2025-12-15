

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SokobanMG.Core
{
    public class VictoryScreen
    {
        private Texture2D winTexture;
        private Texture2D exitTexture;
        private Texture2D pixel;

        private Rectangle exitRect;

        public VictoryScreen(Texture2D win, Texture2D exit, Texture2D pixel)
        {
            this.winTexture = win;
            this.exitTexture = exit;
            this.pixel = pixel;

            exitRect = new Rectangle(540, 380, 200, 70);
        }

        public bool Update(MouseState mouse)
        {
            return mouse.LeftButton == ButtonState.Pressed && exitRect.Contains(mouse.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 720), Color.Black * 0.7f);
            spriteBatch.Draw(winTexture, new Rectangle(440, 160, 400, 200), Color.White);

            var mouse = Mouse.GetState();
            bool hover = exitRect.Contains(mouse.Position);
            spriteBatch.Draw(exitTexture, exitRect, hover ? Color.LightGray : Color.White);
        }
    }
}
