using Microsoft.Xna.Framework;

namespace Jump
{
    public class Camera
    {
        #region Private Fields

        private Vector2 _position;
        private Vector2 _halfViewSize;
        private int _yToFollow = 463;

        #endregion

        #region Public Properties

        public static Matrix ViewMatrix;
        public Rectangle Viewport { get { return new Rectangle((int)(Position.X - _halfViewSize.X), (int)Position.Y, (int)_halfViewSize.X*2, (int)_halfViewSize.Y); } }
        public int Top { get { return 132; } }
        public int Left { get { return (int)(Position.X -_halfViewSize.X + 202); } }
        public int Right { get { return (int)(Position.X + _halfViewSize.X*2 + 200); } }
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

        #endregion

        #region Public Methods
        // Reset the camera values
        public void Reset()
        {
            _position = new Vector2(0, _yToFollow);
        }

        #endregion

        #region Private Methods

        private void UpdateViewMatrix()
        {
            // Change the view matrix depending on the new position
            ViewMatrix = Matrix.CreateTranslation(_halfViewSize.X - _position.X - 200, _halfViewSize.Y - (int)(_yToFollow*1.3), 0.0f) * Matrix.CreateScale(0.85f, 0.85f, 0);
        }

        #endregion
    }
}
