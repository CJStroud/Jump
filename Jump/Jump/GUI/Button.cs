using System;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jump.Sprites.GUI
{
    public class Button
    {
        public SpriteFont Font { get; protected set; }
        public string Text { get; protected set; }
        public Vector2 Position { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Rectangle BoundingBox { get { return new Rectangle((int)Position.X - 40, (int)Position.Y - 20, Width, Height);} }
        public Color DefaultColour { get; protected set; }
        public Color HoverColour { get; protected set; }
        public Color CurrentColour { get; protected set; }
        public bool IsIntersecting { get; protected set; }
        public bool IsClicked { get; set; }

        public Button(string text, SpriteFont font, Vector2 position, Color defaultColour, Color hoverColor)
        {
            Font = font;
            Text = text;
            Position = position;
            DefaultColour = defaultColour;
            HoverColour = hoverColor;
            CurrentColour = DefaultColour;
            Width = (int) font.MeasureString(text).X;
            Height = (int) font.MeasureString(text).Y;
        }

        public void Update(MouseState mouseState, Camera camera)
        {
            IsClicked = false;

            Rectangle mouseBounds = new Rectangle((int)camera.Left + mouseState.X, camera.Top + mouseState.Y, 1, 1);
            if (mouseBounds.Intersects(BoundingBox))
            {
                IsIntersecting = true;
                CurrentColour = HoverColour;
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    IsClicked = true;
                }
            }
            else
            {
                CurrentColour = DefaultColour;
            }


        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.DrawString(Font, Text, Position, CurrentColour);
            
        }
    }
}
