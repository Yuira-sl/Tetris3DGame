using System;
using UnityEngine;

namespace Octamino
{
    public class Game
    {
        private readonly Board _board;
        private readonly IPlayerInput _input;
        private PlayerAction? _nextAction;
        private float _elapsedTime;
        private bool _isPlaying;
        
        public delegate void GameEventHandler();
        
        public event GameEventHandler OnResumed = delegate { };
        public event GameEventHandler OnPaused = delegate { };
        public event GameEventHandler OnGameFinished = delegate { };
        public event GameEventHandler OnPieceMoved = delegate { };
        public event GameEventHandler OnPieceRotated = delegate { };
        public event GameEventHandler OnPieceFinishedFalling = delegate { };
        
        public event Action<Board> OnPieceStartedFalling;

        
        public Score Score { get; private set; }
        public Level Level { get; private set; }
        
        public Game(Board board, IPlayerInput input)
        {
            _board = board;
            _input = input;
            OnPieceFinishedFalling += input.Cancel;
            OnPieceStartedFalling += OnNewPiece;
        }
        
        public void Start()
        {
            _isPlaying = true;
            OnResumed();
            _elapsedTime = 0;
            Score = new Score();
            Level = new Level();
            _board.RemoveAllBlocks();
            AddPiece();
        }
        
        public void Resume()
        {
            _isPlaying = true;
            OnResumed();
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
            if (!_isPlaying) return;

            _input.Update();

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
            OnPieceStartedFalling?.Invoke(_board);
            if (_board.HasCollisions())
            {
                _isPlaying = false;
                OnPaused();
                OnGameFinished();
            }
        }

        private void OnNewPiece(Board board)
        {
            Debug.Log(board.Piece.Type);
        }
        
        private void HandleAutomaticPieceFalling(float deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= Level.FallDelay)
            {
                if (!_board.MovePieceDown())
                {
                    PieceFinishedFalling();
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
                        PieceFinishedFalling();
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
                    PieceFinishedFalling();
                    break;
            }
            if (pieceMoved)
            {
                OnPieceMoved();
            }
        }

        private void PieceFinishedFalling()
        {
            OnPieceFinishedFalling();
            int rowsCount = _board.RemoveFullRows();
            Score.RowsCleared(rowsCount);
            Level.RowsCleared(rowsCount);
            AddPiece();
        }

        private void ResetElapsedTime()
        {
            _elapsedTime = 0;
        }
    }
}
