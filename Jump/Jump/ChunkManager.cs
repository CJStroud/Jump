using System;
using System.Collections.Generic;
using System.Linq;
using Jump.Sprites.Chunks;
using Jump.Sprites.Obstacles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Jump
{
    public class ChunkManager
    {
        #region Public Properties

        /// <summary>
        /// Number between 0 and 1 to determine how often holes spawn
        /// </summary>
        public float HoleSpawnChance
        {
            get { return _holeSpawnChance; }
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
                    _holeSpawnChance = (int) (value*100);
                }
            }
        }

        /// <summary>
        /// Number between 0 and 1 to determine how often obstacles spawn
        /// </summary>
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
                    _obstacleSpawnChance = (int) (value*100);
                }
            }
        }
        
        /// <summary>
        /// The current chunks the manager is dealing with
        /// </summary>
        public List<Chunk> Chunks;

        /// <summary>
        /// The right side of the chunk that is furthest right
        /// </summary>
        public int Right { get; private set; }

        /// <summary>
        /// The left side of the chunk that is furthest left
        /// </summary>
        public int Left { get; private set; }

        /// <summary>
        /// The last chunk that the player collided with
        /// </summary>
        public Chunk LastIntersection { get; private set; }

        #endregion

        #region Private Fields

        // Default spawn chances
        private int _holeSpawnChance = 30;
        private int _obstacleSpawnChance = 30;

        private int _screenWidth;
        private int _nextPostionX;
        private int _defaultChunkWidth = 150;
        private int _defaultChunkHeight = 500;     
        private ContentManager _content;
        private AudioManager _audioManager;
        private Rectangle _startingViewport;

        /// <summary>
        /// The y position of the chunks
        /// </summary>
        private int _chunkY = 500;


        #endregion

        #region Constructor

        public ChunkManager(Rectangle viewport)
        {
            Chunks = new List<Chunk>();
            _screenWidth = viewport.Width;
            _startingViewport = viewport;
        }

        #endregion

        #region Public Methods

        public void LoadContent(ContentManager content, AudioManager audioManager)
        {
            // Store content to load chunks and generate a default map
            _content = content;
            GenerateStart();
            _audioManager = audioManager;
        }

        public void Update(int leftOfScreen, int rightOfScreen)
        {
            // Check if the right side of the map is at the edge of the screen, if it is generate a new chunk
            if (rightOfScreen >= Right)
            {
                GenerateRandomChunk();
            }

            // check if the first chunk is outside of the screen, if it is then destroy it
            if (Chunks.Count > 0)
            {
                bool firstChunkIsOutsideScreen = Chunks[0].DestinationRectangle.Right < leftOfScreen;

                if (firstChunkIsOutsideScreen)
                {
                    DestroyChunkAt(0);
                    GenerateRandomChunk();
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

        public void Add(Chunk chunk)
        {
            // Check chunk isn't null
            if (chunk == null)
                return;

            // Chunk _content isn't null
            if (_content == null)
            {
                throw new NullReferenceException("Content is Null. You must run LoadContent before adding chunks.");
            }

            // Setup the chunk and adjust chunkmanager values
            chunk.LoadContent(_content);
            _nextPostionX += chunk.Width;
            Right = chunk.BoundingBox.Right;

            // Add chunk
            Chunks.Add(chunk);
        }

        public CollisionReason CheckCollision(Rectangle playerBoundingBox)
        {

            // check through all of the chunks
            foreach (Chunk chunk in Chunks)
            {
                #region Obstacle Check

                // if the player is intsercting with the chunks obstacle the return hit obstacle
                if (chunk.Obstacle != null && playerBoundingBox.Intersects(chunk.Obstacle.BoundingBox))
                {
                    _audioManager.PlaySoundEffect("death");
                    return CollisionReason.HitObstacle;
                }

                #endregion

                #region Chunk Check

                if (playerBoundingBox.Intersects(chunk.BoundingBox) && chunk.IsCollidable)
                {
                    // set this chunk as the chunk the player touched last
                    LastIntersection = chunk;

                    #region Collision Side Check

                    // get player top left vector and bottom right vector
                    Vector2 playerMin = new Vector2(playerBoundingBox.X, playerBoundingBox.Y);
                    Vector2 playerMax = new Vector2(playerBoundingBox.Right, playerBoundingBox.Bottom);

                    // get chunk top left vector and bottom right vector
                    Vector2 chunkMin = new Vector2(chunk.BoundingBox.X, chunk.BoundingBox.Y);
                    Vector2 chunkMax = new Vector2(chunk.BoundingBox.Right, chunk.BoundingBox.Bottom);

                    Vector2 minimumTranslationDistance = new Vector2();

                    // work out intersection distances
                    float left = (chunkMin.X - playerMax.X);
                    float right = (chunkMax.X - playerMin.X);
                    float top = (chunkMin.Y - playerMax.Y);
                    float bottom = (chunkMax.Y - playerMin.Y);

                    
                    // work out min translation distance on x and y
                    minimumTranslationDistance.X = Math.Abs(left) < right ? left : right;
                    minimumTranslationDistance.Y = Math.Abs(top) < bottom ? top : bottom;

                    #endregion

                    // if Y is bigger than x then the player has intersected from the top of the chunk
                    if (minimumTranslationDistance.Y < minimumTranslationDistance.X)
                    {
                        return CollisionReason.Gravity;
                    }

                    // Otherwise the player has hit from the side of the building, which means that they have fallen down a hole
                    _audioManager.PlaySoundEffect("death");
                    return CollisionReason.HitBuilding;
                }

                #endregion
            }

            // if nothing else returned then there were no collisions
            return CollisionReason.None;
        }

        /// <summary>
        /// Reset the ChunkManager variables
        /// </summary>
        public void Reset()
        {
            _chunkY = 500;
            Chunks.Clear();
            Right = 0;
            Left = 0;
            _nextPostionX = 0;
            GenerateStart();
        }

        #endregion

        #region Private Methods

        private void GenerateNormalChunk()
        {
            // Create a normal chunk and add it to the chunk list.
            Chunk chunk = new Chunk("Building", new Vector2(_nextPostionX, _chunkY), _defaultChunkWidth, _defaultChunkHeight);
            Add(chunk);
        }

        private void GenerateRandomChunk()
        {
            Chunk chunk;
            Random random = new Random();
            Chunk lastChunk = Chunks.LastOrDefault();

            #region Hole Logic

            // Randomise whether the next chunk is a hole
            bool isHole = false;
            bool doesNotNeedNormalChunk = lastChunk != null && !(lastChunk is HoleChunk) && !lastChunk.HasObstacle;

            if (doesNotNeedNormalChunk)
            {
                isHole = random.Next(1, 100) < _holeSpawnChance;
            }

                // Generate a new chunk and add it to the list
            if (isHole)
            {
                chunk = new HoleChunk("Building", new Vector2(_nextPostionX, _chunkY), _defaultChunkWidth,
                    _defaultChunkHeight);
            }

            #endregion

            else
            {
                // randomise the level of the buildings to generate after a hole
                if (lastChunk is HoleChunk)
                {
                    int i = random.Next(3);
                    _chunkY = 475 + (i*25);
                }

                chunk = new Chunk("Building", new Vector2(_nextPostionX, _chunkY), _defaultChunkWidth,
                    _defaultChunkHeight);

                #region Obstacle Logic

                // check to see if a obstacle can spawn and randomise whether it should
                bool hasObstacle = lastChunk != null && !lastChunk.HasObstacle && random.Next(1, 100) < _obstacleSpawnChance;

                // pick a radom obstacle to add to the chunk
                if (hasObstacle)
                {
                    int i = random.Next(3);

                    switch (i)
                    {
                        case 0:
                            chunk.Obstacle = new RoofDoor(new Vector2(_nextPostionX + chunk.Width/2, chunk.Y));
                            break;

                        case 1:
                            chunk.Obstacle = new Antenna(new Vector2(_nextPostionX + chunk.Width/2, chunk.Y));
                            break;

                        case 2:
                            chunk.Obstacle = new Fan(new Vector2(_nextPostionX + chunk.Width/2, chunk.Y));
                            break;

                    }

                }

                #endregion

            }

            // Add the generated chunk to the manager
            Add(chunk);

        }

        private void GenerateStart()
        {
            while (_startingViewport.Right + _defaultChunkWidth*2 > Right)
            {
                GenerateNormalChunk();
            } 
        }

        private void DestroyChunkAt(int index)
        {
            // Try and remove the chunk at the specified index
            if (Chunks != null && Chunks.Count-1 >= index)
            {
                Chunks.RemoveAt(index);
                Left = Chunks.First().BoundingBox.Left;
            }
        }

        #endregion
    }
}
