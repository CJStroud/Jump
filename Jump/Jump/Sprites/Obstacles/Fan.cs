using Microsoft.Xna.Framework;

namespace Jump.Sprites.Obstacles
{
    public class Fan : Obstacle
    {
        private const string AssetName = "Fan";

        #region Consts

        protected const int Width = 70;
        protected const int Height = 25;

        #endregion

        #region Constructor

        public Fan(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - Height), Width, Height) { }

        #endregion
    }
}
