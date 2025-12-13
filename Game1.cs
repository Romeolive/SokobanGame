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

        private enum GameState { Menu, Playing }
        private GameState currentState = GameState.Menu;

        private const int ButtonSize = 100;
        private const int ButtonSpacing = 20;
        private const int ButtonsPerRow = 5;
        private List<Rectangle> levelButtons;
        private int totalLevels = 10;

        private Level currentLevel;
        private int currentLevelIndex = 0;
        private List<string> levelFiles = new List<string>
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

        private KeyboardState prevKeyboard;
        private Dictionary<string, Texture2D> textures;

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

            LoadTextures();

            CreateLevelButtons();

            prevKeyboard = Keyboard.GetState();
        }

        private void LoadTextures()
        {
            textures = new Dictionary<string, Texture2D>
            {
                ["wall"] = LoadTexture("Content/Textures/wall.png"),
                ["box"] = LoadTexture("Content/Textures/box.png"),
                ["goal"] = LoadTexture("Content/Textures/goal.png"),
                ["floor"] = LoadTexture("Content/Textures/floor.png"),
                ["player"] = LoadTexture("Content/Textures/player.png")
            };
        }

        private Texture2D LoadTexture(string path)
        {
            using var stream = TitleContainer.OpenStream(path);
            return Texture2D.FromStream(GraphicsDevice, stream);
        }

        private void CreateLevelButtons()
        {
            levelButtons = new List<Rectangle>();
            int totalRows = (int)Math.Ceiling(totalLevels / (float)ButtonsPerRow);
            int startY = (GraphicsDevice.Viewport.Height - (totalRows * ButtonSize + (totalRows - 1) * ButtonSpacing)) / 2;

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
            if (currentState == GameState.Menu) UpdateMenu();
            else if (currentState == GameState.Playing) UpdateGameplay();

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
                        LoadLevel(currentLevelIndex);
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

            currentLevel?.Update(ks, prevKeyboard);

            prevKeyboard = ks;
        }

        private bool IsPressed(Keys key, KeyboardState ks)
        {
            return ks.IsKeyDown(key) && !prevKeyboard.IsKeyDown(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            if (currentState == GameState.Menu)
            {
                DrawMenu();
            }
            else if (currentState == GameState.Playing)
            {
                currentLevel?.Draw(_spriteBatch, TileSize, textures);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            var mouse = Mouse.GetState();

            foreach (var rect in levelButtons)
            {
                _spriteBatch.Draw(textures["floor"], rect, Color.Gray);
            }
        }
    }
}



