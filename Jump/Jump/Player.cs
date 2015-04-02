using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jump
{
    public class Player : Sprite
    {
        public bool IsGrounded { get; set; }
        public Vector2 Velocity { get; private set; }
        public float VelocityX { get { return Velocity.X; } set { Velocity = new Vector2(value, Velocity.Y); } }
        public float VelocityY { get { return Velocity.Y; } set { Velocity = new Vector2(Velocity.X, value); } }

        public float MaxSpeed = 5.0f;
        public float JumpSpeed = 6.0f;

        public Player(string assetName, Vector2 position, int width, int height) 
            : base(assetName, position, width, height)
        {
        }

        public override void Update(GameTime gameTime)
        {

            KeyboardState keyboard = Keyboard.GetState();

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
                }
            }
            else
            {
                // Apply gravity to the velocity
                Velocity += gravity * delta;
            }

            #endregion

            if (VelocityX < MaxSpeed && IsGrounded)
            {
                VelocityX += 1f;
            }


            IsGrounded = false;
            // Apply the velocity of the player to their position
            Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);

            base.Update(gameTime);
        }
    }
}
