using Microsoft.Xna.Framework;

namespace Jump.Chunks
{
    public class Chunk : Sprite
    {
        public bool IsCollidable { get; set; }

        public Chunk(string assetName, Vector2 position, int width, int height)
            : base(assetName, position, width, height)
        {
            IsCollidable = true;
        }
    }
}
