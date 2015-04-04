using System.Linq;
using Jump.Obstacles;
using Jump.Sprites;
using Jump.Sprites.Chunks;
using Jump.Sprites.Obstacles;
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

        public float ObstacleSpawnChance
        {
            get { return _obstacleSpawnChance; }
            set
            {
                if (value > 1f)
                {
                    _obstacleSpawnChance = 100;
                }
                else if (value < 0f)
                {
                    _obstacleSpawnChance = 0;
                }
                else
                {
                    _obstacleSpawnChance = (int)(value * 100);
                }
            }
        }

        private int _obstacleSpawnChance = 30;
        
        public List<Chunk> Chunks;
        public int Right { get; private set; }
        public int Left { get; private set; }

        private int _screenWidth;
        private int _nextPostionX;
        private int _defaultChunkWidth = 150;
        private int _defaultChunkHeight = 500;

        public Chunk LastIntersection { get; private set; }

        private ContentManager _content;

        private Rectangle _startingViewport;

        public int ChunkY = 500;

        public ChunkManager(Rectangle viewport)
        {
            Chunks = new List<Chunk>();
            _screenWidth = viewport.Width;
            _startingViewport = viewport;
        }

        public void LoadContent(ContentManager content)
        {
            _content = content;
            GenerateDefault();
        }

        public void Update(int leftOfScreen, int rightOfScreen)
        {
            if (rightOfScreen >= Right)
            {
                GenerateNext();
            }

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
            GenerateNext(false);
        }

        public void GenerateNext(bool normalChunk)
        {
            Chunk chunk;
            Random random = new Random();

            if (normalChunk)
            {
                chunk = new Chunk("Building", new Vector2(_nextPostionX, ChunkY), _defaultChunkWidth, _defaultChunkHeight); ;
            }
            else
            {
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
                    chunk = new HoleChunk("Building", new Vector2(_nextPostionX, ChunkY), _defaultChunkWidth, _defaultChunkHeight);
                }
                else
                {
                    if (lastChunk is HoleChunk)
                    {
                        int i = random.Next(3);
                        ChunkY = 475 + (i * 25);
                    }

                    chunk = new Chunk("Building", new Vector2(_nextPostionX, ChunkY), _defaultChunkWidth, _defaultChunkHeight);

                    bool hasObstacle = false;

                    if (lastChunk != null && !lastChunk.HasObstacle)
                    {
                        hasObstacle = random.Next(1, 100) < _obstacleSpawnChance;
                    }

                    if (hasObstacle)
                    {
                        int i = random.Next(3);

                        switch (i)
                        {
                            case 0: chunk.Obstacle = new RoofDoor(new Vector2(_nextPostionX + chunk.Width / 2, chunk.Y));
                                break;

                            case 1: chunk.Obstacle = new Antenna(new Vector2(_nextPostionX + chunk.Width / 2, chunk.Y));
                                break;

                            case 2: chunk.Obstacle = new Fan(new Vector2(_nextPostionX + chunk.Width / 2, chunk.Y));
                                break;
                            
                        }

                    }
                }
            }


            chunk.LoadContent(_content);
            _nextPostionX += chunk.Width;
            Right = chunk.BoundingBox.Right;

            Chunks.Add(chunk);
        }

        public void GenerateDefault()
        {
            while (_startingViewport.Right + _defaultChunkWidth > Right)
            {
                GenerateNext(true);
            } 
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

        public CollisionReason CheckCollision(Rectangle playerBoundingBox)
        {
            // Check to see if the player intersected with any of the chunks, if it does return the chunk
            foreach (Chunk chunk in Chunks)
            {
                if (chunk.Obstacle != null && playerBoundingBox.Intersects(chunk.Obstacle.BoundingBox))
                {
                    return CollisionReason.HitObstacle;
                }
                if (playerBoundingBox.Intersects(chunk.BoundingBox) && chunk.IsCollidable)
                {

                    LastIntersection = chunk;

                    Vector2 sourceMinimum = new Vector2(playerBoundingBox.X, playerBoundingBox.Y);
                    Vector2 sourceMaximum = new Vector2(playerBoundingBox.Right, playerBoundingBox.Bottom);

                    Vector2 targetMinimum = new Vector2(chunk.BoundingBox.X, chunk.BoundingBox.Y);
                    Vector2 targetMaximum = new Vector2(chunk.BoundingBox.Right, chunk.BoundingBox.Bottom);

                    Vector2 MinimumTranslationDistance = new Vector2();

                    float left = (targetMinimum.X - sourceMaximum.X);
                    float right = (targetMaximum.X - sourceMinimum.X);
                    float top = (targetMinimum.Y - sourceMaximum.Y);
                    float bottom = (targetMaximum.Y - sourceMinimum.Y);

                    if (left > 0 || right < 0) throw new Exception("no intersection");
                    if (top > 0 || bottom < 0) throw new Exception("no intersection");

                    // work out the mtd on both x and y axes.
                    if (Math.Abs(left) < right)
                        MinimumTranslationDistance.X = left;
                    else
                        MinimumTranslationDistance.X = right;

                    if (Math.Abs(top) < bottom)
                        MinimumTranslationDistance.Y = top;
                    else
                        MinimumTranslationDistance.Y = bottom;

                    // 0 the axis with the largest mtd value.
                    if (Math.Abs(MinimumTranslationDistance.X) < Math.Abs(MinimumTranslationDistance.Y))
                        MinimumTranslationDistance.Y = 0;
                    else
                        MinimumTranslationDistance.X = 0;

                    if (MinimumTranslationDistance.Y < MinimumTranslationDistance.X)
                    {
                        return CollisionReason.Gravity;
                    }
                    return CollisionReason.HitBuilding;
                }

            }

            return CollisionReason.None;
        }

        public void Reset()
        {
            ChunkY = 500;
            Chunks.Clear();
            Right = 0;
            Left = 0;
            _nextPostionX = 0;
            GenerateDefault();
        }
    }
}
