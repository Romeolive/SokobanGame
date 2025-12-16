using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SokobanMG.Core
{
    public static class SimpleText
    {
        public const int PIXEL = 4;
        public const int SCALE = 2;

        public static void DrawString(SpriteBatch sb, Texture2D pixel, string text, Vector2 pos, Color color)
        {
            float x = pos.X;
            foreach (char ch in text)
            {
                DrawChar(sb, pixel, ch, new Vector2(x, pos.Y), color);
                x += PIXEL * SCALE * 4;
            }
        }

        static void DrawChar(SpriteBatch sb, Texture2D pixel, char ch, Vector2 pos, Color color)
        {
            if (ch == ' ')
                return;
            ch = char.ToUpper(ch);
            if (ch >= '0' && ch <= '9')
            {
                DrawNumber(sb, pixel, ch - '0', pos, color);
                return;
            }

            int pw = PIXEL * SCALE;
            int ph = PIXEL * SCALE;
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, pw*3, ph*5), color);
        }

        static void DrawNumber(SpriteBatch sb, Texture2D pixel, int d, Vector2 pos, Color color)
        {
            int pw = PIXEL * SCALE;
            int ph = PIXEL * SCALE;
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, pw*2, ph*3), color);
        }
    }
}
