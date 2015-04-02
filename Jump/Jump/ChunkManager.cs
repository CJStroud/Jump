using System.Linq;
using Jump.Chunks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Jump
{
    public class ChunkManager
    {
        /// <summary>
        /// Number between 0 and 1 to determine how often holes spawn
        /// </summary>
        public float HoleSpawnChance { get { return _holeSpawnChance; }
            set
            {
                if (value > 1f)
                {
                    _holeSpawnChance = 100;
                }
                else if (value < 0f)
                {
                    _holeSpawnChance = 0;
                }
                else
                {
                    _holeSpawnChance = (int)(value * 100);
                }
            } 
        }

        private int _holeSpawnChance = 30;
        
        public List<Chunk> Chunks;
        public int Right { get; private set; }
        public int Left { get; private set; }

        private int _screenWidth;
        private int _nextPostionX;
        private int _defaultChunkWidth = 200;
        private int _defaultChunkHeight = 500;

        private int _defaultObsWidth = 30;
        private int _defaultObsHeight = 30;

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
            Chunk chunk;
            Random random = new Random();
            // Randomise whether the next chunk is a hole 
            bool isHole = false;
            Chunk lastChunk = Chunks.LastOrDefault();
            bool doesNotNeedNormalChunk = lastChunk != null && !(lastChunk is HoleChunk) && !lastChunk.HasObstacle;

            if (doesNotNeedNormalChunk)
            {
                isHole = random.Next(1, 100) < _holeSpawnChance;    
            }            

            // Generate a new chunk and add it to the list
            if (isHole)
            {
                chunk = new HoleChunk("Chunk", new Vector2(_nextPostionX, 500), _defaultChunkWidth, _defaultChunkHeight);
            }
            else
            {
                chunk = new Chunk("Chunk", new Vector2(_nextPostionX, 500), _defaultChunkWidth, _defaultChunkHeight);

                bool hasObstacle = false;

                if (lastChunk != null && !lastChunk.HasObstacle)
                {
                    hasObstacle = random.Next(2) == 1;
                }

                if (hasObstacle)
                {
                    chunk.Obstacle = new Obstacle("Obstacle", new Vector2(_nextPostionX + chunk.Width / 2, chunk.Y - _defaultObsHeight), _defaultObsWidth, _defaultObsHeight);
                }
            }
            chunk.LoadContent(_content);
            _nextPostionX += chunk.Width + 5;
            Right = chunk.BoundingBox.Right;

            Chunks.Add(chunk);
        }

        public void Kill(int index)
        {
            // Try and remove the chunk at the specified index
            if (Chunks != null && Chunks.Count-1 >= index)
            {
                Chunks.RemoveAt(index);
                Left = Chunks.First().BoundingBox.Left;
            }
        }

        public Sprite CheckCollision(Rectangle playerBoundingBox)
        {
            // Check to see if the player intersected with any of the chunks, if it does return the chunk
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.Obstacle != null && playerBoundingBox.Intersects(chunk.Obstacle.BoundingBox))
                {
                    return chunk.Obstacle;
                }
                if (playerBoundingBox.Intersects(chunk.BoundingBox) && chunk.IsCollidable)
                {
                    return chunk;
                }

            }

            return null;
        }

        private Chunk GetLast()
        {
            if (Chunks == null || Chunks.Count == 0)
            {
                return null;
            }

            return Chunks.Last();
        }

        public void LoadContent(ContentManager content)
        {
            _content = content;
        }
    }
}
