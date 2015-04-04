using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Jump.Obstacles
{
    public class Fan : Obstacle
    {
        private const string AssetName = "Fan";

        private const int w = 70;
        private const int h = 25;

        public Fan(Vector2 position) 
            : base(AssetName, new Vector2(position.X, position.Y - h), w, h)
        {
        }
    }
}
