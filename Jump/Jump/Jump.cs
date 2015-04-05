using System.Collections.Generic;
using Jump.Sprites;
using Jump.Sprites.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jump
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Jump : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Color FontColour;

        public Player Player;
        public Camera Camera;
        public ChunkManager ChunkManager;

        private bool _gameIsPaused;
        private bool _isHoldingDownP;

        private SpriteFont _font;

        private int _score;

        private Button resetButton;
        private Button mainMenuButton;
        private Button playerButton;
        private Button scoresButton;
        private Button quitButton;

        private bool mouseIsHeld;

        private List<int> _scores; 

        private GameState currentGameState = GameState.MainMenu;

        private AudioManager _audioManager;

        public Jump()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // set font colour
            FontColour = new Color(10, 53, 83);

            // Create a new player object starting at X 100 and Y 100
            Player = new Player("Player", new Vector2(400, 200), 20, 38, 50, 50, 4, 0.1f);
            ChunkManager = new ChunkManager(GraphicsDevice.Viewport.Bounds);
            ChunkManager.HoleSpawnChance = 0.35f;
            ChunkManager.ObstacleSpawnChance = 0.35f;

            Camera = new Camera(GraphicsDevice.Viewport.Bounds);
            
            HighScoreManager.Initialise();
            _audioManager = new AudioManager(this);
            _audioManager.MusicVolume = 0.2f;
            _audioManager.SoundEffectVolume = 0.4f;
            Components.Add(_audioManager);

            Player.Intialise(_audioManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("zekton free");
            Player.LoadContent(Content);
            ChunkManager.LoadContent(Content, _audioManager);
            Camera.Position = Player.Position;

            _audioManager.LoadContent();
            _audioManager.LoadSoundEffect("jump", "Sounds/jump");
            _audioManager.LoadSoundEffect("click", "Sounds/click");
            _audioManager.LoadSoundEffect("scream", "Sounds/scream");
            _audioManager.LoadSong("background", "Sounds/background");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            MouseState mouseState = Mouse.GetState();

            if (mouseIsHeld && mouseState.LeftButton == ButtonState.Released)
            {
                mouseIsHeld = false;
            }

            IsMouseVisible = true;
            KeyboardState keyboardState = Keyboard.GetState();

            // If the player presses escape, close the game
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    #region main menu
                    if (playerButton == null)
                    {
                        playerButton = new Button("play", _font, new Vector2(Camera.Left + 210, 275), FontColour, Color.White, _audioManager);
                    }
                    if (scoresButton == null)
                    {
                        scoresButton = new Button("scores", _font, new Vector2(Camera.Left + 210, 325), FontColour, Color.White, _audioManager);
                    }
                    if (quitButton == null)
                    {
                        quitButton = new Button("quit", _font, new Vector2(Camera.Left + 210, 375), FontColour, Color.White, _audioManager);
                    }

                    playerButton.Update(mouseState, Camera);
                    scoresButton.Update(mouseState, Camera);
                    quitButton.Update(mouseState, Camera);

                    if (playerButton.IsClicked && !mouseIsHeld)
                    {
                        Reset();
                        _audioManager.PlaySong("background", true);
                    }
                    if (scoresButton.IsClicked && !mouseIsHeld)
                    {
                        currentGameState = GameState.Scores;
                        _scores = HighScoreManager.GetScores();
                        mainMenuButton = new Button("main menu", _font, new Vector2(Camera.Left + 210, 275), FontColour, Color.White, _audioManager);
                        quitButton = new Button("quit", _font, new Vector2(Camera.Left + 210, 325), FontColour, Color.White, _audioManager);
                        mouseIsHeld = true;
                    }
                    if (quitButton.IsClicked && !mouseIsHeld)
                    {
                        Exit();
                    }
                    break;
                    #endregion
                case GameState.Scores:
                    quitButton.Update(mouseState, Camera);
                    mainMenuButton.Update(mouseState, Camera);

                    if (quitButton.IsClicked && !mouseIsHeld)
                    {
                        Exit();
                    }
                    else if (mainMenuButton.IsClicked && !mouseIsHeld)
                    {
                        // Show main menu
                        Reset();
                        currentGameState = GameState.MainMenu;
                        mouseIsHeld = true;
                        quitButton = null;
                    }
                    break;

                case GameState.GameOver:
                    #region gameover
                    resetButton.Update(mouseState, Camera);
                    mainMenuButton.Update(mouseState, Camera);
                    if (resetButton.IsClicked)
                    {
                        Reset();
                        _audioManager.PlaySong("background", true);

                    }
                    else if (mainMenuButton.IsClicked)
                    {
                        // Show main menu
                        Reset();
                        currentGameState = GameState.MainMenu;
                        mouseIsHeld = true;
                    }
                    break;
                    #endregion
                case GameState.Paused:
                    PauseCheck();
                    break;

                case GameState.Playing:
                    #region playing
                    IsMouseVisible = false;
                    PauseCheck();
                    // if the game is paused then do not do any updates
                    if (_gameIsPaused)
                    {
                        return;
                    }

                    // Update the chunk manager
                    ChunkManager.Update(Camera.Left, Camera.Right);


                    CollisionReason reason = ChunkManager.CheckCollision(Player.BoundingBox);

                    // if the reason is because of gravity then just move the player so they are above the chunk they collided with
                    if (reason == CollisionReason.Gravity)
                    {
                        Player.IsGrounded = true;
                        Player.Y = ChunkManager.LastIntersection.Y - Player.Height + 1;
                    }
                    // if the reason is that the player hit an obstacle or fell off a building then reset the game
                    else if (reason == CollisionReason.HitObstacle || Player.Y > 700)
                    {
                        currentGameState = GameState.GameOver;
                        HighScoreManager.SaveScore(_score);
                        resetButton = new Button("retry", _font, new Vector2(Camera.Left + 210, 275), FontColour, Color.White, _audioManager);
                        mainMenuButton = new Button("main menu", _font, new Vector2(Camera.Left + 210, 325), FontColour, Color.White, _audioManager);
                        _audioManager.StopSong();
                        return;
                    }
                    // If the player hit a building then move them so they aren't intersecting the building and stop them travelling right
                    else if (reason == CollisionReason.HitBuilding)
                    {
                        Player.X -= Player.Width;
                        Player.VelocityX = 0;
                        Player.IsGrounded = false;
                    }
                    // if there is no collision the player must be off the ground
                    else if (reason == CollisionReason.None)
                    {
                        Player.IsGrounded = false;
                    }

                    Player.Update(gameTime);
                    Camera.Position = Player.Position;

                    _score += (int)Player.VelocityX;


                    base.Update(gameTime);
                    #endregion
                    break;

            }

        }

        private void PauseCheck()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            // If the player presses p then pause or unpause the game
            // The is holding down p stops if from pausing and unpausing form one key press being read over mutliple updates
            if (keyboardState.IsKeyDown(Keys.P) && !_isHoldingDownP)
            {
                _isHoldingDownP = true;
                _gameIsPaused = !_gameIsPaused;
                currentGameState = _gameIsPaused ? GameState.Paused : GameState.Playing;

                if (_audioManager.IsSongPlaying && _gameIsPaused)
                {
                    _audioManager.PauseSong();
                }
                if (_audioManager.IsSongPaused && !_gameIsPaused)
                {
                    _audioManager.ResumeSong();
                }
            }

            else if (!keyboardState.IsKeyDown(Keys.P))
            {
                _isHoldingDownP = false;
            }
        }



        public void Reset()
        {
            // Reset all of the game components and variables
            if (_audioManager.IsSongPlaying)
            {
                _audioManager.StopSong();
            }
            
            _score = 0;

            Player.Reset();
            ChunkManager.Reset();
            Camera.Reset();
            Camera.Position = Player.Position;
            currentGameState = GameState.Playing;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(58, 159, 229));

            // Draw the game components in relation to the camera
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.ViewMatrix);

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    DrawTitle("jump");
                    playerButton.Draw(spriteBatch);
                    scoresButton.Draw(spriteBatch);
                    if (quitButton != null)
                    {
                        quitButton.Draw(spriteBatch);
                    }
                    spriteBatch.DrawString(_font, "spacebar = jump", new Vector2(Camera.Left + 515, 275), FontColour, 0,
                        Vector2.Zero, 0.9f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "p = pause / resume", new Vector2(Camera.Left + 500, 325), FontColour, 0,
                        Vector2.Zero, 0.9f, SpriteEffects.None, 0);
                    break;
                case GameState.Scores:
                    DrawTitle("scores");
                    mainMenuButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "#1: " + (_scores.Count > 0 ? _scores[0].ToString() : "-"), new Vector2(Camera.Left + 500, 275), FontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "#2: " + (_scores.Count > 1 ? _scores[1].ToString() : "-"), new Vector2(Camera.Left + 500, 325), FontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "#3: " + (_scores.Count > 2 ? _scores[2].ToString() : "-"), new Vector2(Camera.Left + 500, 375), FontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "#4: " + (_scores.Count > 3 ? _scores[3].ToString() : "-"), new Vector2(Camera.Left + 700, 275), FontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "#5: " + (_scores.Count > 4 ? _scores[4].ToString() : "-"), new Vector2(Camera.Left + 700, 325), FontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    break;
                case GameState.GameOver:
                    resetButton.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);
                    DrawTitle("game over");
                    spriteBatch.DrawString(_font, "you scored:", new Vector2(Camera.Left + 600, 275), FontColour);
                    spriteBatch.DrawString(_font, _score.ToString(), new Vector2(Camera.Left + 600, 325), FontColour);
                    Player.Draw(spriteBatch);
                    break;
                case GameState.Paused:
                    DrawTitle("paused");
                    spriteBatch.DrawString(_font, "score : " + _score, new Vector2(Camera.Left + 10, 130), FontColour, 0,
                        Vector2.Zero, 0.85f, SpriteEffects.None, 0);
                    Player.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    spriteBatch.DrawString(_font, "score : " + _score, new Vector2(Camera.Left + 10, 130), FontColour, 0,
                        Vector2.Zero, 0.85f, SpriteEffects.None, 0);
                    Player.Draw(spriteBatch);
                    break;
            }

            ChunkManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawTitle(string text)
        {
            spriteBatch.DrawString(_font, text, new Vector2(Camera.Left + 350, 150), FontColour, 0,
                Vector2.Zero, 2f, SpriteEffects.None, 0);
        }

    }
}
