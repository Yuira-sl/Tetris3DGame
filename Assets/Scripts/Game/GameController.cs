using UnityEngine;
using Octamino;

public class GameController : MonoBehaviour
{
    private Game _game;
    private UniversalInput _universalInput;

    [SerializeField] private BoardView _boardView;
    [SerializeField] private PieceView _nextPieceView;
    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private LevelView _levelView;
    [SerializeField] private EndGameView _endGameView;
    [SerializeField] private SettingsView _settingsView;
    [SerializeField] private GameObject _screenButtons;
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private AudioSource _musicAudioSource;
    
    public void OnPauseButtonTap()
    {
        _game.Pause();
        ShowPauseView();
    }

    public void OnMoveLeftButtonTap()
    {
        _game.SetNextAction(PlayerAction.MoveLeft);
    }

    public void OnMoveRightButtonTap()
    {
        _game.SetNextAction(PlayerAction.MoveRight);
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
        HandlePlayerSettings();
        Settings.ChangedEvent += HandlePlayerSettings;
    }

    private void Start()
    {
        Board board = new Board(10, 20);
        
        _boardView.SetBoard(board);
        _nextPieceView.SetBoard(board);

        _universalInput = new UniversalInput(new KeyboardInput(), _boardView.TouchInput);

        _game = new Game(board, _universalInput);
        _game.OnGameFinished += GameGameFinished;
        _game.OnPieceFinishedFalling += _audioPlayer.PlayPieceDropClip;
        _game.OnPieceRotated += _audioPlayer.PlayPieceRotateClip;
        _game.OnPieceMoved += _audioPlayer.PlayPieceMoveClip;
        _game.Start();

        _scoreView.Game = _game;
        _levelView.Game = _game;
    }
    
    private void GameGameFinished()
    {
        _endGameView.SetTitle(Octamino.Constant.Text.GameFinished);
        _endGameView.AddButton(Octamino.Constant.Text.PlayAgain, _game.Start, _audioPlayer.PlayNewGameClip);
        _endGameView.Show();
    }

    private void Update()
    {
        _game.Update(Time.deltaTime);
    }

    private void ShowPauseView()
    {
        _endGameView.SetTitle(Octamino.Constant.Text.GamePaused);
        _endGameView.AddButton(Octamino.Constant.Text.Resume, _game.Resume, _audioPlayer.PlayResumeClip);
        _endGameView.AddButton(Octamino.Constant.Text.NewGame, _game.Start, _audioPlayer.PlayNewGameClip);
        _endGameView.AddButton(Octamino.Constant.Text.Settings, ShowSettingsView, _audioPlayer.PlayResumeClip);
        _endGameView.Show();
    }

    private void ShowSettingsView()
    {
        _settingsView.Show(ShowPauseView);
    }

    private void HandlePlayerSettings()
    {
        _screenButtons.SetActive(true);
        //_boardView.TouchInput.Enabled = !Settings.ScreenButonsEnabled;
        _musicAudioSource.gameObject.SetActive(Settings.MusicEnabled);
    }
}
