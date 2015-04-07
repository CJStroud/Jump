using System.Collections.Generic;
using Jump.GUI;
using Jump.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jump
{
    /// <summary>
    /// Main game class
    /// </summary>
    public class Jump : Game
    {
        #region Private Fields

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Color _fontColour;
        private Player _player;
        private Camera _camera;
        private ChunkManager _chunkManager;
        private bool _gameIsPaused;
        private bool _isHoldingDownP;
        private SpriteFont _font;
        private int _score;
        private Button _resetButton;
        private Button _mainMenuButton;
        private Button _playerButton;
        private Button _scoresButton;
        private Button _quitButton;
        private bool _mouseIsHeld;
        private List<int> _scores; 
        private GameState _currentGameState = GameState.MainMenu;
        private AudioManager _audioManager;

        #endregion

        #region Constructor

        public Jump()
        {
            // Do not remove _graphics or GraphicsDevice will throw an exception.
            _graphics =  new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // set font colour
            _fontColour = new Color(10, 53, 83);

            // Create chunk manager
            _chunkManager = new ChunkManager(GraphicsDevice.Viewport.Bounds)
            {
                HoleSpawnChance = 0.35f,
                ObstacleSpawnChance = 0.35f
            };

            // Create camera
            _camera = new Camera(GraphicsDevice.Viewport.Bounds);
            
            // Setup highscore manager
            HighScoreManager.Initialise();

            // Create Audio Manager
            _audioManager = new AudioManager(this) { MusicVolume = 0.2f, SoundEffectVolume = 0.4f };
            Components.Add(_audioManager);

            // Create a new player object starting at X 400 and Y 200
            _player = new Player("Player", new Vector2(400, 200), 20, 38, 50, 50, 4, 0.1f);
            _player.Intialise(_audioManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("zekton free");
            _player.LoadContent(Content);
            _chunkManager.LoadContent(Content, _audioManager);
            _camera.Position = _player.Position;

            #region Audio

            // Load the audio files into the audio manager
            _audioManager.LoadContent();
            _audioManager.LoadSoundEffect("jump", "Sounds/jump");
            _audioManager.LoadSoundEffect("click", "Sounds/click");
            _audioManager.LoadSoundEffect("death", "Sounds/death");
            _audioManager.LoadSong("background", "Sounds/background");

            #endregion
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            // Check if mouse is being held so that a player does not accidently click a button when it is shown, before letting go of the mouse button
            if (_mouseIsHeld && mouseState.LeftButton == ButtonState.Released)
            {
                _mouseIsHeld = false;
            }

            IsMouseVisible = true;
            KeyboardState keyboardState = Keyboard.GetState();

            // If the player presses escape, close the game
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            switch (_currentGameState)
            {
                #region Main Menu
                case GameState.MainMenu:
                    
                    if (_playerButton == null)
                    {
                        _playerButton = new Button("play", _font, new Vector2(_camera.Left + 210, 275), _fontColour, Color.White, _audioManager);
                    }
                    if (_scoresButton == null)
                    {
                        _scoresButton = new Button("scores", _font, new Vector2(_camera.Left + 210, 325), _fontColour, Color.White, _audioManager);
                    }
                    if (_quitButton == null)
                    {
                        _quitButton = new Button("quit", _font, new Vector2(_camera.Left + 210, 375), _fontColour, Color.White, _audioManager);
                    }

                    _playerButton.Update(mouseState, _camera);
                    _scoresButton.Update(mouseState, _camera);
                    _quitButton.Update(mouseState, _camera);

                    if (_playerButton.IsClicked && !_mouseIsHeld)
                    {
                        Reset();
                        _audioManager.PlaySong("background", true);
                    }
                    if (_scoresButton.IsClicked && !_mouseIsHeld)
                    {
                        _currentGameState = GameState.Scores;
                        _scores = HighScoreManager.GetScores();
                        _mainMenuButton = new Button("main menu", _font, new Vector2(_camera.Left + 210, 275), _fontColour, Color.White, _audioManager);
                        _quitButton = new Button("quit", _font, new Vector2(_camera.Left + 210, 325), _fontColour, Color.White, _audioManager);
                        _mouseIsHeld = true;
                    }
                    if (_quitButton.IsClicked && !_mouseIsHeld)
                    {
                        Exit();
                    }
                    break;
                    #endregion

                #region Scores
                case GameState.Scores:
                    _quitButton.Update(mouseState, _camera);
                    _mainMenuButton.Update(mouseState, _camera);

                    if (_quitButton.IsClicked && !_mouseIsHeld)
                    {
                        Exit();
                    }
                    else if (_mainMenuButton.IsClicked && !_mouseIsHeld)
                    {
                        // Show main menu
                        Reset();
                        _currentGameState = GameState.MainMenu;
                        _mouseIsHeld = true;
                        _quitButton = null;
                    }
                    break;
                #endregion

                #region Playing
                case GameState.Playing:

                    IsMouseVisible = false;
                    PauseCheck();
                    // if the game is paused then do not do any updates
                    if (_gameIsPaused)
                    {
                        return;
                    }

                    // Update the chunk manager
                    _chunkManager.Update(_camera.Left, _camera.Right);


                    CollisionReason reason = _chunkManager.CheckCollision(_player.BoundingBox);

                    // if the reason is because of gravity then just move the player so they are above the chunk they collided with
                    if (reason == CollisionReason.Gravity)
                    {
                        _player.IsGrounded = true;
                        _player.Y = _chunkManager.LastIntersection.Y - _player.Height + 1;
                    }
                    // if the reason is that the player hit an obstacle or fell off a building then reset the game
                    else if (reason == CollisionReason.HitObstacle || _player.Y > 700)
                    {
                        _currentGameState = GameState.GameOver;
                        HighScoreManager.SaveScore(_score);
                        _resetButton = new Button("retry", _font, new Vector2(_camera.Left + 210, 275), _fontColour, Color.White, _audioManager);
                        _mainMenuButton = new Button("main menu", _font, new Vector2(_camera.Left + 210, 325), _fontColour, Color.White, _audioManager);
                        _audioManager.StopSong();
                        return;
                    }
                    // If the player hit a building then move them so they aren't intersecting the building and stop them travelling right
                    else if (reason == CollisionReason.HitBuilding)
                    {
                        _player.X -= _player.Width;
                        _player.VelocityX = 0;
                        _player.IsGrounded = false;
                    }
                    // if there is no collision the player must be off the ground
                    else if (reason == CollisionReason.None)
                    {
                        _player.IsGrounded = false;
                    }

                    _player.Update(gameTime);
                    _camera.Position = _player.Position;

                    _score += (int)_player.VelocityX;


                    base.Update(gameTime);
                    break;
                #endregion

                #region Paused
                case GameState.Paused:
                    PauseCheck();
                    break;
                #endregion

                #region Game Over
                case GameState.GameOver:
                    _resetButton.Update(mouseState, _camera);
                    _mainMenuButton.Update(mouseState, _camera);
                    if (_resetButton.IsClicked)
                    {
                        Reset();
                        _audioManager.PlaySong("background", true);

                    }
                    else if (_mainMenuButton.IsClicked)
                    {
                        // Show main menu
                        Reset();
                        _currentGameState = GameState.MainMenu;
                        _mouseIsHeld = true;
                    }
                    break;
                #endregion
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear the background
            GraphicsDevice.Clear(new Color(58, 159, 229));

            // Draw the game components in relation to the camera
            _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Camera.ViewMatrix);

            switch (_currentGameState)
            {
                #region Main Menu
                case GameState.MainMenu:
                    DrawTitle("jump");
                    _playerButton.Draw(_spriteBatch);
                    _scoresButton.Draw(_spriteBatch);
                    if (_quitButton != null)
                    {
                        _quitButton.Draw(_spriteBatch);
                    }

                    // Draw controls
                    _spriteBatch.DrawString(_font, "spacebar = jump", new Vector2(_camera.Left + 515, 275), _fontColour, 0,
                        Vector2.Zero, 0.9f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, "p = pause / resume", new Vector2(_camera.Left + 500, 325), _fontColour, 0,
                        Vector2.Zero, 0.9f, SpriteEffects.None, 0);
                    break;
                #endregion

                #region Scores
                case GameState.Scores:
                    DrawTitle("scores");
                    _mainMenuButton.Draw(_spriteBatch);
                    _quitButton.Draw(_spriteBatch);
                    _spriteBatch.DrawString(_font, "#1: " + (_scores.Count > 0 ? _scores[0].ToString() : "-"), new Vector2(_camera.Left + 500, 275), _fontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, "#2: " + (_scores.Count > 1 ? _scores[1].ToString() : "-"), new Vector2(_camera.Left + 500, 325), _fontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, "#3: " + (_scores.Count > 2 ? _scores[2].ToString() : "-"), new Vector2(_camera.Left + 500, 375), _fontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, "#4: " + (_scores.Count > 3 ? _scores[3].ToString() : "-"), new Vector2(_camera.Left + 700, 275), _fontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, "#5: " + (_scores.Count > 4 ? _scores[4].ToString() : "-"), new Vector2(_camera.Left + 700, 325), _fontColour, 0,
                        Vector2.Zero, 1f, SpriteEffects.None, 0);
                    break;
                #endregion

                #region Playing
                case GameState.Playing:
                    _spriteBatch.DrawString(_font, "score : " + _score, new Vector2(_camera.Left + 10, 130), _fontColour, 0,
                        Vector2.Zero, 0.85f, SpriteEffects.None, 0);
                    _player.Draw(_spriteBatch);
                    break;
                #endregion

                #region Paused
                case GameState.Paused:
                    DrawTitle("paused");
                    _spriteBatch.DrawString(_font, "score : " + _score, new Vector2(_camera.Left + 10, 130), _fontColour, 0,
                        Vector2.Zero, 0.85f, SpriteEffects.None, 0);
                    _player.Draw(_spriteBatch);
                    break;
                #endregion

                #region Game Over
                case GameState.GameOver:
                    _resetButton.Draw(_spriteBatch);
                    _mainMenuButton.Draw(_spriteBatch);
                    DrawTitle("game over");
                    _spriteBatch.DrawString(_font, "you scored:", new Vector2(_camera.Left + 600, 275), _fontColour);
                    _spriteBatch.DrawString(_font, _score.ToString(), new Vector2(_camera.Left + 600, 325), _fontColour);
                    _player.Draw(_spriteBatch);
                    break;
                #endregion
            }

            // Draw chunks
            _chunkManager.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        private void PauseCheck()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            // If the player presses p then pause or unpause the game
            // The is holding down p stops if from pausing and unpausing form one key press being read over mutliple updates
            if (keyboardState.IsKeyDown(Keys.P) && !_isHoldingDownP)
            {
                _isHoldingDownP = true;
                _gameIsPaused = !_gameIsPaused;
                _currentGameState = _gameIsPaused ? GameState.Paused : GameState.Playing;

                if (_audioManager.IsSongPlaying && _gameIsPaused)
                {
                    _audioManager.PauseSong();
                }
                if (_audioManager.IsSongPaused && !_gameIsPaused)
                {
                    _audioManager.ResumeSong();
                }
            }

            else if (!keyboardState.IsKeyDown(Keys.P))
            {
                _isHoldingDownP = false;
            }
        }

        private void Reset()
        {
            // Reset all of the game components and variables
            if (_audioManager.IsSongPlaying)
            {
                _audioManager.StopSong();
            }

            _score = 0;

            _player.Reset();
            _chunkManager.Reset();
            _camera.Reset();
            _camera.Position = _player.Position;
            _currentGameState = GameState.Playing;
        }

        private void DrawTitle(string text)
        {
            _spriteBatch.DrawString(_font, text, new Vector2(_camera.Left + 350, 150), _fontColour, 0,
                Vector2.Zero, 2f, SpriteEffects.None, 0);
        }

        #endregion
    }
}
