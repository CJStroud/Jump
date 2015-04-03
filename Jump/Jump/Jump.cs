using System.Runtime.InteropServices;
using Jump.Chunks;
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

        public Player Player;
        public Camera Camera;
        public ChunkManager ChunkManager;

        private bool _gameIsPaused;
        private bool _isHoldingDownP;

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
            // set background colour
            

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

            ChunkManager.Update(Camera.Left, Camera.Right);

            Player.Update(gameTime);
            


            Sprite collidedSprite = ChunkManager.CheckCollision(Player.BoundingBox);
            if (collidedSprite is Chunk)
            {
                // todo need to check which side of the chunk the player is hitting.
                // if it is the top then this is right, else it should be a failure state
                 Player.IsGrounded = true;
                Player.Y = collidedSprite.Y - Player.Height;  
            }
            else if (collidedSprite is Obstacle)
            {
                Player.VelocityX = 0;
                Player.X += 60;
                Player.Y = 250;
            }

            Camera.Position = Player.Position;

            

            base.Update(gameTime);
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
            Player.Draw(spriteBatch);
            ChunkManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
