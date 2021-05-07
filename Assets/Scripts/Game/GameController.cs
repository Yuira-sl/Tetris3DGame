using UnityEngine;
using Octamino;

public class GameController : MonoBehaviour
{
    private Game _game;
    private Board _board;
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Gizmos.DrawWireCube(new Vector3(j,i,1), Vector3.one);
            }
        }
    }
#endif
    private void Start()
    {
        _board = new Board(10, 20);

        _nextPieceView.SetBoard(_board);

        _universalInput = new UniversalInput(new KeyboardInput(), _boardView.TouchInput);

        _game = new Game(_board, _universalInput);
        _game.OnGameFinished += GameGameFinished;
        _game.OnPieceSettled += _audioPlayer.PlayPieceDropClip;
        _game.OnPieceRotated += _audioPlayer.PlayPieceRotateClip;
        _game.OnPieceMoved += _audioPlayer.PlayPieceMoveClip;
        _game.Start();
        
        _board.SetGame(_game);
        _boardView.SetBoard(_board);
        _game.SetBoard(_boardView);
        
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
        _musicAudioSource.gameObject.SetActive(Settings.MusicEnabled);
    }
}
