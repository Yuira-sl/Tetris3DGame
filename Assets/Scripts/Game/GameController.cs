﻿using Octamino.Constant;
using UnityEngine;

namespace Octamino
{
    public class GameController : MonoBehaviour
    {
        private Game _game;
        private Board _board;
        private Input _input;
        private AdvertisementController _advertisement;
        
        [SerializeField] private BoardView _boardView;
        [SerializeField] private PieceView _nextPieceView;
        [SerializeField] private GamePauseView _gamePauseView;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private HighscoreView _highscoreView;
        [SerializeField] private GameObject _screenButtons;
        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private AudioSource _musicAudioSource;

        [SerializeField] private ButtonsData _buttonsData;
        public void OnPauseButtonTap()
        {
            _game.Pause();
            ShowPauseView();
        }

        public void OnMoveDownButtonTap()
        {
            _game.MoveDown();
        }

        public void OnFallButtonTap()
        {
            _game.FallDown();
        }

        public void OnRotateLeftButtonTap()
        {
            _game.Rotate(true);
        }

        public void OnRotateRightButtonTap()
        {
            _game.Rotate(false);
        }

        private void Awake()
        {
            _advertisement = new AdvertisementController();
            HandlePlayerSettings();
            Settings.ChangedEvent += HandlePlayerSettings;
        }
        
        private void Start()
        {
            _board = new Board();
            _nextPieceView.Initialize(_board);
            _input = new Input(new KeyboardInput(), new TouchInput());
            _game = new Game(_board);

            _game.OnGameFinished += GameGameFinished;
            _game.OnPieceSettled += _audioPlayer.PlayPieceDropClip;
            _game.OnPieceRotated += _audioPlayer.PlayPieceRotateClip;
            _game.OnPieceMoved += _audioPlayer.PlayPieceMoveClip;
            _game.Start();

            _boardView.Initialize(_board);
        }

        private void OnDestroy()
        {
            Settings.ChangedEvent -= HandlePlayerSettings;
            _game.OnGameFinished -= GameGameFinished;
            _game.OnPieceSettled -= _audioPlayer.PlayPieceDropClip;
            _game.OnPieceRotated -= _audioPlayer.PlayPieceRotateClip;
            _game.OnPieceMoved -= _audioPlayer.PlayPieceMoveClip;

            _advertisement.Dispose();
        }

        private void Update()
        {
            _game.Update(Time.deltaTime);
            _input.Update();
        }

        private void GameGameFinished()
        {
            _gamePauseView.SetTitle(Text.GameFinished);
            _gamePauseView.SetSubscript(Text.LifesRemaning + _game.LifeCount);
            _gamePauseView.SetSlider(_game.LifeCount);

            if (_game.LifeCount > 0)
            {
                _gamePauseView.AddButton(_buttonsData.RollBack, ShowAds, _audioPlayer.PlayNewGameClip);
            }
            _gamePauseView.AddButton(_buttonsData.NewGame, _game.Start, _audioPlayer.PlayNewGameClip);
            _gamePauseView.AddButton(_buttonsData.ExitGame, Application.Quit, _audioPlayer.PlayNewGameClip);
            _gamePauseView.Show();
        }

        private void ShowPauseView()
        {
            _gamePauseView.SetTitle(Text.GamePaused);
            _gamePauseView.SetSubscript();
            _gamePauseView.SetSlider();
            _gamePauseView.AddButton(_buttonsData.Resume, _game.Resume, _audioPlayer.PlayResumeClip);
            _gamePauseView.AddButton(_buttonsData.NewGame, _game.Start, _audioPlayer.PlayNewGameClip);
            _gamePauseView.AddButton(_buttonsData.Settings, ShowSettingsView, _audioPlayer.PlayResumeClip);
            _gamePauseView.AddButton(_buttonsData.Highscore, ShowHighscoreView, _audioPlayer.PlayResumeClip);
            _gamePauseView.AddButton(_buttonsData.ExitGame, Application.Quit, _audioPlayer.PlayResumeClip);
            _gamePauseView.Show();
        }

        private void ShowSettingsView()
        {
            _settingsView.Show(ShowPauseView);
        }

        private void ShowHighscoreView()
        {
            _highscoreView.Show(ShowPauseView);
        }

        private void ShowAds()
        {
            var count = Game.Instance.LifeCount;
            _advertisement.ShowRewardedVideo(count);
        }

        private void HandlePlayerSettings()
        {
            _screenButtons.SetActive(true);
            _musicAudioSource.gameObject.SetActive(Settings.MusicEnabled);
        }
    }
}