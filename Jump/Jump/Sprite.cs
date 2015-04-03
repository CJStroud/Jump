using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump
{
    public abstract class Sprite
    {
        public Texture2D Texture { get; private set; }
        protected int TextureWidth { get; set; }
        protected int TextureHeight { get; set; }

        // Todo change x and y to be parameter driven
        protected Rectangle SourceRectangle; 

        /// <summary>
        /// The position on screen of the sprite
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The X position on screen of the sprite
        /// </summary>
        public float X { get { return Position.X; } set { Position = new Vector2(value, Position.Y); } }
        
        /// <summary>
        /// The Y position on screen of the sprite
        /// </summary>
        public float Y { get { return Position.Y; } set { Position = new Vector2(Position.X, value); } }

        /// <summary>
        /// The Height of the Sprite
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The Width of the Sprite
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The area that the sprite occupies, used to determine the collision area
        /// </summary>
        public Rectangle BoundingBox { get { return new Rectangle((int)X, (int)Y, Width, Height); } }

        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public float LayerDepth { get; set; }

        /// <summary>
        /// Returns the rectangle that degins the area of the screen to draw the sprite
        /// </summary>
        public Rectangle DestinationRectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, TextureWidth, TextureHeight);}
        }

        private string _assetName;

        public Sprite(string assetName, Vector2 position, int width, int height)
        {
            // Set the Sprite properties
            _assetName = assetName;
            Position = position;
            Scale = 1f;
            Width = width;
            Height = height;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Check to make sure the texture isn't null
            if (Texture == null)
            {
                throw new NullReferenceException("The texture is null! Make sure that LoadContent has been called before drawing.");
            }

            // Draws the Sprite to the spritebatch. We use null as the source rectangle so it uses the entire texture.
            spriteBatch.Draw(Texture, DestinationRectangle, SourceRectangle, Color.White, Rotation, Origin, SpriteEffects.None, LayerDepth);    
        }

        public virtual void LoadContent(ContentManager content)
        {
            // Load the texture using the content manager
            Texture = content.Load<Texture2D>(_assetName);
            TextureWidth = TextureWidth == 0 ? Texture.Width : TextureWidth;
            TextureHeight = TextureHeight == 0 ? Texture.Height : TextureHeight;
            SourceRectangle = new Rectangle(0, 0, TextureWidth, TextureHeight);
        }

        public virtual void Update(GameTime gameTime)
        {
            List<Type> test = new List<Type>();

            test.Add(typeof(Player));

        }

    }
}
