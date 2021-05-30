using UnityEngine;

namespace Octamino
{
    public class TouchInput : IPlayerInput
    {
        private Rect _screen;
        private float _timePressed;
        
        public TouchInput()
        {
            _screen = new Rect(0, Screen.height * 0.125f, Screen.width, Screen.height * 0.75f);
        }
        
        public void Update()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    _timePressed = 0;
                    HorizontalMove(touch.position);
                }
               
                if(touch.phase == TouchPhase.Stationary)
                {
                    _timePressed += Time.deltaTime;
                    if (_timePressed > 0.3f)
                    {
                        HorizontalMove(touch.position);
                    }
                }
            }
        }
        
        private void HorizontalMove(Vector2 position)
        {
            var normalizedPosition = position.x / Screen.width;

            if (_screen.Contains(position))
            {
                Game.Instance.MoveHorizontal(normalizedPosition);
            }
        }
    }
}