using System;
using UnityEngine;

//Callbacks for input events
public delegate void HorizontalInputCallback(int direction);
public delegate void InputCallback();

public class InputController : MonoBehaviour
{
    private Rect _leftArea;
    private Rect _rightArea;

    private bool _detectSwipeAfterRelease;
    private bool _blockHorizontalInput;
    
    private Vector2 _fingerDownPos;
    private Vector2 _fingerUpPos;
    
    [SerializeField] private GameManager _manager;
    [SerializeField] private float _swipeThreshold = 20f;
    public event HorizontalInputCallback OnHorizontalInputDown;
    public event InputCallback OnSpeedDown, OnSpeedUp, OnRotateLeftDown, OnRotateRightDown, OnSwitchDown;
    public event Action OnForcedDropDown; 
    
    private void Awake()
    {
        _leftArea = new Rect(0, Screen.height * 0.2f, Screen.width / 2, Screen.height * 0.8f);
        _rightArea = new Rect(Screen.width / 2, Screen.height * 0.2f, Screen.width / 2, Screen.height  * 0.8f);
    }

    public void RotateLeft()
    {
        OnRotateLeftDown?.Invoke();
    }
    
    public void RotateRight()
    {
        OnRotateRightDown?.Invoke();
    }
    
    public void Drop()
    {
        OnForcedDropDown?.Invoke();
    }
 
    private void Update()
    {
#if UNITY_EDITOR
        EditorInput();
#endif
     
        if (!_manager.IsPaused())
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var point = touch.position;

                    _fingerUpPos = touch.position;
                    _fingerDownPos = touch.position;
                    _detectSwipeAfterRelease = false;

                    if (!_blockHorizontalInput)
                    {
                        if (_leftArea.Contains(point))
                        {
                            OnHorizontalInputDown?.Invoke(-1);
                        }

                        if (_rightArea.Contains(point))
                        {
                            OnHorizontalInputDown?.Invoke(1);
                        }
                    }
                }
                
                //Detects Swipe while finger is still moving on screen
                if (touch.phase == TouchPhase.Moved)
                {
                    if (!_detectSwipeAfterRelease)
                    {
                        _blockHorizontalInput = true;
                        _fingerDownPos = touch.position;
                        DetectSwipeDown();
                        _detectSwipeAfterRelease = true;
                    }
                }

                //Detects swipe after finger is released from screen
                if (touch.phase == TouchPhase.Ended)
                {
                    _fingerDownPos = touch.position;
                    DetectSwipeUp();
                    _blockHorizontalInput = false;
                }
            }
        }
    }

    private void EditorInput()
    {
        if (!_manager.IsPaused())
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnHorizontalInputDown?.Invoke(-1);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                OnHorizontalInputDown?.Invoke(1);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpeedDown?.Invoke();
            }
            
            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnSpeedUp?.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnRotateLeftDown?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnRotateRightDown?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnSwitchDown?.Invoke();
            }
        }
    }
    
    private void DetectSwipeDown()
    {
        if (VerticalMoveValue() > _swipeThreshold && VerticalMoveValue() > HorizontalMoveValue())
        {
            if (_fingerDownPos.y - _fingerUpPos.y < 0)
            {
                OnSpeedDown?.Invoke();
            }
            _fingerUpPos = _fingerDownPos;
        }
    }

    private void DetectSwipeUp()
    {
        if (VerticalMoveValue() > _swipeThreshold && VerticalMoveValue() > HorizontalMoveValue())
        {
            if (_fingerDownPos.y - _fingerUpPos.y < 0)
            {
                OnSpeedUp?.Invoke();
            }
            _fingerUpPos = _fingerDownPos;
        }
    }

    private float VerticalMoveValue()
    {
        return Mathf.Abs(_fingerDownPos.y - _fingerUpPos.y);
    }

    private float HorizontalMoveValue()
    {
        return Mathf.Abs(_fingerDownPos.x - _fingerUpPos.x);
    }
}
