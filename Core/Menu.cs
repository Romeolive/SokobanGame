using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SokobanMG.Core
{
    public class Menu
    {
        public List<Rectangle> Buttons { get; private set; }
        private int totalLevels;
        private const int ButtonSize = 100;
        private const int ButtonSpacing = 20;
        private const int ButtonsPerRow = 5;

        public Menu(int totalLevels)
        {
            this.totalLevels = totalLevels;
            CreateButtons();
        }

        private void CreateButtons()
        {
            Buttons = new List<Rectangle>();
            int totalRows = (int)System.Math.Ceiling(totalLevels / (float)ButtonsPerRow);
            int totalHeight = totalRows * ButtonSize + (totalRows - 1) * ButtonSpacing;
            int startY = (720 - totalHeight) / 2;

            for (int i = 0; i < totalLevels; i++)
            {
                int row = i / ButtonsPerRow;
                int col = i % ButtonsPerRow;
                int totalRowWidth = ButtonsPerRow * ButtonSize + (ButtonsPerRow - 1) * ButtonSpacing;
                int startX = (1280 - totalRowWidth) / 2;

                Buttons.Add(new Rectangle(
                    startX + col * (ButtonSize + ButtonSpacing),
                    startY + row * (ButtonSize + ButtonSpacing),
                    ButtonSize,
                    ButtonSize));
            }
        }

        public int GetClickedButton(MouseState mouse)
        {
            if (mouse.LeftButton != ButtonState.Pressed) return -1;
            for (int i = 0; i < Buttons.Count; i++)
                if (Buttons[i].Contains(mouse.Position)) return i;
            return -1;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (var rect in Buttons)
                spriteBatch.Draw(pixel, rect, Color.Gray);
        }
    }
}
