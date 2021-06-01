namespace Octamino
{
    public class Game
    {
        public static Game Instance;
        
        private readonly Board _board;
        private float _elapsedTime;
        private bool _isPlaying;
        private bool _canMove;
        
        public delegate void GameEventHandler();
        
        public event GameEventHandler OnResumed = delegate { };
        public event GameEventHandler OnPaused = delegate { };
        public event GameEventHandler OnNewPiece = delegate { };
        public event GameEventHandler OnGameFinished = delegate { };
        public event GameEventHandler OnPieceMoved = delegate { };
        public event GameEventHandler OnPieceRotated = delegate { };
        public event GameEventHandler OnPieceSettled = delegate { };

        public bool CanMove
        {
            get => _canMove;
            set => _canMove = value;
        }

        public bool IsPlaying => _isPlaying;
        public Score Score { get; private set; }
        public Level Level { get; private set; }
        public int LifeCount { get; set; }

        
        public Game(Board board)
        {
            if (Instance == null)
            {
                Instance = this;
            }

            _canMove = true;
            _board = board;
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
            _board.RemoveLastRows(10, 0.5f);
        }
        
        public void Pause()
        {
            _isPlaying = false;
            OnPaused();
        }
        
        public void MoveHorizontal(float pos)
        {
            if (!_canMove)
            {
                return;
            }
            
            var pieceMoved = pos > 0.5 ? _board.MovePieceRight() : _board.MovePieceLeft();
            if (pieceMoved)
            {
                OnPieceMoved();
            }
        }

        public void MoveDown()
        {
            if (!_canMove)
            {
                return;
            }
            
            bool pieceMoved = false;
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
            if (pieceMoved)
            {
                OnPieceMoved();
            }
        }

        public void FallDown()
        {
            if (!_canMove)
            {
                return;
            }
            
            Score.PieceFinishedFalling(_board.FallPiece());
            ResetElapsedTime();
            PieceSettled();
        }
        
        public void Rotate(bool counterclockwise)
        {
            if (!_canMove)
            {
                return;
            }
            
            var isRotate = _board.RotatePiece(counterclockwise);
            if (isRotate)
            {
                OnPieceRotated();
            }
        }
        
        public void Update(float deltaTime)
        {
            if (!_isPlaying)
            {
                return;
            }
            
            PieceFalling(deltaTime);
        }
        
        private void AddPiece()
        {
            OnNewPiece();
            _board.AddPiece();
            if (_board.HasCollisions())
            {
                _isPlaying = false;
                OnPaused();
                LifeCount--;
                OnGameFinished();
            }
        }
        
        private void PieceFalling(float deltaTime)
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
        
        private void PieceSettled()
        {
            OnPieceSettled();
            _board.RemoveFullRows(0.35f);
            AddPiece();
        }
        
        private void ResetElapsedTime()
        {
            _elapsedTime = 0;
        }
    }
}
