﻿using Microsoft.Xna.Framework;

namespace Jump
{
    public class Camera
    {
        public Matrix ViewMatrix;
        private Vector2 _position;
        private Vector2 _halfViewSize;

        public Rectangle Viewport { get { return new Rectangle((int)(Position.X - _halfViewSize.X), (int)Position.Y, (int)_halfViewSize.X*2, (int)_halfViewSize.Y); } }

        private int _yToFollow = 463;

        public int Left { get { return (int)(Position.X -_halfViewSize.X); } }

        public int Right { get { return (int)(Position.X + _halfViewSize.X*2); } }

        public Camera(Rectangle clientRect)
        {
            // Set the default view matrix using the rectangle passed through as the focus
            _halfViewSize = new Vector2(clientRect.Width * 0.5f, clientRect.Height * 1f);
            UpdateViewMatrix();
        }

        /// <summary>
        /// The position of the area for the camera to focus on
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }

            set
            {
                // set x value if x position is higher
                if (value.X < _position.X)
                    return;
                _position = value;
                UpdateViewMatrix();
            }
        }

        public void Reset()
        {
            _position = new Vector2(0, _yToFollow);
        }


        private void UpdateViewMatrix()
        {
            // Change the view matrix depending on the new position
            ViewMatrix = Matrix.CreateTranslation(_halfViewSize.X - _position.X - 200, _halfViewSize.Y - (int)(_yToFollow*1.3), 0.0f) * Matrix.CreateScale(0.85f, 0.85f, 0);
        }
    }
}
