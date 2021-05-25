namespace Octamino
{
    public class Game
    {
        public static Game Instance;
        
        private readonly Board _board;
        private readonly IPlayerInput _input;
        private BoardView _boardView;
        private PlayerAction? _nextAction;
        private float _elapsedTime;
        private bool _isPlaying;
        
        public delegate void GameEventHandler();
        
        public event GameEventHandler OnResumed = delegate { };
        public event GameEventHandler OnPaused = delegate { };
        public event GameEventHandler OnGameFinished = delegate { };
        public event GameEventHandler OnPieceMoved = delegate { };
        public event GameEventHandler OnPieceRotated = delegate { };
        public event GameEventHandler OnPieceSettled = delegate { };
        
        public Score Score { get; private set; }
        public Level Level { get; private set; }
        public int LifeCount { get; set; }

        
        public Game(Board board, IPlayerInput input)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            
            _board = board;
            _input = input;
        }

        public void SetBoard(BoardView boardView)
        {
            _boardView = boardView;
        }
        
        public void Start()
        {
            _isPlaying = true;
            OnResumed();
            _elapsedTime = 0;
            Score = new Score();
            Level = new Level();
            LifeCount = 3;
            _board.RemoveAllBlocks();
            AddPiece();
        }
        
        public void Resume()
        {
            _isPlaying = true;
            OnResumed();
        }
        
        public void RollBack()
        {
            _boardView.StartCoroutine(_board.RemoveLastRows(10, 0.5f));
        }
        
        public void Pause()
        {
            _isPlaying = false;
            OnPaused();
        }
        
        public void SetNextAction(PlayerAction action)
        {
            _nextAction = action;
        }
        
        public void Update(float deltaTime)
        {
            if (!_isPlaying)
            {
                return;
            }
            
            var action = _input?.GetPlayerAction();
            if (action.HasValue)
            {
                HandlePlayerAction(action.Value);
            }
            else if (_nextAction.HasValue)
            {
                HandlePlayerAction(_nextAction.Value);
                _nextAction = null;
            }
            else
            {
                HandleAutomaticPieceFalling(deltaTime);
            }
        }
        
        private void AddPiece()
        {
            _board.AddPiece();
            if (_board.HasCollisions())
            {
                _isPlaying = false;
                OnPaused();
                LifeCount--;
                OnGameFinished();
            }
        }
        
        private void HandleAutomaticPieceFalling(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= Level.FallDelay)
            {
                if (!_board.MovePieceDown())
                {
                    PieceSettled();
                }
                ResetElapsedTime();
            }
        }

        private void HandlePlayerAction(PlayerAction action)
        {
            var pieceMoved = false;
            switch (action)
            {
                case PlayerAction.MoveLeft:
                    pieceMoved = _board.MovePieceLeft();
                    break;

                case PlayerAction.MoveRight:
                    pieceMoved = _board.MovePieceRight();
                    break;

                case PlayerAction.MoveDown:
                    ResetElapsedTime();
                    if (_board.MovePieceDown())
                    {
                        pieceMoved = true;
                        Score.PieceMovedDown();
                    }
                    else
                    {
                        PieceSettled();
                    }
                    break;

                case PlayerAction.RotateRight:
                    var didRightRotate = _board.RotatePiece(false);
                    if (didRightRotate)
                    {
                        OnPieceRotated();
                    }
                    break;
                
                case PlayerAction.RotateLeft:
                    var didLeftRotate = _board.RotatePiece(true);
                    if (didLeftRotate)
                    {
                        OnPieceRotated();
                    }
                    break;

                case PlayerAction.Fall:
                    Score.PieceFinishedFalling(_board.FallPiece());
                    ResetElapsedTime();
                    PieceSettled();
                    break;
            }
            if (pieceMoved)
            {
                OnPieceMoved();
            }
        }

        private void PieceSettled()
        {
            OnPieceSettled();
            _boardView.StartCoroutine(_board.RemoveFullRows(0.3f));
            var rowsRemoved = _board.RowsRemoved; 
            Score.RowsCleared(rowsRemoved); 
            Level.RowsCleared(rowsRemoved);
            AddPiece();
        }
        
        private void ResetElapsedTime()
        {
            _elapsedTime = 0;
        }
    }
}
