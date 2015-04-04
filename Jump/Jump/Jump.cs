using System.Security.Cryptography;
using Jump.Sprites;
using Jump.Sprites.Chunks;
using Jump.Sprites.Obstacles;
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

        private SpriteFont _scoreFont;

        private int _score;

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
            Player = new Player("Player", new Vector2(400, 0), 20, 38, 50, 50, 4, 0.1f);
            ChunkManager = new ChunkManager(GraphicsDevice.Viewport.Bounds);
            ChunkManager.HoleSpawnChance = 0.35f;
            ChunkManager.ObstacleSpawnChance = 0.35f;

            Camera = new Camera(GraphicsDevice.Viewport.Bounds);

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
            _scoreFont = Content.Load<SpriteFont>("zekton free");

            Player.LoadContent(Content);
            ChunkManager.LoadContent(Content);
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
            KeyboardState keyboardState = Keyboard.GetState();

            // If the player presses escape, close the game
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // If the player presses P then pause or unpause the game
            // The is holding down p stops if from pausing and unpausing form one key press being read over mutliple updates
            if (keyboardState.IsKeyDown(Keys.P) && !_isHoldingDownP)
            {
                _isHoldingDownP = true;
                _gameIsPaused = !_gameIsPaused;
            }
            else if (!keyboardState.IsKeyDown(Keys.P))
            {
                _isHoldingDownP = false;
            }

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
                Reset();   
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
        }

        public void Reset()
        {
            // Reset all of the game components and variables
            _score = 0;

            Player.Reset();
            ChunkManager.Reset();
            Camera.Reset();
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
            spriteBatch.DrawString(_scoreFont, "score : " + _score, new Vector2(Camera.Left + 210, 130), FontColour, 0, Vector2.Zero, 0.85f, SpriteEffects.None, 0);
            Player.Draw(spriteBatch);
            ChunkManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
