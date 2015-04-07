using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jump.GUI
{
    public class Button
    {

        #region Public Properties

        public SpriteFont Font { get; protected set; }
        public string Text { get; protected set; }
        public Vector2 Position { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int) Position.X - 40, (int) Position.Y - 20, Width, Height); }
        }

        public Color DefaultColour { get; protected set; }
        public Color HoverColour { get; protected set; }
        public Color CurrentColour { get; protected set; }
        public bool IsIntersecting { get; protected set; }
        public bool IsClicked { get; set; }

        #endregion

        #region Private Fields

        private AudioManager _audioManager;
        private bool _played;

        #endregion

        #region Constructor

        public Button(string text, SpriteFont font, Vector2 position, Color defaultColour, Color hoverColor,
            AudioManager audioManager)
        {
            // assign constructor parameters to the properties
            Font = font;
            Text = text;
            Position = position;
            DefaultColour = defaultColour;
            HoverColour = hoverColor;
            CurrentColour = DefaultColour;

            // work out the size of the text
            Width = (int) font.MeasureString(text).X;
            Height = (int) font.MeasureString(text).Y;
            _audioManager = audioManager;
        }

        #endregion

        #region Public Methods

        public void Update(MouseState mouseState, Camera camera)
        {
            // default is clicked to false
            IsClicked = false;

            // work out where the mouse is using the camera
            Rectangle mouseBounds = new Rectangle((int) camera.Left + mouseState.X, camera.Top + mouseState.Y, 1, 1);

            // if the mouse is over the button change text colour to hover colour
            if (mouseBounds.Intersects(BoundingBox))
            {
                IsIntersecting = true;
                CurrentColour = HoverColour;

                //  if the user is pressing the button then set isclicked to true and play the button clikc sound effect
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    IsClicked = true;
                    if (!_played)
                    {
                        _audioManager.PlaySoundEffect("click");
                        _played = true;
                    }
                }
            }
            else
            {
                CurrentColour = DefaultColour;
                _played = false;
            }


        }

        public void Draw(SpriteBatch spritebatch)
        {
            // Draw the text to the screen using the properties
            spritebatch.DrawString(Font, Text, Position, CurrentColour);
        }

        #endregion

    }
}
