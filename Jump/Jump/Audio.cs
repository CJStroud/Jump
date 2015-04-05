using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Jump
{
    public class AudioManager : GameComponent
    {
        #region Constructor

        public AudioManager(Game game)
            : base(game)
        {

        }

        #endregion

        #region Public Properties

        public string CurrentSong { get; set; }
        public bool IsSongPlaying { get { return _isSongPlaying; } }
        public bool IsSongPaused { get { return _isSongPaused; } }

        public float MusicVolume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }
        public float SoundEffectVolume
        {
            get { return SoundEffect.MasterVolume; }
            set { SoundEffect.MasterVolume = value; }
        }

        #endregion

        #region Private Fields

        private ContentManager _content;

        private Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>();

        private Song _currentSong;

        private bool _isSongPlaying;
        private bool _isSongPaused;

        private SoundEffect _currentSoundEffect;

        private SoundEffectInstance[] _soundEffectsBeingPlayed = new SoundEffectInstance[MaxSounds];
        private const int MaxSounds = 16;

        #endregion

        #region Overriden

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < _soundEffectsBeingPlayed.Length; i++)
            {
                if (_soundEffectsBeingPlayed[i] != null && _soundEffectsBeingPlayed[i].State == SoundState.Stopped)
                {
                    _soundEffectsBeingPlayed[i].Dispose();
                    _soundEffectsBeingPlayed[i] = null;
                }
            }

            if (_currentSong != null && MediaPlayer.State == MediaState.Stopped)
            {
                _currentSong = null;
                CurrentSong = null;
            }


            base.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public void LoadContent()
        {
            LoadContent(string.Empty);
        }

        public void LoadContent(string contentFolder)
        {
            _content = contentFolder != String.Empty
                ? new ContentManager(Game.Content.ServiceProvider, Game.Content.RootDirectory + "\\" + contentFolder)
                : new ContentManager(Game.Content.ServiceProvider, Game.Content.RootDirectory);
        }

        public void LoadSoundEffect(string soundName, string soundPath)
        {
            if (_soundEffects.ContainsKey(soundName)) return;

            _soundEffects.Add(soundName, _content.Load<SoundEffect>(soundPath));
        }

        public void PlaySoundEffect(string soundName)
        {
            SoundEffect soundEffect;
            _soundEffects.TryGetValue(soundName, out soundEffect);

            int index = GetAvailableIndex();
            if (index == -1) return;

            if (soundEffect != null)
            {
                _soundEffectsBeingPlayed[index] = soundEffect.CreateInstance();
                _soundEffectsBeingPlayed[index].Play();
            }

        }

        public void LoadSong(string songName, string songPath)
        {
            if (_soundEffects.ContainsKey(songName)) return;

            _songs.Add(songName, _content.Load<Song>(songPath));
        }

        public void PlaySong(string songName, bool isRepeating)
        {
            Song song;
            if (!_songs.TryGetValue(songName, out song) || song == null) return;

            _isSongPaused = false;
            _isSongPlaying = true;

            MediaPlayer.Stop();

            MediaPlayer.IsRepeating = isRepeating;
            MediaPlayer.Play(song);
            _currentSong = song;
            CurrentSong = songName;
        }

        public void PauseSong()
        {
            if (_isSongPlaying && !_isSongPaused)
            {
                MediaPlayer.Pause();
                _isSongPlaying = false;
                _isSongPaused = true;
            }
        }

        public void ResumeSong()
        {
            if (_isSongPaused)
            {
                MediaPlayer.Resume();
                _isSongPlaying = true;
                _isSongPaused = false;
            }
        }

        public void StopSong()
        {
            MediaPlayer.Stop();
            _currentSong = null;
            CurrentSong = null;
        }


        #endregion

        #region Private Methods

        private int GetAvailableIndex()
        {
            for (int i = 0; i < _soundEffectsBeingPlayed.Length; i++)
            {
                if (_soundEffectsBeingPlayed[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
