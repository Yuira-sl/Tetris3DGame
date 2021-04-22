using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Period to trigger block descent
    private float _currentPeriod;
    
    //Period when not in speed mode
    private float _normalPeriod;
    private int level = 1;
    
    [SerializeField] private InputController _inputController;
    [SerializeField] private GameData _gameData;
    [SerializeField] private BlockController _blockController;

    [SerializeField] private GameObject _gameOverUI;
    
    public float CurrentPeriod => _currentPeriod;

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

    public void NewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Game");
    }

    public void Resume(GameObject go)
    {
        if (go.activeSelf)
        { 
            _blockController.IsPaused = false;
            go.SetActive(false);
        }
    }
    
    public void ToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    public bool IsPaused()
    {
        return _blockController.IsPaused;
    }
    
    private void Awake()
    {
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
        _inputController.OnSpeedDown += OnSpeedDown;
        _inputController.OnSpeedUp += OnSpeedUp;
        
        _blockController.OnBlockSettle += OnBlockSettle;

        _normalPeriod = _gameData.StartingPeriod;
        _currentPeriod = _normalPeriod;
    }

    private void OnDestroy()
    {
        _inputController.OnSpeedDown -= OnSpeedDown;
        _inputController.OnSpeedUp -= OnSpeedUp;
     
        _blockController.OnBlockSettle -= OnBlockSettle;
    }

    //Shrink the current period
    private void OnSpeedDown()
    {
        _currentPeriod = _gameData.SpeedPeriod;
    }

    //Return to normal period
    private void OnSpeedUp()
    {
        _currentPeriod = _normalPeriod;
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

    private void OnLevelUp()
    {
        _normalPeriod -= _gameData.DecrementPerLevel;
        _currentPeriod = _normalPeriod;
    }

    public int GetLevel()
    {
        return level;
    }

    public void LevelUp()
    {
        level++;
        OnLevelUp();
    }

    //Checks if game is over to the specific coordinate
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
        //Stop time
        Time.timeScale = 0;

        _gameOverUI.SetActive(true);
        //Destroy required objects
    }
}
