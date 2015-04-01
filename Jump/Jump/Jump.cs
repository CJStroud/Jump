using System;
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

        public Chunk Chunk1;
        public Chunk Chunk2;

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
            // Create a new player object starting at X 100 and Y 100
            Player = new Player("Player", new Vector2(0, 0), 20, 40);
            ChunkManager = new ChunkManager(GraphicsDevice.Viewport.Width);

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

            if (Player.X >= ChunkManager.Right - 100)
            {
                ChunkManager.GenerateNext();
            }

            ChunkManager.Update(Camera.Left);

            Player.Update(gameTime);

            Chunk collidedChunk = ChunkManager.CheckCollision(Player.BoundingBox);
            if (collidedChunk != null)
            {
                Player.IsGrounded = true;
                Player.Y = collidedChunk.Y - Player.Height;
            }
            else
            {
                // todo move this to player
                Player.IsGrounded = false;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the game components
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.ViewMatrix);
            Player.Draw(spriteBatch);
            ChunkManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
