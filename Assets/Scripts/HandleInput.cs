using System;
using UnityEngine;

public class HandleInput
{
    private readonly GameManager _manager;
    private readonly Settings _settings;
    
    public HandleInput(GameManager manager, Settings settings)
    {
        _manager = manager;
        _settings = settings;
    }
    
    public void Update(Func<PieceController, bool> checkPosition, Action hide, Action draw)
    {
        UpdateUI();
        UpdateControls(checkPosition, hide, draw);
        UpdateCamera();
    }

    private void UpdateUI()
    {
        if (_manager.CurrentMenu != _manager.MainMenu)
        {
            if (Input.GetKeyDown(_settings.PauseBtn))
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
                }
                else
                {
                    _manager.PreviousCanvas();
                }
            }
        }
    }
    private void UpdateControls(Func<PieceController, bool> checkPosition, Action hide, Action draw)
    {
        if (!_settings.Clearing && !_settings.Paused)
        {
            //Sets the keys to be relative to the orientation of the camera
            KeyCode relativeLeft = _settings.LeftBtn;
            KeyCode relativeRight = _settings.RightBtn;
            KeyCode relativeForward = _settings.ForwardBtn;
            KeyCode relativeBackward = _settings.BackwardBtn;
            
            if ((_settings.CameraAngleXZ >= 0f && _settings.CameraAngleXZ < 45f) || (_settings.CameraAngleXZ >= 315f && _settings.CameraAngleXZ <= 360f))
            {
                //Debug.Log("Primed for relative control in Quadrant 1");
                relativeLeft = _settings.LeftBtn;
                relativeRight = _settings.RightBtn;
                relativeForward = _settings.ForwardBtn;
                relativeBackward = _settings.BackwardBtn;
            }
            else if (_settings.CameraAngleXZ >= 45f && _settings.CameraAngleXZ < 135f)
            {
                //Debug.Log("Primed for relative control in Quadrant 2");
                relativeLeft = _settings.BackwardBtn;
                relativeRight = _settings.ForwardBtn;
                relativeForward = _settings.LeftBtn;
                relativeBackward = _settings.RightBtn;
            }
            else if (_settings.CameraAngleXZ >= 135f && _settings.CameraAngleXZ < 225f)
            {
                //Debug.Log("Primed for relative control in Quadrant 3");
                relativeLeft = _settings.RightBtn;
                relativeRight = _settings.LeftBtn;
                relativeForward = _settings.BackwardBtn;
                relativeBackward = _settings.ForwardBtn;
            }
            else if (_settings.CameraAngleXZ >= 225f && _settings.CameraAngleXZ < 315f)
            {
                //Debug.Log("Primed for relative control in Quadrant 4");
                relativeLeft = _settings.ForwardBtn;
                relativeRight = _settings.BackwardBtn;
                relativeForward = _settings.RightBtn;
                relativeBackward = _settings.LeftBtn;
            }
            else
            {
                Debug.Log("Tracing the value of CameraAngle XZ is: " + _settings.CameraAngleXZ);
                Debug.Log("No conditions met, this shouldn't be happening.");
            }

            if (Input.GetKeyDown(relativeLeft))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.MoveX(-1);
                if (checkPosition(testPieceController))
                {
                    MoveX(_settings,-1, hide, draw);
                }
            }

            if (Input.GetKeyDown(relativeRight))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.MoveX(1);
                if (checkPosition(testPieceController))
                {
                    MoveX(_settings,1, hide, draw);
                }
            }

            if (Input.GetKeyDown(relativeBackward))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.MoveZ(-1);
                if (checkPosition(testPieceController))
                {
                    MoveZ(_settings,-1, hide, draw);
                }
            }

            if (Input.GetKeyDown(relativeForward))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.MoveZ(1);
                if (checkPosition(testPieceController))
                {
                    MoveZ(_settings,1, hide, draw);
                }
            }

            if (Input.GetKeyDown(_settings.RotateXZBtn))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.RotateXZ(!_settings.XZClockwise);
                if (checkPosition(testPieceController))
                {
                    RotateXZ(_settings,hide, draw);
                }
                else
                {
                    testPieceController = _settings.CurrentPieceController.Clone();
                    testPieceController.RotateXZ(_settings.XZClockwise);
                    if (checkPosition(testPieceController))
                    {
                        _settings.XZClockwise = !_settings.XZClockwise;
                        RotateXZ(_settings,hide, draw);
                    }
                }
            }

            if (Input.GetKeyDown(_settings.RotateXYBtn))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.RotateXY(!_settings.XYClockwise);
                if (checkPosition(testPieceController))
                {
                    RotateXY(_settings,hide, draw);
                }
                else
                {
                    testPieceController = _settings.CurrentPieceController.Clone();
                    testPieceController.RotateXY(_settings.XYClockwise);
                    if (checkPosition(testPieceController))
                    {
                        _settings.XYClockwise = !_settings.XYClockwise;
                        RotateXY(_settings,hide, draw);
                    }
                }
            }

            if (Input.GetKeyDown(_settings.RotateYZBtn))
            {
                PieceController testPieceController = _settings.CurrentPieceController.Clone();
                testPieceController.RotateYZ(!_settings.YZClockwise);
                if (checkPosition(testPieceController))
                {
                    RotateYZ(_settings,hide, draw);
                }
                else
                {
                    testPieceController = _settings.CurrentPieceController.Clone();
                    testPieceController.RotateYZ(_settings.YZClockwise);
                    if (checkPosition(testPieceController))
                    {
                        _settings.YZClockwise = !_settings.YZClockwise;
                        RotateYZ(_settings,hide, draw);
                    }
                }
            }
        }
        
        if (Input.GetKeyDown(_settings.DropBtn))
        {
            _settings.CurrentDropTime = 0.1f;
        }

        if (Input.GetKeyUp(_settings.DropBtn))
        {
            _settings.CurrentDropTime = _settings.DropTimeDefault;
        }
    }
    private void UpdateCamera()
    {
        if (!_settings.Paused)
        {
            if (Input.GetKey(_settings.RotateCamera))
            {
                _settings.CameraAngleXZ += 20 * Input.GetAxisRaw("Mouse X") * _settings.RotationSpeed;
                if (_settings.CameraAngleXZ > 360f)
                {
                    _settings.CameraAngleXZ -= 360f;
                }
                else if (_settings.CameraAngleXZ < 0f)
                {
                    _settings.CameraAngleXZ += 360f;
                }

                if (_settings.CameraAngleXY + (20 * Input.GetAxisRaw("Mouse Y") * _settings.RotationSpeed) < 200 &&
                    _settings.CameraAngleXY + (20 * Input.GetAxisRaw("Mouse Y") * _settings.RotationSpeed) >= 90f)
                {
                    _settings.CameraAngleXY += 20 * Input.GetAxisRaw("Mouse Y") * _settings.RotationSpeed;
                }
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f)
            {
                if (_settings.CameraDistance + _settings.ZoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel") > _settings.CameraDistanceMinMax.x &&
                    _settings.CameraDistance + _settings.ZoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel") < _settings.CameraDistanceMinMax.y)
                {
                    _settings.CameraDistance += _settings.ZoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel");
                }
            }
        }
    }
    private void RotateXZ(Settings settings, Action hide, Action draw)
    {
        hide();
        settings.CurrentPieceController.RotateXZ(!settings.XZClockwise);
        draw();
    }
    private void RotateXY(Settings settings, Action hide, Action draw)
    {
        hide();
        settings.CurrentPieceController.RotateXY(!settings.XYClockwise);
        draw();
    }
    private void RotateYZ(Settings settings, Action hide, Action draw)
    {
        hide();
        settings.CurrentPieceController.RotateYZ(!settings.YZClockwise);
        draw();
    }
    private void MoveX(Settings settings, int input, Action hide, Action draw)
    {
        hide();
        settings.CurrentPieceController.MoveX(input);
        draw();
    }
    private void MoveZ(Settings settings, int input, Action hide, Action draw)
    {
        hide();
        settings.CurrentPieceController.MoveZ(input);
        draw();
    }
}