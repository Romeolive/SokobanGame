using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SokobanMG.Core;

namespace SokobanMG;


public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;

        private const int TileSize = 64;

        // уровень
        private LevelData levelData;
        private int width;
        private int height;

        // статичная карта: 0=floor, 1=wall, 2=goal
        private int[,] staticMap;
        // ящики
        private List<Point> boxes = new List<Point>();
        // игрок
        private Point player;

        // клавиатура
        private KeyboardState prevKeyboard;

        // список уровней (для будущего расширения)
        private List<string> levelFiles = new List<string> { "Content/Maps/level1.json" };
        private int currentLevelIndex = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            LoadLevel(currentLevelIndex);
            prevKeyboard = Keyboard.GetState();
        }

        private void LoadLevel(int index)
        {
            string path = levelFiles[index];
            levelData = LevelLoader.LoadFromContent(path);
            width = levelData.Width;
            height = levelData.Height;

            // Подгоняем окно
            _graphics.PreferredBackBufferWidth = width * TileSize;
            _graphics.PreferredBackBufferHeight = height * TileSize;
            _graphics.ApplyChanges();

            staticMap = new int[width, height];
            boxes.Clear();

            for (int y = 0; y < height; y++)
            {
                string row = levelData.Map[y];
                for (int x = 0; x < width; x++)
                {
                    char ch = row[x];
                    switch (ch)
                    {
                        case '#':
                            staticMap[x, y] = 1;
                            break;
                        case '.':
                            staticMap[x, y] = 2;
                            break;
                        case '$':
                            staticMap[x, y] = 0;
                            boxes.Add(new Point(x, y));
                            break;
                        case '*':
                            staticMap[x, y] = 2;
                            boxes.Add(new Point(x, y));
                            break;
                        case '@':
                            staticMap[x, y] = 0;
                            player = new Point(x, y);
                            break;
                        case '+':
                            staticMap[x, y] = 2;
                            player = new Point(x, y);
                            break;
                        default:
                            staticMap[x, y] = 0;
                            break;
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
                Exit();

            // одноразовые нажатия
            if (IsPressed(Keys.W, ks) || IsPressed(Keys.Up, ks))
                TryMove(0, -1);
            else if (IsPressed(Keys.S, ks) || IsPressed(Keys.Down, ks))
                TryMove(0, 1);
            else if (IsPressed(Keys.A, ks) || IsPressed(Keys.Left, ks))
                TryMove(-1, 0);
            else if (IsPressed(Keys.D, ks) || IsPressed(Keys.Right, ks))
                TryMove(1, 0);

            prevKeyboard = ks;
            base.Update(gameTime);
        }

        private bool IsPressed(Keys key, KeyboardState ks) =>
            ks.IsKeyDown(key) && !prevKeyboard.IsKeyDown(key);

        private void TryMove(int dx, int dy)
        {
            int nx = player.X + dx;
            int ny = player.Y + dy;

            // границы
            if (nx < 0 || nx >= width || ny < 0 || ny >= height) return;

            // стена?
            if (staticMap[nx, ny] == 1) return;

            // есть ли ящик
            int boxIndex = boxes.FindIndex(b => b.X == nx && b.Y == ny);
            if (boxIndex >= 0)
            {
                int bx = nx + dx;
                int by = ny + dy;
                // границы и препятствия
                if (bx < 0 || bx >= width || by < 0 || by >= height) return;
                if (staticMap[bx, by] == 1) return;
                if (boxes.Exists(b => b.X == bx && b.Y == by)) return;

                // перемещаем ящик
                boxes[boxIndex] = new Point(bx, by);
            }

            // двигаем игрока
            player = new Point(nx, ny);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Статичные тайлы
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var rect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                if (staticMap[x, y] == 1)
                    _spriteBatch.Draw(_pixel, rect, Color.Gray);
                else if (staticMap[x, y] == 2)
                    _spriteBatch.Draw(_pixel, rect, Color.Yellow);
                // пол не рисуем — фон уже чёрный
            }

            // Ящики
            foreach (var b in boxes)
            {
                var rect = new Rectangle(b.X * TileSize, b.Y * TileSize, TileSize, TileSize);
                Color c = staticMap[b.X, b.Y] == 2 ? Color.Orange : Color.Brown;
                _spriteBatch.Draw(_pixel, rect, c);
            }

            // Игрок
            var pr = new Rectangle(player.X * TileSize, player.Y * TileSize, TileSize, TileSize);
            _spriteBatch.Draw(_pixel, pr, Color.Blue);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            _pixel?.Dispose();
            _spriteBatch?.Dispose();
            base.UnloadContent();
        }
    }