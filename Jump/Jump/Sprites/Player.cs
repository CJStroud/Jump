﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jump.Sprites
{
    public class Player : Sprite, IAnimated
    {
        #region Animation Variables

        public float TotalElapsed { get; set; }
        public float TimePerFrame { get; set; }
        public int Frame { get; set; }
        public int FrameCount { get; set; }

        #endregion

        #region Public Properties

        public bool IsGrounded { get; set; }
        public Vector2 Velocity { get; private set; }
        public float VelocityX { get { return Velocity.X; } set { Velocity = new Vector2(value, Velocity.Y); } }
        public float VelocityY { get { return Velocity.Y; } set { Velocity = new Vector2(Velocity.X, value); } }
        public float MaxSpeed = 5.0f;
        public float JumpSpeed = 6.0f;

        #endregion

        #region Private Fields

        private Vector2 _startingPosition;
        private AudioManager _audioManager;

        #endregion

        #region Constructors

        public Player(string assetName, Vector2 position, int width, int height, int textureWidth, int textureHeight) 
            : base(assetName, position, width, height)
        {
            TextureWidth = textureWidth;
            TextureHeight = textureHeight;
            _startingPosition = position;
        }

        public Player(string assetName, Vector2 position, int width, int height, int textureWidth, int textureHeight,
            int frameCount, float timePerFrame)
            : this(assetName, position, width, height, textureWidth, textureHeight)
        {
            FrameCount = frameCount;
            TimePerFrame = timePerFrame;
        }

        #endregion

        #region Public Methods

        public void Intialise(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();

            #region Animation

            if (!IsGrounded)
            {
                // if player is the air show the jump texture
                Frame = 4;
                SourceRectangle = new Rectangle(TextureWidth * Frame, SourceRectangle.Y, TextureWidth, TextureHeight);
            }
            else
            {
                TotalElapsed += elapsed;
                if (TotalElapsed > TimePerFrame)
                {
                    Frame++;
                    // Keep the Frame between 0 and the total frames, minus one.
                    Frame = Frame % FrameCount;
                    TotalElapsed -= TimePerFrame;
                    SourceRectangle = new Rectangle(TextureWidth * Frame, SourceRectangle.Y, TextureWidth, TextureHeight);
                }
            }


            #endregion

            #region Jumping Logic

            Vector2 gravity = new Vector2(0, 15f);
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (IsGrounded)
            {
                // Remove any downwards velocity
                VelocityY = 0;

                // If the player presses the jump key the make the player jump
                if (keyboard.IsKeyDown(Keys.Space))
                {
                   VelocityY -= JumpSpeed;
                    _audioManager.PlaySoundEffect("jump");
                }
            }
            else
            {
                // Apply gravity to the velocity
                Velocity += gravity * delta;
            }

            #endregion

            // speed up, unless player has reached max speed or is in the air
            if (VelocityX < MaxSpeed && IsGrounded)
            {
                VelocityX += 1f;
            }

            // Apply the velocity of the player to their position
            Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);

            base.Update(gameTime);
        }

        /// <summary>
        /// Reset the player variables
        /// </summary>
        public void Reset()
        {
            Velocity = Vector2.Zero;
            Position = _startingPosition;
            IsGrounded = false;
        }

        #endregion
    }
}
