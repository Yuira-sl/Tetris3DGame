using System;
using UnityEngine;

//Callbacks for input events
public delegate void HorizontalInputCallback(int direction);
public delegate void InputCallback();

public class InputController : MonoBehaviour
{
    private Rect _leftArea;
    private Rect _rightArea;
    
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

    public void SpeedUp()
    {
        OnSpeedUp?.Invoke();
    }

    public void SpeedDown()
    {
        OnSpeedDown?.Invoke();
    }
    
    public void DropDown()
    {
        OnForcedDropDown?.Invoke();
    }
 
    private void Update()
    {
#if UNITY_EDITOR
        EditorInput();
#endif
     
        if (!GameManager.Instance.IsPaused())
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var point = touch.position;
                    
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
        }
    }

    private void EditorInput()
    {
        if (!GameManager.Instance.IsPaused())
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
}
