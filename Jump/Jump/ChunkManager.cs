using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump
{
    public class ChunkManager
    {
        public List<Chunk> Chunks;
        public int Right { get; private set; }

        private int _screenWidth;
        private int _nextPostionX;
        private int _defaultChunkWidth = 100;
        private int _defaultChunkHeight = 500;
        private ContentManager _content;

        public ChunkManager(int screenWidth)
        {
            Chunks = new List<Chunk>();
            _screenWidth = screenWidth;
        }

        public void Update(int leftOfScreen)
        {
            if (Chunks.Count > 0)
            {
                bool firstChunkIsOutsideScreen = Chunks[0].DestinationRectangle.Right < leftOfScreen;

                if (firstChunkIsOutsideScreen)
                {
                    Kill(0);
                    GenerateNext();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw each chunk
            foreach (Chunk chunk in Chunks)
            {
                chunk.Draw(spriteBatch);
            }
        }

        public void GenerateNext()
        {
            // Generate a new chunk and add it to the list
            Chunk chunk = new Chunk("Chunk", new Vector2(_nextPostionX, 500), _defaultChunkWidth, _defaultChunkHeight);
            chunk.LoadContent(_content);
            _nextPostionX += chunk.Width + 5;
            Right = chunk.BoundingBox.Right;

            bool yes = new Random().Next(10) > 1;
            if (yes)
                Chunks.Add(chunk);
        }

        public void Kill(int index)
        {
            // Try and remove the chunk at the specified index
            if (Chunks != null && Chunks.Count-1 >= index)
            {
                Chunks.RemoveAt(index);
            }
        }

        public Chunk CheckCollision(Rectangle playerBoundingBox)
        {
            // Check to see if the player intersected with any of the chunks, if it does return the chunk
            foreach (Chunk chunk in Chunks)
            {
                if (playerBoundingBox.Intersects(chunk.BoundingBox))
                {
                    return chunk;
                }
            }

            return null;
        }

        public void LoadContent(ContentManager content)
        {
            _content = content;
        }
    }
}
