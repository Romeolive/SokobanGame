using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SokobanMG.Core
{
    public class VictoryScreen
    {
        private Texture2D winTexture;
        private Texture2D exitTexture;
        private Texture2D background;
        private Texture2D pixel;

        private Vector2 exitPosition;
        private float exitScale = 1.5f; 
        private Rectangle exitRect; 

        public VictoryScreen(Texture2D win, Texture2D exit, Texture2D background, Texture2D pixel)
        {
            this.winTexture = win;
            this.exitTexture = exit;
            this.background = background;
            this.pixel = pixel;

 
            int exitWidth = (int)(exit.Width * exitScale);
            int exitHeight = (int)(exit.Height * exitScale);
            exitPosition = new Vector2(640 - exitWidth / 2, 500); 
            exitRect = new Rectangle((int)exitPosition.X, (int)exitPosition.Y, exitWidth, exitHeight);
        }

        public bool Update(MouseState mouse)
        {
            return mouse.LeftButton == ButtonState.Pressed && exitRect.Contains(mouse.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var viewport = spriteBatch.GraphicsDevice.Viewport;
            
            spriteBatch.Draw(background, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            
            int winWidth = winTexture.Width;
            int winHeight = winTexture.Height;
            Vector2 winPos = new Vector2(viewport.Width / 2 - winWidth / 2, 150);
            spriteBatch.Draw(winTexture, winPos, Color.White);

            //  кнопка Exit 
            var mouse = Mouse.GetState();
            bool hover = exitRect.Contains(mouse.Position);
            Color color = hover ? Color.LightGray : Color.White;

            spriteBatch.Draw(exitTexture, exitPosition, null, color, 0f, Vector2.Zero, exitScale, SpriteEffects.None, 0f);
        }
    }
}
