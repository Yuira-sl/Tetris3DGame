using UnityEngine;

public class InputController
{
    private readonly GameManager _manager;
    private readonly Settings _settings;
    private readonly Rect _leftArea;
    private readonly Rect _rightArea;
    
    public InputController(GameManager manager, Settings settings)
    {
        _manager = manager;
        _settings = settings;
        _leftArea = new Rect(0, 0, Screen.width / 2, Screen.height);
        _rightArea = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height);
    }

    public void Update()
    {
        if (!_settings.Paused && !_settings.Clearing)
        {
            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     MoveLeft();
            // }
            // if (Input.GetKeyDown(KeyCode.D))
            // {
            //     MoveRight();
            // }
            
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var point = touch.position;
            
                    if (_leftArea.IsTouchIntersectsButtons(point, _manager.DropDownBtn, _manager.RotateBtn,
                        _manager.FlipBtn))
                    {
                        MoveLeft();
                    }
            
                    if (_rightArea.IsTouchIntersectsButtons(point, _manager.DropDownBtn, _manager.RotateBtn,
                        _manager.FlipBtn))
                    {
                        MoveRight();
                    }
                }
            }
        }

        //Handles the automatic progression of the game, when not paused or clearing
        if (!_settings.Paused && !_settings.Clearing)
        {
            _settings.DropClock += Time.deltaTime;

            //Drops the block down the vertical axis
            if (_settings.DropClock > _settings.CurrentDropTime)
            {
                _settings.DropClock = 0f;
                var piece = _settings.CurrentPieceController.Clone();
                piece.MoveY(-1);
                if (_manager.CheckPosition(piece))
                {
                    MoveY(-1);
                }
                else
                {
                    _manager.PlacePiece();
                }
            }

            _settings.DifficultyClock += Time.deltaTime;

            //Speeds up block dropping as the game goes on
            if (_settings.DifficultyClock > _settings.DifficultyTime)
            {
                _settings.DifficultyClock = 0f;
                if (_settings.DropTimeDefault * 0.95f > _settings.DropTimeMinMax.x)
                {
                    _settings.DropTimeDefault *= 0.95f;
                    _settings.ClearTime = _settings.DropTimeDefault;
                }
            }
        }
    }

    public void RotateOnXY()
    {
        if (!_settings.Clearing && !_settings.Paused)
        {
            var piece = _settings.CurrentPieceController.Clone();
            piece.RotateXY();
            if (_manager.CheckPosition(piece))
            {
                _manager.HidePiece();
                _settings.CurrentPieceController.RotateXY();
                _manager.DrawPiece();
            }
        }
    }

    public void FlipVertical()
    {
        if (!_settings.Clearing && !_settings.Paused)
        {
            var piece = _settings.CurrentPieceController.Clone();
            piece.FlipVertically();
            if (_manager.CheckPosition(piece))
            {
                _manager.HidePiece();
                _settings.CurrentPieceController.FlipVertically();
                _manager.DrawPiece();
            }
        }
    }

    public void DropDown()
    {
        var piece = _settings.CurrentPieceController.Clone();

        do
        {
            piece.MoveY(-1);
            if (_manager.CheckPosition(piece))
            {
                MoveY(-1);
            }
            else
            {
                _manager.PlacePiece();
            }
        } while (_manager.CheckPosition(piece));
    }
    
    public void PauseMenu()
    {
        if (_manager.CurrentMenu != _manager.MainMenu)
        {
            if (_manager.CurrentMenu == _manager.InGameUI)
            {
                _manager.Pause();
            }
            else if (_manager.CurrentMenu == _manager.PauseMenu)
            {
                _manager.Resume();
            }
            else if (_manager.CurrentMenu == _manager.GameOverMenu)
            {
                // Do something
            }
            else
            {
                _manager.PreviousCanvas();
            }
        }
    }

    private void MoveLeft()
    {
        var piece = _settings.CurrentPieceController.Clone();
        piece.MoveX(-1);
        if (_manager.CheckPosition(piece))
        {
            MoveX(-1);
        }
    }

    private void MoveRight()
    {
        var piece = _settings.CurrentPieceController.Clone();
        piece.MoveX(1);
        if (_manager.CheckPosition(piece))
        {
            MoveX(1);
        }
    }

    private void MoveX(int input)
    {
        _manager.HidePiece();
        _settings.CurrentPieceController.MoveX(input);
        _manager.DrawPiece();
    }

    private void MoveY(int input)
    {
        _manager.HidePiece();
        _settings.CurrentPieceController.MoveY(input);
        _manager.DrawPiece();
    }
}