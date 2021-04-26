using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private float _currentBlockSpeed;
    private float _defaultBlockSpeed;
    private int _level = 1;
    
    [SerializeField] private GameData _gameData;
    [SerializeField] private InputController _inputController;
    [SerializeField] private BlockController _blockController;
    [SerializeField] private LevelController _levelController;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private bool _isNeedToCheck;
    public float CurrentBlockSpeed => _currentBlockSpeed;
    public InputController InputController => _inputController;
    public BlockController BlockController => _blockController;
    public LevelController LevelController => _levelController;

    public void OpenPauseMenu(GameObject go)
    {
        if (!go.activeSelf)
        {
            _blockController.IsPaused = true;
            go.SetActive(true);
        }
        else
        {
            _blockController.IsPaused = false;
            go.SetActive(false);
        }
    }
    
    public void Resume(GameObject go)
    {
        if (go.activeSelf)
        { 
            _blockController.IsPaused = false;
            go.SetActive(false);
        }
    }
    
    public void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public bool IsPaused()
    {
        return _blockController.IsPaused;
    }
    
    public int GetLevel()
    {
        return _level;
    }

    public float GetPeriod()
    {
        return _currentBlockSpeed;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        if (!_isNeedToCheck)
        {
            return;
        }
        
        Time.timeScale = 1;

        if (_blockController.IsPaused)
        {
            _blockController.IsPaused = false;
        }
        
        if (_gameOverUI.activeSelf)
        {
            _gameOverUI.SetActive(false);
        }
    }

    private void Start()
    {
        if (!_isNeedToCheck)
        {
            return;
        }
        _inputController.OnSpeedDown += OnSpeedDown;
        _inputController.OnSpeedUp += OnSpeedUp;
        _levelController.OnLevelUp += OnLevelUp;
        _blockController.OnBlockSettle += OnBlockSettle;

        _defaultBlockSpeed = _gameData.StartingPeriod;
        _currentBlockSpeed = _defaultBlockSpeed;
    }

    private void OnDestroy()
    {
        if (!_isNeedToCheck)
        {
            return;
        }
        _inputController.OnSpeedDown -= OnSpeedDown;
        _inputController.OnSpeedUp -= OnSpeedUp;
        _levelController.OnLevelUp -= OnLevelUp;
        _blockController.OnBlockSettle -= OnBlockSettle;
    }

    private void OnLevelUp()
    {
        _level++;
        _defaultBlockSpeed -= _gameData.DecrementPerLevel;
        _currentBlockSpeed = _defaultBlockSpeed;
    }
    
    //Shrink the current period
    private void OnSpeedDown()
    {
        _currentBlockSpeed = _gameData.SpeedPeriod;
    }

    //Return to normal period
    private void OnSpeedUp()
    {
        _currentBlockSpeed = _defaultBlockSpeed;
    }

    private void OnBlockSettle(List<Vector2Int> positions, bool speed)
    {
        bool gameOver;
        foreach(var position in positions)
        {
            gameOver = IsGameOver(position);
            if (gameOver)
            {
                GameOver();
                return;
            }
        }
    }
    
    private bool IsGameOver(Vector2Int position)
    {
        return position.y > _gameData.GameOverLimitRow - 1;
    }

    private void GameOver()
    {
        if (!_blockController.IsPaused)
        {
            _blockController.IsPaused = true;
        }
        
        Time.timeScale = 0;
        
        _gameOverUI.SetActive(true);
    }
}
