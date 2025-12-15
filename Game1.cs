using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SokobanMG.Core;
using System;
using System.Collections.Generic;

namespace SokobanMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // ===== WINDOW =====
        private const int WindowWidth = 1280;
        private const int WindowHeight = 800;

        private const int TileSize = 64;

        private enum GameState
        {
            Menu,
            Playing,
            Victory
        }

        private GameState currentState = GameState.Menu;

        // ===== MENU =====
        private const int ButtonSize = 100;
        private const int ButtonSpacing = 20;
        private const int ButtonsPerRow = 5;
        private const int TotalLevels = 10;

        private List<Rectangle> levelButtons;
        private List<Texture2D> levelButtonTextures;
        private Texture2D menuBackground;

        // ===== GAME =====
        private Level currentLevel;
        private int currentLevelIndex;

        private readonly List<string> levelFiles = new()
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
        private Texture2D winTexture;
        private Texture2D exitTexture;
        private Texture2D victoryBackground;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            menuBackground = LoadTexture("Content/Textures/background_solid_sky.png");
            victoryBackground = LoadTexture("Content/Textures/background_solid_sky.png");

            winTexture = LoadTexture("Content/Textures/torch_on_a.png");
            exitTexture = LoadTexture("Content/Textures/sign_exit.png");

            LoadGameTextures();
            LoadMenuButtons();
            CreateLevelButtons();

            prevKeyboard = Keyboard.GetState();
        }

        private void LoadGameTextures()
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

        private void LoadMenuButtons()
        {
            levelButtonTextures = new List<Texture2D>();

            for (int i = 0; i < TotalLevels; i++)
            {
                levelButtonTextures.Add(
                    LoadTexture($"Content/Textures/hud_character_{i}.png")
                );
            }
        }

        private Texture2D LoadTexture(string path)
        {
            using var stream = TitleContainer.OpenStream(path);
            return Texture2D.FromStream(GraphicsDevice, stream);
        }

        private void CreateLevelButtons()
        {
            levelButtons = new List<Rectangle>();

            int totalRows = (int)Math.Ceiling(TotalLevels / (float)ButtonsPerRow);
            int totalHeight = totalRows * ButtonSize + (totalRows - 1) * ButtonSpacing;
            int startY = (WindowHeight - totalHeight) / 2;

            int totalRowWidth = ButtonsPerRow * ButtonSize + (ButtonsPerRow - 1) * ButtonSpacing;
            int startX = (WindowWidth - totalRowWidth) / 2;

            for (int i = 0; i < TotalLevels; i++)
            {
                int row = i / ButtonsPerRow;
                int col = i % ButtonsPerRow;

                int x = startX + col * (ButtonSize + ButtonSpacing);
                int y = startY + row * (ButtonSize + ButtonSpacing);

                levelButtons.Add(new Rectangle(x, y, ButtonSize, ButtonSize));
            }
        }

        private void LoadLevel(int index)
        {
            if (index < 0 || index >= levelFiles.Count)
                return;

            LevelData data = LevelLoader.LoadFromContent(levelFiles[index]);
            currentLevel = new Level(data);
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

            Rectangle exitRect = new Rectangle(
                WindowWidth / 2 - 120,
                WindowHeight / 2 + 20,
                140,
                70
            );

            if (mouse.LeftButton == ButtonState.Pressed &&
                exitRect.Contains(mouse.Position))
            {
                currentState = GameState.Menu;
            }
        }

        private bool IsPressed(Keys key, KeyboardState ks)
        {
            return ks.IsKeyDown(key) && !prevKeyboard.IsKeyDown(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.Menu:
                    DrawMenu();
                    break;

                case GameState.Playing:
                    DrawGameplay();
                    break;

                case GameState.Victory:
                    DrawVictory();
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            _spriteBatch.Draw(
                menuBackground,
                new Rectangle(0, 0, WindowWidth, WindowHeight),
                Color.White
            );

            var mouse = Mouse.GetState();

            for (int i = 0; i < levelButtons.Count; i++)
            {
                bool hover = levelButtons[i].Contains(mouse.Position);

                _spriteBatch.Draw(
                    levelButtonTextures[i],
                    levelButtons[i],
                    hover ? Color.White : Color.LightGray
                );
            }
        }

        private void DrawGameplay()
        {
            int offsetX = (WindowWidth - currentLevel.Width * TileSize) / 2;
            int offsetY = (WindowHeight - currentLevel.Height * TileSize) / 2;

            currentLevel.Draw(
                _spriteBatch,
                TileSize,
                textures,
                offsetX,
                offsetY
            );
        }

        private void DrawVictory()
        {
            _spriteBatch.Draw(
                victoryBackground,
                new Rectangle(0, 0, WindowWidth, WindowHeight),
                Color.White
            );

            Rectangle winRect = new Rectangle(
                WindowWidth / 2 - 50,
                WindowHeight / 2 - 180,
                100,
                100
            );

            _spriteBatch.Draw(winTexture, winRect, Color.White);

            Rectangle exitRect = new Rectangle(
                WindowWidth / 2 - 120,
                WindowHeight / 2 + 20,
                140,
                70
            );

            var mouse = Mouse.GetState();
            bool hover = exitRect.Contains(mouse.Position);

            _spriteBatch.Draw(
                exitTexture,
                exitRect,
                hover ? Color.LightGray : Color.White
            );
        }
    }
}
