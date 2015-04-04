using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Jump.Obstacles
{
    public class RoofDoor : Obstacle
    {
        private const string AssetName = "Roof Door";
        private const int w = 60;
        private const int h = 40;

        public RoofDoor(Vector2 position)
            : base(AssetName, new Vector2(position.X, position.Y - h), 60, 40)
        {

        }
    }
}
