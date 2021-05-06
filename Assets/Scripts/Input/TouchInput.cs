using UnityEngine;

namespace Octamino
{
    public class TouchInput : IPlayerInput
    {
        private Board _board;
        
        private Vector2 _initialPosition = Vector2.zero;
        private Vector2 _processedOffset = Vector2.zero;
        private PlayerAction? _playerAction;
        private bool _moveDownDetected;
        private float _touchBeginTime;
        private readonly float _tapMaxDuration = 0.25f;
        private readonly float _tapMaxOffset = 30.0f;
        private readonly float _swipeMaxDuration = 0.3f;
        private bool _cancelCurrentTouch;
        private bool _enabled = true;

        private Rect _leftArea;
        private Rect _rightArea;
        
        public float BlockSize;
        
        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _cancelCurrentTouch = false;
                _playerAction = null;
            }
        }

        public TouchInput()
        {
            _leftArea = new Rect(0, Screen.height * 0.2f, Screen.width / 2, Screen.height * 0.8f);
            _rightArea = new Rect(Screen.width / 2, Screen.height * 0.2f, Screen.width / 2, Screen.height  * 0.8f);
        }

        public void Initialize(Board board)
        {
            _board = board;
        }
        
        public void Update()
        {
            _playerAction = null;

            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                
                if (_cancelCurrentTouch)
                {
                    _cancelCurrentTouch &= touch.phase != TouchPhase.Ended;
                }
                else if (touch.phase == TouchPhase.Began)
                {
                    TouchBegan(touch);
                }
                // else if (touch.phase == TouchPhase.Moved)
                // {
                //     var offset = touch.position - _initialPosition - _processedOffset;
                //     HandleMove(touch, offset);
                // }
                else if (touch.phase == TouchPhase.Ended)
                {
                    var touchDuration = Time.time - _touchBeginTime;
                    var offset = (touch.position - _initialPosition).magnitude;
                    
                    if (_leftArea.Contains(touch.position))
                    {
                        _playerAction = PlayerAction.MoveLeft;
                        _board.MovePieceLeft();
                    }
                    
                    if (_rightArea.Contains(touch.position))
                    {
                        _playerAction = PlayerAction.MoveRight;
                        _board.MovePieceRight();
                    }
                    
                    if (touchDuration < _tapMaxDuration && offset < _tapMaxOffset)
                    {
                        _playerAction = PlayerAction.RotateRight;
                    }
                    else if (_moveDownDetected && touchDuration < _swipeMaxDuration)
                    {
                        _playerAction = PlayerAction.Fall;
                    }
                }
            }
            else
            {
                _cancelCurrentTouch = false;
            }
        }

        public PlayerAction? GetPlayerAction()
        {
            return Enabled ? _playerAction : null;
        }

        public void Cancel()
        {
            _cancelCurrentTouch |= Input.touchCount > 0;
        }

        private void TouchBegan(Touch touch)
        {
            _initialPosition = touch.position;
            _processedOffset = Vector2.zero;
            _moveDownDetected = false;
            _touchBeginTime = Time.time;
        }

        private void HandleMove(Touch touch, Vector2 offset)
        {
            HandleHorizontalMove(touch, offset.x);
            _playerAction = ActionForHorizontalMoveOffset(offset.x);
            
            // if (Mathf.Abs(offset.x) >= BlockSize)
            // {
            //     HandleHorizontalMove(touch, offset.x);
            //     _playerAction = ActionForHorizontalMoveOffset(offset.x);
            // }

            if (offset.y <= -BlockSize)
            {
                HandleVerticalMove(touch);
                _playerAction = PlayerAction.MoveDown;
            }
        }

        private void HandleHorizontalMove(Touch touch, float offset)
        {
            _processedOffset.x += Mathf.Sign(offset) * BlockSize;
            _processedOffset.y = (touch.position - _initialPosition).y;
        }

        private void HandleVerticalMove(Touch touch)
        {
            _moveDownDetected = true;
            _processedOffset.y -= BlockSize;
            _processedOffset.x = (touch.position - _initialPosition).x;
        }

        private PlayerAction ActionForHorizontalMoveOffset(float offset)
        {
            Debug.Log(offset);
            return offset > 0 ? PlayerAction.MoveRight : PlayerAction.MoveLeft;
        }
    }
}