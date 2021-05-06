using UnityEngine;

namespace Octamino
{
    public class TouchInput : IPlayerInput
    {
        private PlayerAction? _playerAction;
        private bool _cancelCurrentTouch;
        private bool _enabled = true;

        private Rect _leftScreenPart;
        private Rect _rightScreenPart;
        
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
            _leftScreenPart = new Rect(0, Screen.height * 0.15f, Screen.width / 2, Screen.height * 0.85f);
            _rightScreenPart = new Rect(Screen.width / 2, Screen.height * 0.15f, Screen.width / 2, Screen.height  * 0.85f);
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
                else if (touch.phase == TouchPhase.Ended)
                {
                    _playerAction = ActionForHorizontalMoveOffset(touch.position);
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
        
        private PlayerAction? ActionForHorizontalMoveOffset(Vector2 position)
        {
            if (_leftScreenPart.Contains(position))
            {
                _playerAction = PlayerAction.MoveLeft;
            }
                    
            if (_rightScreenPart.Contains(position))
            {
                _playerAction = PlayerAction.MoveRight;
            }
            
            return _playerAction;
        }
    }
}