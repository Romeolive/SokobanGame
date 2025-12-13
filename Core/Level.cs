using System.Collections.Generic;
using SokobanMG.Enteties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SokobanMG.Core
{
    public class Level
    {
        private int width;
        private int height;
        private Entity[,] map;
        private Point player;

        public Level(LevelData data)
        {
            width = data.Width;
            height = data.Height;

            map = new Entity[height, width];
            player = new Point(data.Player.X, data.Player.Y);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char c = data.Map[y][x];
                    map[y, x] = c switch
                    {
                        '#' => new Wall(),
                        '$' => new Box(),
                        '.' => new Goal(),
                        '@' => new Floor(),
                        ' ' => new Floor(),
                        _ => new Floor()
                    };
                }
            }
        }
        
        public bool IsCompleted()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[y, x] is Box)
                        return false;
                }
            }
            return true;
        }


        public void Update(KeyboardState current, KeyboardState previous)
        {
            if (IsPressed(Keys.W, current, previous) || IsPressed(Keys.Up, current, previous)) Move(0, -1);
            if (IsPressed(Keys.S, current, previous) || IsPressed(Keys.Down, current, previous)) Move(0, 1);
            if (IsPressed(Keys.A, current, previous) || IsPressed(Keys.Left, current, previous)) Move(-1, 0);
            if (IsPressed(Keys.D, current, previous) || IsPressed(Keys.Right, current, previous)) Move(1, 0);
        }

        private bool IsPressed(Keys key, KeyboardState current, KeyboardState previous)
        {
            return current.IsKeyDown(key) && !previous.IsKeyDown(key);
        }

        private void Move(int dx, int dy)
        {
            int nx = player.X + dx;
            int ny = player.Y + dy;

            if (!Inside(nx, ny)) return;

            var target = map[ny, nx];

            if (!target.IsWalkable && !target.IsPushable) return;

            if (target.IsPushable)
            {
                int bx = nx + dx;
                int by = ny + dy;

                if (!Inside(bx, by)) return;

                var next = map[by, bx];
                if (!next.IsWalkable) return;

                if (next is Goal)
                    map[by, bx] = new BoxOnGoal();
                else
                    map[by, bx] = new Box();

                map[ny, nx] = target is BoxOnGoal ? new Goal() : new Floor();
            }

            player = new Point(nx, ny);
        }

        private bool Inside(int x, int y)
            => x >= 0 && x < width && y >= 0 && y < height;

        public void Draw(SpriteBatch sb, int cellSize, Dictionary<string, Texture2D> textures)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Entity e = map[y, x];
                    Texture2D tex = e switch
                    {
                        Wall => textures["wall"],
                        Box => textures["box"],
                        BoxOnGoal => textures["box"],
                        Goal => textures["goal"],
                        Floor => textures["floor"],
                        _ => textures["floor"]
                    };

                    sb.Draw(tex, new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize), Color.White);
                }
            }

            sb.Draw(textures["player"], new Rectangle(player.X * cellSize, player.Y * cellSize, cellSize, cellSize), Color.White);
        }
    }
}



