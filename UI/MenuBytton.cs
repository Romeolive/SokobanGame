namespace SokobanMG.UI;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public class MenuButton
{
    public Rectangle Bounds;
    public string Text;

    private SpriteFont _font;
    private Texture2D _texture;
    private Color _normalColor = Color.DarkSlateGray;
    private Color _hoverColor = Color.SlateGray;

    public MenuButton(string text, Rectangle bounds, SpriteFont font, GraphicsDevice device)
    {
        Text = text;
        Bounds = bounds;
        _font = font;

        _texture = new Texture2D(device, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public bool IsHovered(Point mousePos)
        => Bounds.Contains(mousePos);

    public void Draw(SpriteBatch spriteBatch, Point mousePos)
    {
        var color = IsHovered(mousePos) ? _hoverColor : _normalColor;
        spriteBatch.Draw(_texture, Bounds, color);

        // текст по центру кнопки
        Vector2 size = _font.MeasureString(Text);
        Vector2 pos = new Vector2(
            Bounds.X + (Bounds.Width - size.X) / 2,
            Bounds.Y + (Bounds.Height - size.Y) / 2);

        spriteBatch.DrawString(_font, Text, pos, Color.White);
    }
}
