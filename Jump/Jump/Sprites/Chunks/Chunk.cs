using Jump.Sprites.Obstacles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprites.Chunks
{
    public class Chunk : Sprite
    {
        public bool IsCollidable { get; set; }
        public Obstacle Obstacle { get; set; }
        public bool HasObstacle { get { return Obstacle != null; } }

        public Chunk(string assetName, Vector2 position, int width, int height)
            : base(assetName, position, width, height)
        {
            IsCollidable = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Obstacle != null)
            {
                Obstacle.Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }

        public override void LoadContent(ContentManager content)
        {
            if (Obstacle != null)
            {
                Obstacle.LoadContent(content);
            }

            base.LoadContent(content);
        }
    }
}
