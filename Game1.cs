using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SokobanMG.Core;
using System.Collections.Generic;

namespace SokobanMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private enum GameState
        {
            Menu,
            Playing,
            Victory
        }

        private GameState currentState = GameState.Menu;

        private MenuScreen menuScreen;
        private GameScreen gameScreen;
        private VictoryScreen victoryScreen;

        private KeyboardState prevKeyboard;

        private List<string> levelFiles;
        private List<Texture2D> levelButtonTextures;
        private Dictionary<string, Texture2D> gameTextures;

        private Texture2D menuBackground;
        private Texture2D victoryBackground;
        private Texture2D winTexture;
        private Texture2D exitTexture;
        private Texture2D pixel;

        private SpriteFont font;
        private int currentLevelIndex = -1;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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
            
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });


            font = Content.Load<SpriteFont>("Fonts/arial");
            
            gameTextures = new Dictionary<string, Texture2D>
            {
                ["wall"]   = LoadTexture("Content/Textures/wall.png"),
                ["box"]    = LoadTexture("Content/Textures/box.png"),
                ["goal"]   = LoadTexture("Content/Textures/goal.png"),
                ["floor"]  = LoadTexture("Content/Textures/floor.png"),
                ["player"] = LoadTexture("Content/Textures/player.png")
            };
            
            levelButtonTextures = new List<Texture2D>();
            for (int i = 0; i < 10; i++)
                levelButtonTextures.Add(
                    LoadTexture($"Content/Textures/hud_character_{i}.png")
                );
            
            levelFiles = new List<string>
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


            string savedPlayerName = PlayerDataLoader.LoadPlayerName();

            menuScreen = new MenuScreen(
                totalLevels: 10,
                background: menuBackground,
                buttonTextures: levelButtonTextures,
                pixel: pixel,
                font: font,
                initialName: savedPlayerName
            );
            
            menuScreen.onNameChanged = name =>
            {
                PlayerDataLoader.SavePlayerName(name);
            };

            prevKeyboard = Keyboard.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            var ks = Keyboard.GetState();
            var mouse = Mouse.GetState();

            switch (currentState)
            {
                case GameState.Menu:
                {
                    int clicked = menuScreen.Update(mouse, ks, prevKeyboard);
                    if (clicked != -1)
                    {
                        currentLevelIndex = clicked;
                        LevelData data = LevelLoader.LoadFromContent(levelFiles[currentLevelIndex]);

                        gameScreen = new GameScreen(
                            new Level(data),
                            gameTextures,
                            menuScreen.PlayerName,
                            font,
                            pixel
                        );

                        victoryScreen = new VictoryScreen(
                            winTexture,
                            exitTexture,
                            victoryBackground,
                            pixel
                        );

                        currentState = GameState.Playing;
                    }
                    break;
                }

                case GameState.Playing:
                {
                    gameScreen.Update(ks, prevKeyboard);
                    if (gameScreen.IsLevelCompleted())
                        currentState = GameState.Victory;
                    break;
                }

                case GameState.Victory:
                {
                    if (victoryScreen.Update(mouse))
                    {
                        currentState = GameState.Menu;
                        gameScreen = null;
                        currentLevelIndex = -1;
                    }
                    break;
                }
            }

            prevKeyboard = ks;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.Menu:
                    menuScreen.Draw(_spriteBatch);
                    break;
                case GameState.Playing:
                    gameScreen.Draw(_spriteBatch);
                    break;
                case GameState.Victory:
                    victoryScreen.Draw(_spriteBatch);
                    break;
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private Texture2D LoadTexture(string path)
        {
            using var stream = TitleContainer.OpenStream(path);
            return Texture2D.FromStream(GraphicsDevice, stream);
        }
    }
}
