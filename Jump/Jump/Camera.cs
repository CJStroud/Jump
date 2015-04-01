using Microsoft.Xna.Framework;

namespace Jump
{
    public class Camera
    {
        public Matrix ViewMatrix;
        private Vector2 _position;
        private Vector2 _halfViewSize;

        public int Left { get { return (int)(Position.X -_halfViewSize.X); } }

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
                _position = value;
                UpdateViewMatrix();
            }
        }

        private void UpdateViewMatrix()
        {
            // Change the view matrix depending on the new position
            ViewMatrix = Matrix.CreateTranslation(_halfViewSize.X - _position.X, _halfViewSize.Y - _position.Y, 0.0f) * Matrix.CreateScale(0.65f, 0.65f, 0);
        }
    }
}
