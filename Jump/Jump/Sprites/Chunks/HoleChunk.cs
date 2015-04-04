using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprites.Chunks
{
    public class HoleChunk : Chunk
    {
        public HoleChunk(string assetName, Vector2 position, int width, int height) : base(assetName, position, width, height)
        {
            IsCollidable = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw anything because it's a hole!
        }
    }
}
