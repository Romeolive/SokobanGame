using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SokobanMG.Core
{
    public class MenuScreen
    {
        private Texture2D background;
        private List<Texture2D> levelButtonTextures;
        private Texture2D pixel;
        private SpriteFont font;

        public Menu Menu { get; private set; }

        private string playerName;
        private bool isEditingName = false;
        private Rectangle nameRect = new Rectangle(490, 40, 300, 50);

        public Action<string>? onNameChanged;

        public string PlayerName => playerName;

        public MenuScreen(int totalLevels,
                          Texture2D background,
                          List<Texture2D> buttonTextures,
                          Texture2D pixel,
                          SpriteFont font,
                          string initialName = "Player")
        {
            this.background = background;
            this.levelButtonTextures = buttonTextures;
            this.pixel = pixel;
            this.font = font;
            Menu = new Menu(totalLevels);

            playerName = initialName; 
        }

        public int Update(MouseState mouse, KeyboardState ks, KeyboardState prev)
        {
            if (mouse.LeftButton == ButtonState.Pressed && nameRect.Contains(mouse.Position))
                isEditingName = true;

            if (isEditingName)
            {
                foreach (var key in ks.GetPressedKeys())
                {
                    if (prev.IsKeyDown(key)) continue;

                    if (key == Keys.Enter)
                    {
                        isEditingName = false;
                        onNameChanged?.Invoke(playerName);
                    }
                    else if (key == Keys.Back && playerName.Length > 0)
                        playerName = playerName[..^1];
                    else if (key >= Keys.A && key <= Keys.Z)
                        playerName += key.ToString();
                    else if (key == Keys.Space)
                        playerName += " ";
                }
            }

            return Menu.GetClickedButton(mouse);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);

            spriteBatch.Draw(pixel, nameRect, Color.Black * 0.5f);
            spriteBatch.DrawString(font, playerName, new Vector2(nameRect.X + 5, nameRect.Y + 5), Color.White);

            for (int i = 0; i < Menu.Buttons.Count; i++)
            {
                var rect = Menu.Buttons[i];
                bool hover = rect.Contains(Mouse.GetState().Position);
                spriteBatch.Draw(levelButtonTextures[i], rect,
                    hover ? Color.White : Color.LightGray);
            }
        }

        public void SetPlayerName(string name)
        {
            if (!string.IsNullOrEmpty(name))
                playerName = name;
        }
    }
}
