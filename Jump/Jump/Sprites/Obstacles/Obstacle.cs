using Microsoft.Xna.Framework;

namespace Jump.Sprites.Obstacles
{
    public abstract class Obstacle : Sprite
    {
        public Obstacle(string assetName, Vector2 position, int width, int height) : base(assetName, position, width, height)
        {

        }
    }
}
