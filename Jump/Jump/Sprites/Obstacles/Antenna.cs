using Microsoft.Xna.Framework;

namespace Jump.Sprites.Obstacles
{
    public class Antenna : Obstacle
    {
        private const string AssetName = "Antenna";

        #region Consts

        protected const int Width = 50;
        protected const int Height = 50;

        #endregion

        #region Constructor

        public Antenna(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - Height), Width, Height) { }

        #endregion
    }
}
