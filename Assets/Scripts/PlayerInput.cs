using System;
using UnityEngine;

//Callbacks for input events
public delegate void HorizontalInputCallback(int direction);
public delegate void InputCallback();

public class PlayerInput : MonoBehaviour
{
    private readonly Rect _leftArea;
    private readonly Rect _rightArea;

    [SerializeField] private RectTransform _dropDownButton;
    [SerializeField] private RectTransform _rotateLeftButton;
    [SerializeField] private RectTransform _rotateRightButton;
    
    public static PlayerInput Instance;
    
    public event HorizontalInputCallback OnHorizontalInputDown;
    public event InputCallback OnSpeedDown, OnSpeedUp, OnRotateLeftDown, OnRotateRightDown, OnSwitchDown;
    public event Action OnForcedDropDown; 
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
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
#endif
        
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var point = touch.position;
            
                if (_leftArea.IsTouchIntersectsButtons(point, _dropDownButton, _rotateLeftButton, _rotateRightButton))
                {
                    OnHorizontalInputDown?.Invoke(-1);
                }
            
                if (_rightArea.IsTouchIntersectsButtons(point, _dropDownButton, _rotateLeftButton, _rotateRightButton))
                {
                    OnHorizontalInputDown?.Invoke(1);
                }
            }
        }
    }

}
