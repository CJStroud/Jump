﻿using Microsoft.Xna.Framework;

namespace Jump.Sprites.Obstacles
{
    public class RoofDoor : Obstacle
    {
        private const string AssetName = "Roof Door";

        #region Consts

        protected const int Width = 60;
        protected const int Height = 40;

        #endregion

        #region Constructor

        public RoofDoor(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - Height), Width, Height) { }

        #endregion
    }
}
