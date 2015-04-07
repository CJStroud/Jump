using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jump.Sprites.Chunks
{
    public class HoleChunk : Chunk
    {
        #region Constructor

        public HoleChunk(string assetName, Vector2 position, int width, int height) : base(assetName, position, width, height)
        {
            IsCollidable = false;
        }

        #endregion

        #region Public Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Don't draw anything because it's a hole!
        }

        #endregion
    }
}
