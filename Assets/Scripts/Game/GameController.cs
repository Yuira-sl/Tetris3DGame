using Octamino.Constant;
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
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private LevelView _levelView;
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
            _game.SetNextAction(PlayerAction.MoveDown);
        }

        public void OnFallButtonTap()
        {
            _game.SetNextAction(PlayerAction.Fall);
        }

        public void OnRotateLeftButtonTap()
        {
            _game.SetNextAction(PlayerAction.RotateLeft);
        }

        public void OnRotateRightButtonTap()
        {
            _game.SetNextAction(PlayerAction.RotateRight);
        }

        private void Awake()
        {
            _advertisement = new AdvertisementController();
            HandlePlayerSettings();
            Settings.ChangedEvent += HandlePlayerSettings;
        }

        private void Start()
        {
            _board = new Board(10, 20);
            _nextPieceView.SetBoard(_board);
            _input = new Input(new KeyboardInput(), _boardView.TouchInput);
            _game = new Game(_board, _input);

            _game.OnGameFinished += GameGameFinished;
            _game.OnPieceSettled += _audioPlayer.PlayPieceDropClip;
            _game.OnPieceRotated += _audioPlayer.PlayPieceRotateClip;
            _game.OnPieceMoved += _audioPlayer.PlayPieceMoveClip;
            _game.Start();

            _boardView.SetBoard(_board);
            _game.SetBoard(_boardView);

            _scoreView.Game = _game;
            _levelView.Game = _game;
        }

        private void Update()
        {
            _game.Update(Time.deltaTime);
        }

        private void GameGameFinished()
        {
            _gamePauseView.SetTitle(Text.GameFinished);
            _gamePauseView.SetSubscript(Text.LifesRemaning + _game.LifeCount);
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