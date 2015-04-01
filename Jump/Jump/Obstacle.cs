using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Jump
{
    public class Obstacle : Sprite
    {
        public Obstacle(string assetName, Vector2 position, int width, int height) : base(assetName, position, width, height)
        {

        }
    }
}
