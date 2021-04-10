using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Assets")] [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _block;
    [SerializeField] private GameObject _ghostBlock;

    [SerializeField] private List<Material> _blockMaterials = new List<Material>();
    private Material _currentMaterial;

    [SerializeField] private GameObject _basePlateSource;
    [SerializeField] private ParticleSystem _clearEffectSource;
    private ParticleSystem _clearEffect;

    [Header("UI")] [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _inGameUi;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _gameOverMenu;
    private GameObject _currentMenu;

    [Header("Grid Size")] [SerializeField] private Vector3Int _grid = new Vector3Int(5, 8, 5);

    [Header("Controls")] [SerializeField] private KeyCode _forwardBtn = KeyCode.W;
    [SerializeField] private KeyCode _backwardBtn = KeyCode.S;
    [SerializeField] private KeyCode _leftBtn = KeyCode.A;
    [SerializeField] private KeyCode _rightBtn = KeyCode.D;
    [SerializeField] private KeyCode _rotateXZBtn = KeyCode.Alpha1;
    [SerializeField] private KeyCode _rotateXYBtn = KeyCode.Alpha2;
    [SerializeField] private KeyCode _rotateYZBtn = KeyCode.Alpha3;
    [SerializeField] private KeyCode _dropBtn = KeyCode.Space;
    [SerializeField] private KeyCode _pauseBtn = KeyCode.Escape;
    [SerializeField] private KeyCode _rotateCamera = KeyCode.Mouse0;

    private bool[,,] _isCellFilled;
    private PieceController _currentPieceController;

    /// <summary>
    /// Array containing all the GameObjects associated with the game board.
    /// </summary>
    private GameObject[,,] _tileLiterals;

    private GameObject[,,] _ghostLiterals;

    private GameObject _basePlate;
    private GameObject _piecesRoot;

    //Designates where new pieces will spawn in
    private Vector3Int _pointer = new Vector3Int(2, 7, 2);

    //Variables for controlling the rotation direction of the current game piece
    private bool _XZClockwise;
    private bool _XYClockwise;
    private bool _YZClockwise;

    //Variables related to camera control
    private float _rotationSpeed = 1f;
    private float _rotationSpeedMin = 0.5f;
    private float _rotationSpeedMax = 2f;
    private float _cameraDistance = 7f;
    private float _cameraDistanceMax = 18f;
    private float _cameraDistanceMin = 6f;
    private float _zoomSpeed = 4f;
    private float _zoomSpeedMin = 2f;
    private float _zoomSpeedMax = 8f;
    private float _cameraAngleXY;
    private float _cameraAngleXZ;
    private float _lastCameraAngleXY;
    private float _lastCameraAngleXZ;
    private Vector3 _cameraTarget;
    private Vector3 _lastCameraTarget;

    //DROPPING
    private float _dropClock;
    private float _dropTime = 1f;
    private float _dropTimeDefault = 1f;
    private float _dropTimeMin = 0.4f;
    private float _dropTimeMax = 1f;

    //DIFFICULTY
    private float _difficultyClock;
    private float _difficultyTime = 10f;

    //CLEARING
    private bool _clearing;
    private float _clearClock;
    private float _clearTime = 0.1f;
    private float _clearTimeDefault = 0.1f;
    private bool[] _clearingLevels;

    //UI
    private List<GameObject> _menuList = new List<GameObject>();
    private int _menuDepth;
    private bool _freeSpin = true;

    private bool _needsUpdate;

    private bool _paused;
    private int _score;

    private void Awake()
    {
        //All instantiation goes here
        _isCellFilled = new bool[_grid.x, _grid.y, _grid.z];
        _tileLiterals = new GameObject[_grid.x, _grid.y, _grid.z];
        _ghostLiterals = new GameObject[_grid.x, _grid.y, _grid.z];

        //Sets the pointer position to the top middle of the game board
        _pointer.x = _grid.x / 2;
        _pointer.y = _grid.y - 1;
        _pointer.z = _grid.z / 2;

        SetCameraProperties();
    }

    private void Start()
    {
        //Starts game on pause with the main menu active
        _currentMenu = _mainMenu;
        _menuList.Add(_currentMenu);
        _paused = true;
        _freeSpin = true;
        _score = 0;
        _dropTime = _dropTimeMax;
        _dropTimeDefault = _dropTimeMax;

        //Sets all clearing levels to false, readying the ClearingLevels array
        _clearingLevels = new bool[_grid.y];
        for (int i = 0; i < _grid.y; i++)
        {
            _clearingLevels[i] = false;
        }

        //Positions the camera looking down over the game board.
        _camera.transform.position = Vector3.zero;
        _camera.transform.rotation = Quaternion.Euler(35, 0, 0);

        GenerateTiles();
    }

    public void NewGame()
    {
        _camera.transform.position = new Vector3(_grid.x / 2f, _grid.y / 2f + _cameraDistance, _grid.z / 2f);
        _camera.transform.rotation = Quaternion.Euler(45, 0, 0);
        
        for (int x = 0; x < _grid.x; x++)
        {
            for (int y = 0; y < _grid.y; y++)
            {
                for (int z = 0; z < _grid.z; z++)
                {
                    _isCellFilled[x, y, z] = false;
                }
            }
        }

        ClearAllGhostLevel();

        _piecesRoot.SetActive(true);
        _basePlate.SetActive(true);

        _currentPieceController = new PieceController();
        _currentPieceController.SetPosition(_pointer.x, _pointer.y, _pointer.z);

        _currentMaterial = _blockMaterials[Random.Range(0, _blockMaterials.Count)];
        _score = 0;
        _dropTimeDefault = _dropTimeMax;
        _dropTime = _dropTimeMax;
        _clearTime = _dropTimeMax;
        SetMenu(_inGameUi);
        _paused = false;
        _freeSpin = false;
        UpdateBoard();
    }

    private void SetCameraProperties()
    {
        _cameraTarget = new Vector3(_grid.x / 2f, _grid.y / 2f, _grid.z / 2f);
        _cameraDistanceMin = _grid.y;
        _cameraDistanceMax = _grid.y * 3;
        _cameraDistance = _grid.y * 1.5f;
        _cameraAngleXY = 135f;
        _cameraAngleXZ = 0f;
        _lastCameraAngleXY = _cameraAngleXY;
        _lastCameraAngleXZ = _cameraAngleXZ;
        _lastCameraTarget = _cameraTarget;
    }

    private void ResetCameraProperties()
    {
        _camera.transform.position = Vector3.zero;
        _camera.transform.rotation = Quaternion.Euler(35, 0, 0);
    }

    private void GenerateTiles()
    {
        if (_piecesRoot == null)
        {
            _piecesRoot = new GameObject();
            _piecesRoot.name = "Pieces Root";
        }

        if (_basePlate == null)
        {
            _basePlate = Instantiate(_basePlateSource, new Vector3(_grid.x / 2f - 0.5f, -0.8f, _grid.z / 2f - 0.5f),
                Quaternion.identity);
            _basePlate.transform.localScale =
                new Vector3(_grid.x - 0.01f, _basePlate.transform.localScale.y, _grid.z - 0.01f);
            _basePlate.SetActive(false);
        }

        for (int x = 0; x < _grid.x; x++)
        {
            for (int y = 0; y < _grid.y; y++)
            {
                for (int z = 0; z < _grid.z; z++)
                {
                    _isCellFilled[x, y, z] = false;

                    _tileLiterals[x, y, z] = Instantiate(_block, new Vector3(x, y, z), Quaternion.identity);
                    _tileLiterals[x, y, z].transform.parent = _piecesRoot.transform;

                    _ghostLiterals[x, y, z] = Instantiate(_ghostBlock, new Vector3(x, y, z), Quaternion.identity);
                    _ghostLiterals[x, y, z].transform.parent = _piecesRoot.transform;
                }
            }
        }
    }

    private void Update()
    {
        HandleInput();

        //Handles the automatic progression of the game, when not paused or clearing
        if (!_paused && !_clearing)
        {
            _dropClock += Time.deltaTime;

            //Drops the block down the vertical axis
            if (_dropClock > _dropTime)
            {
                _dropClock = 0f;
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.MoveY(-1);
                if (CheckPosition(testPieceController))
                {
                    MoveY(-1);
                }
                else
                {
                    PlacePiece();
                }
            }

            _difficultyClock += Time.deltaTime;

            //Speeds up block dropping as the game goes on
            if (_difficultyClock > _difficultyTime)
            {
                _difficultyClock = 0f;
                if (_dropTimeDefault * 0.95f > _dropTimeMin)
                {
                    _dropTimeDefault *= 0.95f;
                    _clearTime = _dropTimeDefault;

                    if (!Input.GetKey(_dropBtn))
                    {
                        _dropTime = _dropTimeDefault;
                    }
                }
            }
        }

        //Handles the clearing animation
        if (!_paused && _clearing)
        {
            _clearClock += Time.deltaTime;

            if (_clearClock > _clearTime)
            {
                for (int i = 0; i < _clearingLevels.Length; i++)
                {
                    if (_clearingLevels[i])
                    {
                        _clearEffect = Instantiate(_clearEffectSource, new Vector3(_grid.x / 2, i, _grid.z / 2),
                            Quaternion.identity);
                    }
                }

                PushDown();
                _clearClock = 0f;
                _clearing = false;
            }
        }
        else
        {
            if (_clearEffect != null && !_clearEffect.isPlaying)
            {
                Destroy(_clearEffect);
            }
        }

        if (_freeSpin)
        {
            _cameraAngleXZ += 15f * Time.deltaTime;
            if (_cameraAngleXZ > 360f)
            {
                _cameraAngleXZ -= 360f;
            }
        }

        if (_needsUpdate)
        {
            UpdateBoard();
        }

        if (_lastCameraAngleXY != _cameraAngleXY || _lastCameraAngleXZ != _cameraAngleXZ ||
            _lastCameraTarget != _cameraTarget)
        {
            PositionCamera();
        }
    }

    /// <summary>
    /// Handles any possible input for a player command.
    /// </summary>
    private void HandleInput()
    {
        if (_currentMenu != _mainMenu)
        {
            if (Input.GetKeyDown(_pauseBtn))
            {
                if (_currentMenu == _inGameUi)
                {
                    Pause();
                }
                else if (_currentMenu == _pauseMenu)
                {
                    Resume();
                }
                else if (_currentMenu == _gameOverMenu)
                {
                }
                else
                {
                    PreviousCanvas();
                }
            }
        }

        if (!_clearing && !_paused)
        {
            //Sets the keys to be relative to the orientation of the camera
            KeyCode relativeLeft = _leftBtn;
            KeyCode relativeRight = _rightBtn;
            KeyCode relativeForward = _forwardBtn;
            KeyCode relativeBackward = _backwardBtn;
            if ((_cameraAngleXZ >= 0f && _cameraAngleXZ < 45f) || (_cameraAngleXZ >= 315f && _cameraAngleXZ <= 360f))
            {
                //Debug.Log("Primed for relative control in Quadrant 1");
                relativeLeft = _leftBtn;
                relativeRight = _rightBtn;
                relativeForward = _forwardBtn;
                relativeBackward = _backwardBtn;
            }
            else if (_cameraAngleXZ >= 45f && _cameraAngleXZ < 135f)
            {
                //Debug.Log("Primed for relative control in Quadrant 2");
                relativeLeft = _backwardBtn;
                relativeRight = _forwardBtn;
                relativeForward = _leftBtn;
                relativeBackward = _rightBtn;
            }
            else if (_cameraAngleXZ >= 135f && _cameraAngleXZ < 225f)
            {
                //Debug.Log("Primed for relative control in Quadrant 3");
                relativeLeft = _rightBtn;
                relativeRight = _leftBtn;
                relativeForward = _backwardBtn;
                relativeBackward = _forwardBtn;
            }
            else if (_cameraAngleXZ >= 225f && _cameraAngleXZ < 315f)
            {
                //Debug.Log("Primed for relative control in Quadrant 4");
                relativeLeft = _forwardBtn;
                relativeRight = _backwardBtn;
                relativeForward = _rightBtn;
                relativeBackward = _leftBtn;
            }
            else
            {
                Debug.Log("Tracing the value of CameraAngle XZ is: " + _cameraAngleXZ);
                Debug.Log("No conditions met, this shouldn't be happening.");
            }

            if (Input.GetKeyDown(relativeLeft))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.MoveX(-1);
                if (CheckPosition(testPieceController))
                {
                    MoveX(-1);
                }
            }

            if (Input.GetKeyDown(relativeRight))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.MoveX(1);
                if (CheckPosition(testPieceController))
                {
                    MoveX(1);
                }
            }

            if (Input.GetKeyDown(relativeBackward))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.MoveZ(-1);
                if (CheckPosition(testPieceController))
                {
                    MoveZ(-1);
                }
            }

            if (Input.GetKeyDown(relativeForward))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.MoveZ(1);
                if (CheckPosition(testPieceController))
                {
                    MoveZ(1);
                }
            }

            if (Input.GetKeyDown(_rotateXZBtn))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.RotateXZ(!_XZClockwise);
                if (CheckPosition(testPieceController))
                {
                    RotateXZ();
                }
                else
                {
                    testPieceController = _currentPieceController.Clone();
                    testPieceController.RotateXZ(_XZClockwise);
                    if (CheckPosition(testPieceController))
                    {
                        _XZClockwise = !_XZClockwise;
                        RotateXZ();
                    }
                }
            }

            if (Input.GetKeyDown(_rotateXYBtn))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.RotateXY(!_XYClockwise);
                if (CheckPosition(testPieceController))
                {
                    RotateXY();
                }
                else
                {
                    testPieceController = _currentPieceController.Clone();
                    testPieceController.RotateXY(_XYClockwise);
                    if (CheckPosition(testPieceController))
                    {
                        _XYClockwise = !_XYClockwise;
                        RotateXY();
                    }
                }
            }

            if (Input.GetKeyDown(_rotateYZBtn))
            {
                PieceController testPieceController = _currentPieceController.Clone();
                testPieceController.RotateYZ(!_YZClockwise);
                if (CheckPosition(testPieceController))
                {
                    RotateYZ();
                }
                else
                {
                    testPieceController = _currentPieceController.Clone();
                    testPieceController.RotateYZ(_YZClockwise);
                    if (CheckPosition(testPieceController))
                    {
                        _YZClockwise = !_YZClockwise;
                        RotateYZ();
                    }
                }
            }
        }

        if (!_paused)
        {
            //Rotate camera when the rotate camera button is held down
            if (Input.GetKey(_rotateCamera))
            {
                //Rotates camera around the XZ plane when the mouse is moved in the X direction
                //m_camera.transform.RotateAround(CameraTarget, Vector3.up, 20 * Input.GetAxisRaw("Mouse X") * RotationSpeed);

                //Updates the tracker for camera angle around the XZ plane and keeps its value between 0 and 360 degrees
                _cameraAngleXZ += 20 * Input.GetAxisRaw("Mouse X") * _rotationSpeed;
                if (_cameraAngleXZ > 360f)
                {
                    _cameraAngleXZ -= 360f;
                }
                else if (_cameraAngleXZ < 0f)
                {
                    _cameraAngleXZ += 360f;
                }

                //Checks to make sure that the Y mouse movement doesn't put the camera 20 degrees below the horizon (180 degrees) or go past looking straight down at the target
                if (_cameraAngleXY + (20 * Input.GetAxisRaw("Mouse Y") * _rotationSpeed) < 200 &&
                    _cameraAngleXY + (20 * Input.GetAxisRaw("Mouse Y") * _rotationSpeed) >= 90f)
                {
                    //Transform to find the axis which is perpindicular to the vertically oriented plane between the camera and target
                    _cameraAngleXY += 20 * Input.GetAxisRaw("Mouse Y") * _rotationSpeed;
                }
            }

            //Zooms in and out using the mouse scrool wheel
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f)
            {
                //Checks to make sure the camera doesn't go past the maximum or minimum zoom distance
                if (_cameraDistance + _zoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel") > _cameraDistanceMin &&
                    _cameraDistance + _zoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel") < _cameraDistanceMax)
                {
                    _cameraDistance += _zoomSpeed * -1f * Input.GetAxisRaw("Mouse ScrollWheel");
                }
            }
        }

        if (Input.GetKeyDown(_dropBtn))
        {
            _dropTime = 0.1f;
        }

        if (Input.GetKeyUp(_dropBtn))
        {
            _dropTime = _dropTimeDefault;
        }
    }

    /// <summary>
    /// Completely refreshes the rendering of the game board and sets NeedsUpdate to false.
    /// </summary>
    private void UpdateBoard()
    {
        HidePiece();

        for (var x = 0; x < _grid.x; x++)
        {
            for (var y = 0; y < _grid.y; y++)
            {
                for (var z = 0; z < _grid.z; z++)
                {
                    var tile = _tileLiterals[x, y, z].GetComponent<Block>();
                    tile.Renderer.enabled = _isCellFilled[x, y, z];
                }
            }
        }

        DrawPiece();
        _needsUpdate = false;
    }

    private void PositionCamera()
    {
        _camera.transform.position = new Vector3(
            -1 * Mathf.Sin(Mathf.Deg2Rad * _cameraAngleXZ) *
            Mathf.Sqrt(1 - Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * _cameraAngleXY), 2)),
            Mathf.Sin(Mathf.Deg2Rad * _cameraAngleXY),
            -1 * Mathf.Cos(Mathf.Deg2Rad * _cameraAngleXZ) *
            Mathf.Sqrt(1 - Mathf.Pow(Mathf.Sin(Mathf.Deg2Rad * _cameraAngleXY), 2))) * _cameraDistance + _cameraTarget;
        _camera.transform.LookAt(_cameraTarget);
    }

    private void DrawPiece()
    {
        var pieces = _currentPieceController.GetPieces();

        for (var i = 0; i < pieces.Length; i++)
        {
            var finalX = Round(pieces[i].GetX()) + _currentPieceController.GetPositionX();
            var finalY = Round(pieces[i].GetY()) + _currentPieceController.GetPositionY();
            var finalZ = Round(pieces[i].GetZ()) + _currentPieceController.GetPositionZ();
            if (_grid.x > finalX && finalX >= 0 && _grid.y > finalY && finalY >= 0 && _grid.z > finalZ && finalZ >= 0)
            {
                var tile = _tileLiterals[finalX, finalY, finalZ].GetComponent<Block>();
                var y = Round(pieces[i].GetY());

                var ghostTile = _ghostLiterals[finalX, 0, finalZ].GetComponent<GhostBlock>();

                ghostTile.Renderer.enabled = true;
                tile.SetMaterial(_currentMaterial);
                tile.Renderer.enabled = true;
            }
            else
            {
                Debug.Log("Failed to place at position: (" + finalX + "," + finalY + "," + finalZ + ")");
            }
        }
    }

    private void HidePiece()
    {
        var pieces = _currentPieceController.GetPieces();
        for (var i = 0; i < pieces.Length; i++)
        {
            var finalX = Round(pieces[i].GetX()) + _currentPieceController.GetPositionX();
            var finalY = Round(pieces[i].GetY()) + _currentPieceController.GetPositionY();
            var finalZ = Round(pieces[i].GetZ()) + _currentPieceController.GetPositionZ();
            if (_grid.x > finalX && finalX >= 0 && _grid.y > finalY && finalY >= 0 && _grid.z > finalZ && finalZ >= 0)
            {
                var tile = _tileLiterals[finalX, finalY, finalZ].GetComponent<Block>();
                var y = Round(pieces[i].GetY());

                var ghostTile = _ghostLiterals[finalX, 0, finalZ].GetComponent<GhostBlock>();

                if (ghostTile)
                {
                    ghostTile.Renderer.enabled = false;
                }

                tile.Renderer.enabled = false;
            }
            else
            {
                Debug.Log("Failed to place at position: (" + finalX + "," + finalY + "," + finalZ + ")");
            }
        }
    }

    private bool CheckPosition(PieceController input)
    {
        bool valid = true;
        Piece[] pieces = input.GetPieces();
        for (int i = 0; i < pieces.Length; i++)
        {
            int finalX = Round(pieces[i].GetX()) + input.GetPositionX();
            int finalY = Round(pieces[i].GetY()) + input.GetPositionY();
            int finalZ = Round(pieces[i].GetZ()) + input.GetPositionZ();
            if (!(_grid.x > finalX && finalX >= 0 && _grid.y > finalY && finalY >= 0 && _grid.z > finalZ &&
                  finalZ >= 0))
            {
                valid = false;
                return valid;
            }

            if (_isCellFilled[finalX, finalY, finalZ])
            {
                valid = false;
                return valid;
            }
        }

        return valid;
    }

    private void PlacePiece()
    {
        ClearAllGhostLevel();

        Piece[] pieces = _currentPieceController.GetPieces();
        for (int i = 0; i < pieces.Length; i++)
        {
            int finalX = Round(pieces[i].GetX()) + _currentPieceController.GetPositionX();
            int finalY = Round(pieces[i].GetY()) + _currentPieceController.GetPositionY();
            int finalZ = Round(pieces[i].GetZ()) + _currentPieceController.GetPositionZ();
            _isCellFilled[finalX, finalY, finalZ] = true;

            if (CheckForClear(finalY))
            {
                ClearLevel(finalY);
            }
        }

        PieceController testNewPieceController = new PieceController();
        testNewPieceController.SetPosition(_pointer.x, _pointer.y, _pointer.z);
        if (CheckPosition(testNewPieceController))
        {
            _currentPieceController = testNewPieceController.Clone();
            _currentMaterial = _blockMaterials[Random.Range(0, _blockMaterials.Count)];
        }
        else
        {
            EndGame();
        }

        UpdateBoard();
    }

    private bool CheckForClear(int level)
    {
        for (int x = 0; x < _grid.x; x++)
        {
            for (int z = 0; z < _grid.z; z++)
            {
                if (!_isCellFilled[x, level, z])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void ClearLevel(int level)
    {
        _clearing = true;
        for (int x = 0; x < _grid.x; x++)
        {
            for (int z = 0; z < _grid.z; z++)
            {
                var tile = _tileLiterals[x, level, z].GetComponent<Block>();
                tile.Clear(_clearTime);
            }
        }

        _clearingLevels[level] = true;
    }

    private void ClearAllGhostLevel()
    {
        _clearing = true;
        for (int x = 0; x < _grid.x; x++)
        {
            for (int y = 0; y < _grid.y; y++)
            {
                for (int z = 0; z < _grid.z; z++)
                {
                    var tileGhost = _ghostLiterals[x, y, z].GetComponent<GhostBlock>();
                    tileGhost.Clear(_clearTime);

                    tileGhost.Renderer.enabled = false;
                }
            }
        }
    }

    private void PushDown()
    {
        int levelsCleared = 0;
        for (int i = 0; i < _clearingLevels.Length; i++)
        {
            if (_clearingLevels[i])
            {
                for (int y = i - levelsCleared; y < _grid.y; y++)
                {
                    for (int x = 0; x < _grid.x; x++)
                    {
                        for (int z = 0; z < _grid.z; z++)
                        {
                            if (y < _grid.y - 1)
                            {
                                _isCellFilled[x, y, z] = _isCellFilled[x, y + 1, z];
                                var currentTile = _tileLiterals[x, y, z].GetComponent<Block>();
                                var previousTile = _tileLiterals[x, y + 1, z].GetComponent<Block>();
                                currentTile.SetMaterial(previousTile.GetMaterial());
                            }
                            else
                            {
                                _isCellFilled[x, y, z] = false;
                            }
                        }
                    }
                }

                levelsCleared += 1;
                _score += 1;
                _clearingLevels[i] = false;
            }
        }

        UpdateBoard();
    }

    private void RotateXZ()
    {
        HidePiece();
        _currentPieceController.RotateXZ(!_XZClockwise);
        DrawPiece();
    }

    private void RotateXY()
    {
        HidePiece();
        _currentPieceController.RotateXY(!_XYClockwise);
        DrawPiece();
    }

    private void RotateYZ()
    {
        HidePiece();
        _currentPieceController.RotateYZ(!_YZClockwise);
        DrawPiece();
    }

    private void MoveX(int input)
    {
        HidePiece();
        _currentPieceController.MoveX(input);
        DrawPiece();
    }

    private void MoveY(int input)
    {
        HidePiece();
        _currentPieceController.MoveY(input);
        DrawPiece();
    }

    private void MoveZ(int input)
    {
        HidePiece();
        _currentPieceController.MoveZ(input);
        DrawPiece();
    }

    private int Round(float input)
    {
        float output = input;
        int outputModifier = 0;
        output -= (int) input;
        if (output >= 0.5f)
        {
            outputModifier = 1;
        }

        return (int) input + outputModifier;
    }

    private void EndGame()
    {
        SetMenu(_gameOverMenu);
        _freeSpin = true;
        _paused = true;
    }

    public int GetScore()
    {
        return _score;
    }

    public void Pause()
    {
        SetMenu(_pauseMenu);
        _paused = true;
    }

    public void Resume()
    {
        SetMenu(_inGameUi);
        _paused = false;
        _freeSpin = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        ResetCameraProperties();
        _basePlate.SetActive(false);
        _piecesRoot.SetActive(false);
        SetMenu(_mainMenu);
        _freeSpin = true;
        _paused = true;
    }

    private void SetMenu(GameObject input)
    {
        _currentMenu.SetActive(false);
        _currentMenu = input;
        _currentMenu.SetActive(true);
        if (_menuDepth < 10)
        {
            _menuList.Add(_currentMenu);
            _menuDepth++;
        }
        else
        {
            _menuList.RemoveAt(0);
            _menuList.Add(_currentMenu);
        }
    }

    private void PreviousCanvas()
    {
        if (_menuDepth > 0)
        {
            _menuList.RemoveAt(_menuDepth);
            SetMenu(_menuList[_menuDepth - 1]);
            _menuList.RemoveAt(_menuDepth - 1);
            _menuDepth -= 2;
        }
    }

    public void SetRotationSpeed(float input)
    {
        _rotationSpeed = Mathf.Lerp(_rotationSpeedMin, _rotationSpeedMax, input);
    }

    public float GetRotationSpeed()
    {
        return (_rotationSpeed - _rotationSpeedMin) / (_rotationSpeedMax - _rotationSpeedMin);
    }

    public void SetZoomSpeed(float input)
    {
        _zoomSpeed = Mathf.Lerp(_zoomSpeedMin, _zoomSpeedMax, input);
    }

    public float GetZoomSpeed()
    {
        return (_zoomSpeed - _zoomSpeedMin) / (_zoomSpeedMax - _zoomSpeedMin);
    }
}