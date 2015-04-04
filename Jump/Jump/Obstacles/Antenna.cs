using Microsoft.Xna.Framework;

namespace Jump.Obstacles
{
    public class Antenna : Obstacle
    {
        private const string AssetName = "Antenna";

        private const int w = 50;
        private const int h = 50;

        public Antenna(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - h), w, h)
        {

        }
    }
}
