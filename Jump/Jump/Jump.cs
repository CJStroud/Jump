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
        private Button backButton;

        private bool mouseIsHeld;

        private GameState currentGameState = GameState.MainMenu;

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
            ChunkManager.LoadContent(Content);
            Camera.Position = Player.Position;
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
                        playerButton = new Button("play", _font, new Vector2(Camera.Left + 210, 275), FontColour, Color.White);
                    }
                    if (scoresButton == null)
                    {
                        scoresButton = new Button("high scores", _font, new Vector2(Camera.Left + 210, 325), FontColour, Color.White);
                    }
                    if (quitButton == null)
                    {
                        quitButton = new Button("quit", _font, new Vector2(Camera.Left + 210, 375), FontColour, Color.White);
                    }

                    playerButton.Update(mouseState, Camera);
                    scoresButton.Update(mouseState, Camera);
                    quitButton.Update(mouseState, Camera);

                    if (mouseIsHeld)
                    {
                        return;
                    }

                    if (playerButton.IsClicked)
                    {
                        Reset();
                    }
                    if (scoresButton.IsClicked)
                    {
                        currentGameState = GameState.Scores;
                    }
                    if (quitButton.IsClicked)
                    {
                        Exit();
                    }
                    break;
                    #endregion
                case GameState.Scores:
                        
                    break;

                case GameState.GameOver:
                    #region gameover
                    resetButton.Update(mouseState, Camera);
                    mainMenuButton.Update(mouseState, Camera);
                    if (resetButton.IsClicked)
                    {
                        Reset();
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
                        resetButton = new Button("retry", _font, new Vector2(Camera.Left + 210, 275), FontColour, Color.White);
                        mainMenuButton = new Button("main menu", _font, new Vector2(Camera.Left + 210, 325), FontColour, Color.White);
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
            // If the player presses P then pause or unpause the game
            // The is holding down p stops if from pausing and unpausing form one key press being read over mutliple updates
            if (keyboardState.IsKeyDown(Keys.P) && !_isHoldingDownP)
            {
                _isHoldingDownP = true;
                _gameIsPaused = !_gameIsPaused;
                currentGameState = _gameIsPaused ? GameState.Paused : GameState.Playing;
            }
            else if (!keyboardState.IsKeyDown(Keys.P))
            {
                _isHoldingDownP = false;
            }
        }

        public void Reset()
        {
            // Reset all of the game components and variables
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
                    spriteBatch.DrawString(_font, "jump", new Vector2(Camera.Left + 350, 150), FontColour, 0,
                        Vector2.Zero, 2f, SpriteEffects.None, 0);
                    playerButton.Draw(spriteBatch);
                    scoresButton.Draw(spriteBatch);
                    quitButton.Draw(spriteBatch);
                    break;
                case GameState.Scores:

                    break;
                case GameState.GameOver:
                    resetButton.Draw(spriteBatch);
                    mainMenuButton.Draw(spriteBatch);
                    spriteBatch.DrawString(_font, "game over", new Vector2(Camera.Left + 350, 150), FontColour, 0,
                        Vector2.Zero, 2f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(_font, "you scored:", new Vector2(Camera.Left + 600, 275), FontColour);
                    spriteBatch.DrawString(_font, _score.ToString(), new Vector2(Camera.Left + 600, 325), FontColour);
                    Player.Draw(spriteBatch);
                    break;
                case GameState.Paused:
                    spriteBatch.DrawString(_font, "paused", new Vector2(Camera.Left + 350, 150), FontColour, 0,
                        Vector2.Zero, 2f, SpriteEffects.None, 0);
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
    }
}
