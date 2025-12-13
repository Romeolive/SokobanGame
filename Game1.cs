using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SokobanMG.Core;





namespace SokobanMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const int TileSize = 64;

        private enum GameState
        {
            Menu,
            Playing,
            Victory
        }

        private GameState currentState = GameState.Menu;


        private const int ButtonSize = 100;
        private const int ButtonSpacing = 20;
        private const int ButtonsPerRow = 5;
        private List<Rectangle> levelButtons;
        private int totalLevels = 10;

        private Level currentLevel;
        private int currentLevelIndex = 0;

        private List<string> levelFiles = new()
        {
            "Content/Maps/level1.json",
            "Content/Maps/level2.json",
            "Content/Maps/level3.json",
            "Content/Maps/level4.json",
            "Content/Maps/level5.json",
            "Content/Maps/level6.json",
            "Content/Maps/level7.json",
            "Content/Maps/level8.json",
            "Content/Maps/level9.json",
            "Content/Maps/level10.json"
        };

        // ====== INPUT ======
        private KeyboardState prevKeyboard;

        // ====== TEXTURES ======
        private Dictionary<string, Texture2D> textures;
        private Texture2D pixel;
        private Texture2D winTexture;
        private Texture2D exitTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // 1x1 pixel
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            winTexture = LoadTexture("Content/Textures/torch_on_a.png");
            exitTexture = LoadTexture("Content/Textures/sign_exit.png");

            LoadTextures();
            CreateLevelButtons();

            prevKeyboard = Keyboard.GetState();
        }
        
        private void LoadTextures()
        {
            textures = new Dictionary<string, Texture2D>
            {
                ["wall"]   = LoadTexture("Content/Textures/wall.png"),
                ["box"]    = LoadTexture("Content/Textures/box.png"),
                ["goal"]   = LoadTexture("Content/Textures/goal.png"),
                ["floor"]  = LoadTexture("Content/Textures/floor.png"),
                ["player"] = LoadTexture("Content/Textures/player.png"),
            };
        }

        private Texture2D LoadTexture(string path)
        {
            using var stream = TitleContainer.OpenStream(path);
            return Texture2D.FromStream(GraphicsDevice, stream);
        }

        // ====== MENU ======
        private void CreateLevelButtons()
        {
            levelButtons = new List<Rectangle>();

            int totalRows = (int)Math.Ceiling(totalLevels / (float)ButtonsPerRow);
            int totalHeight = totalRows * ButtonSize + (totalRows - 1) * ButtonSpacing;
            int startY = (GraphicsDevice.Viewport.Height - totalHeight) / 2;

            for (int i = 0; i < totalLevels; i++)
            {
                int row = i / ButtonsPerRow;
                int col = i % ButtonsPerRow;

                int totalRowWidth = ButtonsPerRow * ButtonSize + (ButtonsPerRow - 1) * ButtonSpacing;
                int startX = (GraphicsDevice.Viewport.Width - totalRowWidth) / 2;

                int x = startX + col * (ButtonSize + ButtonSpacing);
                int y = startY + row * (ButtonSize + ButtonSpacing);

                levelButtons.Add(new Rectangle(x, y, ButtonSize, ButtonSize));
            }
        }

        private void LoadLevel(int index)
        {
            if (index < 0 || index >= levelFiles.Count) return;

            var data = LevelLoader.LoadFromContent(levelFiles[index]);
            currentLevel = new Level(data);

            _graphics.PreferredBackBufferWidth = data.Width * TileSize;
            _graphics.PreferredBackBufferHeight = data.Height * TileSize;
            _graphics.ApplyChanges();
        }
        
        protected override void Update(GameTime gameTime)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    UpdateMenu();
                    break;

                case GameState.Playing:
                    UpdateGameplay();
                    break;

                case GameState.Victory:
                    UpdateVictory();
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateMenu()
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < levelButtons.Count; i++)
                {
                    if (levelButtons[i].Contains(mouse.Position))
                    {
                        currentLevelIndex = i;
                        LoadLevel(i);
                        currentState = GameState.Playing;
                        break;
                    }
                }
            }
        }

        private void UpdateGameplay()
        {
            var ks = Keyboard.GetState();

            if (IsPressed(Keys.Escape, ks))
            {
                currentState = GameState.Menu;
                return;
            }

            currentLevel.Update(ks, prevKeyboard);

            if (currentLevel.IsCompleted())
            {
                currentState = GameState.Victory;
            }

            prevKeyboard = ks;
        }

        private void UpdateVictory()
        {
            var mouse = Mouse.GetState();

            var viewport = GraphicsDevice.Viewport;
            var button = new Rectangle(
                viewport.Width / 2 - 100,
                viewport.Height / 2 + 20,
                200,
                50
            );

            if (mouse.LeftButton == ButtonState.Pressed &&
                button.Contains(mouse.Position))
            {
                currentState = GameState.Menu;
            }
        }

        private bool IsPressed(Keys key, KeyboardState ks)
        {
            return ks.IsKeyDown(key) && !prevKeyboard.IsKeyDown(key);
        }

        // ====== DRAW ======
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            if (currentState == GameState.Menu)
                DrawMenu();
            else if (currentState == GameState.Playing)
                currentLevel.Draw(_spriteBatch, TileSize, textures);
            else if (currentState == GameState.Victory)
                DrawVictory();

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            foreach (var rect in levelButtons)
            {
                _spriteBatch.Draw(pixel, rect, Color.Gray);
            }
        }

        private void DrawVictory()
        {
            var viewport = GraphicsDevice.Viewport;
            var mouse = Mouse.GetState();

            // затемнение фона
            _spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, viewport.Width, viewport.Height),
                Color.Black * 0.7f
            );
            
            Rectangle winRect = new Rectangle(
                viewport.Width / 2 - 200,
                viewport.Height / 2 - 220,
                100,
                100
            );

            _spriteBatch.Draw(winTexture, winRect, Color.White);
            
            Rectangle exitRect = new Rectangle(
                viewport.Width / 2 - 120,
                viewport.Height / 2 + 20,
                140,
                70
            );

            bool hover = exitRect.Contains(mouse.Position);
            _spriteBatch.Draw(
                exitTexture,
                exitRect,
                hover ? Color.LightGray : Color.White
            );
            
            if (hover && mouse.LeftButton == ButtonState.Pressed)
            {
                currentState = GameState.Menu;
            }
        }

        private void DrawBorder(Rectangle rect, int thickness, Color color)
        {
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            _spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }


    }
}




