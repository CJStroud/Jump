using Jump.Chunks;
using Microsoft.Xna.Framework;

namespace Jump
{
    public class HoleChunk : Chunk
    {
        public HoleChunk(string assetName, Vector2 position, int width, int height) : base(assetName, position, width, height)
        {
            IsCollidable = false;
        }


    }
}
