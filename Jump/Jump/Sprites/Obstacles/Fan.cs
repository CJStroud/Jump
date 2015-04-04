using Jump.Sprites.Obstacles;
using Microsoft.Xna.Framework;

namespace Jump.Obstacles
{
    public class Fan : Obstacle
    {
        private const string AssetName = "Fan";
        protected const int Width = 70;
        protected const int Height = 25;

        public Fan(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - Height), Width, Height)
        {
        }
    }
}
