using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private InputController _inputController;
    private readonly List<GameObject> _menuList = new List<GameObject>();
    private int _menuDepth;
    private GameObject _basePlate;
    private GameObject _piecesRoot;
    private GameObject _godRays;
    private Material _currentMaterial;
    private ParticleSystem _currentClearEffect;
    private GameObject _currentMenu;
    private int _score;
    
    [SerializeField] private Settings _settings;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _pauseMenuBtn;
    [SerializeField] private GameObject _gameOverMenu;

    [SerializeField] private RectTransform _DropDownBtn;
    [SerializeField] private RectTransform _RotateBtn;
    [SerializeField] private RectTransform _FlipBtn;

    public RectTransform DropDownBtn => _DropDownBtn;
    public RectTransform RotateBtn => _RotateBtn;
    public RectTransform FlipBtn => _FlipBtn;

    public GameObject CurrentMenu => _currentMenu;
    public GameObject MainMenu => _mainMenu;
    public GameObject InGameUI => _inGameUI;
    public GameObject PauseMenu => _pauseMenu;
    public GameObject GameOverMenu => _gameOverMenu;
    
    public void RotateXY()
    {
        _inputController.RotateOnXY();
    }
    public void FlipVertical()
    {
        _inputController.FlipVertical();
    }
    public void DropDown()
    {
        _inputController.DropDown();
    }
    public void DropUp()
    {
        _inputController.DropUp();
    }

    public void OnPauseMenu()
    {
        _inputController.PauseMenu();
    }
    
    public void NewGame()
    {
        _currentMenu.SetActive(false);
        _pauseMenuBtn.SetActive(true);
        _pauseMenuBtn.gameObject.GetComponent<PlayAnimation>().ResetAnimation();
        
        SetMenu(_inGameUI);
        
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int y = 0; y < _settings.GridSize.y; y++)
            {
                for (int z = 0; z < _settings.GridSize.z; z++)
                {
                    _settings.IsCellFilled[x, y, z] = false;
                }
            }
        }

        ClearAllGhostLevel();

        _piecesRoot.SetActive(true);
        _basePlate.SetActive(true);
        _godRays.SetActive(true);
        _settings.CurrentPieceController = new PieceController();
        _settings.CurrentPieceController.SetPosition(_settings.SpawnPointer.x, _settings.SpawnPointer.y, _settings.SpawnPointer.z);

        _currentMaterial = _settings.BlockMaterials[Random.Range(0, _settings.BlockMaterials.Count)];
        _score = 0;
        _settings.CurrentDropTime = _settings.DropTimeMinMax.y;
        _settings.DropTimeDefault = _settings.DropTimeMinMax.y;
        _settings.ClearTime = _settings.DropTimeMinMax.y;
        SetMenu(_inGameUI);
        _settings.Paused = false;
        UpdateBoard();
    }
    public void Pause()
    {
        SetMenu(_pauseMenu);
        _settings.Paused = true;
    }
  
    public void PreviousCanvas()
    {
        if (_menuDepth > 0)
        {
            _menuList.RemoveAt(_menuDepth);
            SetMenu(_menuList[_menuDepth - 1]);
            _menuList.RemoveAt(_menuDepth - 1);
            _menuDepth -= 2;
        }
    }
    public void Resume()
    {
        SetMenu(_inGameUI);
        _settings.Paused = false;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GoToMainMenu()
    {        
        _pauseMenuBtn.SetActive(false);
        _basePlate.SetActive(false);
        _godRays.SetActive(false);
        _piecesRoot.SetActive(false);
        SetMenu(_mainMenu);
        _settings.Paused = true;
    }
    public int GetScore()
    {
        return _score;
    }
    
    private void Awake()
    {
        _inputController = new InputController(this, _settings);

        //All instantiation goes here
        _settings.IsCellFilled = new bool[_settings.GridSize.x, _settings.GridSize.y, _settings.GridSize.z];
        _settings.TileLiterals = new GameObject[_settings.GridSize.x, _settings.GridSize.y, _settings.GridSize.z];
        _settings.GhostLiterals = new GameObject[_settings.GridSize.x, _settings.GridSize.y, _settings.GridSize.z];

        //Sets the pointer position to the top middle of the game board
        _settings.SpawnPointer.x = _settings.GridSize.x / 2;
        _settings.SpawnPointer.y = _settings.GridSize.y - 1;
        _settings.SpawnPointer.z = _settings.GridSize.z / 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int y = 0; y < _settings.GridSize.y; y++)
            {
                for (int z = 0; z < _settings.GridSize.z; z++)
                {
                    Gizmos.DrawWireCube(new Vector3(x,y,z), Vector3.one);
                }
            }
        }
    }

    private void Start()
    {
        //Starts game on pause with the main menu active
        _currentMenu = _mainMenu;
        _menuList.Add(_currentMenu);
        _settings.Paused = true;
        _pauseMenuBtn.SetActive(false);
        _score = 0;
        _settings.CurrentDropTime = _settings.DropTimeMinMax.y;
        _settings.DropTimeDefault = _settings.DropTimeMinMax.y;

        //Sets all clearing levels to false, readying the ClearingLevels array
        _settings.ClearingLevels = new bool[_settings.GridSize.y];
        for (int i = 0; i < _settings.GridSize.y; i++)
        {
            _settings.ClearingLevels[i] = false;
        }
        
        GenerateTiles();
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
            _basePlate = Instantiate(_settings.BasePlateSource, 
                new Vector3(_settings.GridSize.x / 2f - 0.5f,
                -0.9f,
                _settings.GridSize.z / 2f -0.5f), Quaternion.identity);
            _basePlate.SetActive(false);
        }

        if (_godRays == null)
        {
            _godRays = Instantiate(_settings.GodRays, new Vector3(
                _settings.GridSize.x / 2f - 0.5f,
                0,
                _settings.GridSize.z / 2f -2f), Quaternion.Euler(10, 110, 20));
        }
        
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int y = 0; y < _settings.GridSize.y; y++)
            {
                for (int z = 0; z < _settings.GridSize.z; z++)
                {
                    _settings.IsCellFilled[x, y, z] = false;

                    _settings.TileLiterals[x, y, z] =
                        Instantiate(_settings.Block, new Vector3(x, y, z), Quaternion.identity);
                    _settings.TileLiterals[x, y, z].transform.parent = _piecesRoot.transform;

                    _settings.GhostLiterals[x, y, z] =
                        Instantiate(_settings.GhostBlock, new Vector3(x, y, z), Quaternion.identity);
                    _settings.GhostLiterals[x, y, z].transform.parent = _piecesRoot.transform;
                }
            }
        }
    }
    private void Update()
    {
        _inputController.Update();
        
        //Handles the clearing animation
        if (!_settings.Paused && _settings.Clearing)
        {
            _settings.ClearClock += Time.deltaTime;

            if (_settings.ClearClock > _settings.ClearTime)
            {
                for (int i = 0; i < _settings.ClearingLevels.Length; i++)
                {
                    if (_settings.ClearingLevels[i])
                    {
                        _currentClearEffect = Instantiate(_settings.ClearEffectSource,
                            new Vector3(_settings.GridSize.x / 2, i, _settings.GridSize.z / 2), Quaternion.identity);
                    }
                }

                PushDown();
                _settings.ClearClock = 0f;
                _settings.Clearing = false;
            }
        }
        // TODO: need to organize pool / cache effects
        if (_currentClearEffect != null && !_currentClearEffect.isPlaying)
        {
            Destroy(_currentClearEffect.gameObject);
        }
        

        if (_settings.NeedsUpdate)
        {
            UpdateBoard();
        }
    }
    private void UpdateBoard()
    {
        HidePiece();

        for (var x = 0; x < _settings.GridSize.x; x++)
        {
            for (var y = 0; y < _settings.GridSize.y; y++)
            {
                for (var z = 0; z < _settings.GridSize.z; z++)
                {
                    var tile = _settings.TileLiterals[x, y, z].GetComponent<Block>();
                    tile.Renderer.enabled = _settings.IsCellFilled[x, y, z];
                }
            }
        }

        DrawPiece();
        _settings.NeedsUpdate = false;
    }
 
    public void DrawPiece()
    {
        var pieces = _settings.CurrentPieceController.GetPieces();

        for (var i = 0; i < pieces.Length; i++)
        {
            var finalX = Utilities.Round(pieces[i].GetX()) + _settings.CurrentPieceController.GetPositionX();
            var finalY = Utilities.Round(pieces[i].GetY()) + _settings.CurrentPieceController.GetPositionY();
            var finalZ = Utilities.Round(pieces[i].GetZ()) + _settings.CurrentPieceController.GetPositionZ();
            if (_settings.GridSize.x > finalX && finalX >= 0 && _settings.GridSize.y > finalY && finalY >= 0 &&
                _settings.GridSize.z > finalZ && finalZ >= 0)
            {
                var tile = _settings.TileLiterals[finalX, finalY, finalZ].GetComponent<Block>();
                var ghostTile = _settings.GhostLiterals[finalX, 0, finalZ].GetComponent<GhostBlock>();

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
    public void HidePiece()
    {
        var pieces = _settings.CurrentPieceController.GetPieces();
        for (var i = 0; i < pieces.Length; i++)
        {
            var finalX = Utilities.Round(pieces[i].GetX()) + _settings.CurrentPieceController.GetPositionX();
            var finalY = Utilities.Round(pieces[i].GetY()) + _settings.CurrentPieceController.GetPositionY();
            var finalZ = Utilities.Round(pieces[i].GetZ()) + _settings.CurrentPieceController.GetPositionZ();
            if (_settings.GridSize.x > finalX && finalX >= 0 && _settings.GridSize.y > finalY && finalY >= 0 &&
                _settings.GridSize.z > finalZ && finalZ >= 0)
            {
                var tile = _settings.TileLiterals[finalX, finalY, finalZ].GetComponent<Block>();
                var y = Utilities.Round(pieces[i].GetY());

                var ghostTile = _settings.GhostLiterals[finalX, 0, finalZ].GetComponent<GhostBlock>();

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
    public bool CheckPosition(PieceController input)
    {
        Piece[] pieces = input.GetPieces();
        for (int i = 0; i < pieces.Length; i++)
        {
            int finalX = Utilities.Round(pieces[i].GetX()) + input.GetPositionX();
            int finalY = Utilities.Round(pieces[i].GetY()) + input.GetPositionY();
            int finalZ = Utilities.Round(pieces[i].GetZ()) + input.GetPositionZ();
            if (!(_settings.GridSize.x > finalX && finalX >= 0 && _settings.GridSize.y > finalY && finalY >= 0 &&
                  _settings.GridSize.z > finalZ && finalZ >= 0))
            {
                return false;
            }

            if (_settings.IsCellFilled[finalX, finalY, finalZ])
            {
                return false;
            }
        }

        return true;
    }
    public void PlacePiece()
    {
        ClearAllGhostLevel();

        Piece[] pieces = _settings.CurrentPieceController.GetPieces();
        for (int i = 0; i < pieces.Length; i++)
        {
            int finalX = Utilities.Round(pieces[i].GetX()) + _settings.CurrentPieceController.GetPositionX();
            int finalY = Utilities.Round(pieces[i].GetY()) + _settings.CurrentPieceController.GetPositionY();
            int finalZ = Utilities.Round(pieces[i].GetZ()) + _settings.CurrentPieceController.GetPositionZ();
            _settings.IsCellFilled[finalX, finalY, finalZ] = true;

            if (CheckForClear(finalY))
            {
                ClearLevel(finalY);
            }
        }

        var controller = new PieceController();
        controller.SetPosition(_settings.SpawnPointer.x, _settings.SpawnPointer.y, _settings.SpawnPointer.z);
        if (CheckPosition(controller))
        {
            _settings.CurrentPieceController = controller.Clone();
            _currentMaterial = _settings.BlockMaterials[Random.Range(0, _settings.BlockMaterials.Count)];
        }
        else
        {
            EndGame();
        }

        UpdateBoard();
    }
    private bool CheckForClear(int level)
    {
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int z = 0; z < _settings.GridSize.z; z++)
            {
                if (!_settings.IsCellFilled[x, level, z])
                {
                    return false;
                }
            }
        }

        return true;
    }
    private void ClearLevel(int level)
    {
        _settings.Clearing = true;
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int z = 0; z < _settings.GridSize.z; z++)
            {
                var tile = _settings.TileLiterals[x, level, z].GetComponent<Block>();
                tile.Clear(_settings.ClearTime);
            }
        }

        _settings.ClearingLevels[level] = true;
    }
    private void ClearAllGhostLevel()
    {
        _settings.Clearing = true;
        for (int x = 0; x < _settings.GridSize.x; x++)
        {
            for (int y = 0; y < _settings.GridSize.y; y++)
            {
                for (int z = 0; z < _settings.GridSize.z; z++)
                {
                    var tileGhost = _settings.GhostLiterals[x, y, z].GetComponent<GhostBlock>();
                    tileGhost.Clear(_settings.ClearTime);

                    tileGhost.Renderer.enabled = false;
                }
            }
        }
    }
    private void PushDown()
    {
        int levelsCleared = 0;
        for (int i = 0; i < _settings.ClearingLevels.Length; i++)
        {
            if (_settings.ClearingLevels[i])
            {
                for (int y = i - levelsCleared; y < _settings.GridSize.y; y++)
                {
                    for (int x = 0; x < _settings.GridSize.x; x++)
                    {
                        for (int z = 0; z < _settings.GridSize.z; z++)
                        {
                            if (y < _settings.GridSize.y - 1)
                            {
                                _settings.IsCellFilled[x, y, z] = _settings.IsCellFilled[x, y + 1, z];
                                var currentTile = _settings.TileLiterals[x, y, z].GetComponent<Block>();
                                var previousTile = _settings.TileLiterals[x, y + 1, z].GetComponent<Block>();
                                currentTile.SetMaterial(previousTile.GetMaterial());
                            }
                            else
                            {
                                _settings.IsCellFilled[x, y, z] = false;
                            }
                        }
                    }
                }

                levelsCleared += 1;
                _score += 1;
                _settings.ClearingLevels[i] = false;
            }
        }

        UpdateBoard();
    }
    private void EndGame()
    {
        SetMenu(_gameOverMenu);
        _settings.Paused = true;
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
}