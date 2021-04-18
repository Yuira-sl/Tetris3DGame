using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    //Period to trigger block descent
    private float _currentPeriod;
    
    //Period when not in speed mode
    private float _normalPeriod;
    private int level = 1;
    
    [SerializeField] private GameData _gameData;
    [SerializeField] private BlockController _blockController;

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
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main");
    }

    public void ToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Menu");
    }
    
    public void Quit()
    {
        Application.Quit();
    }

    private void Awake()
    {
        Time.timeScale = 1;
        if (_blockController.IsPaused)
        {
            _blockController.IsPaused = false;
        }
    }

    private void Start()
    {
        _playerInput = PlayerInput.Instance;

        _playerInput.OnSpeedDown += OnSpeedDown;
        _playerInput.OnSpeedUp += OnSpeedUp;
        _blockController.OnBlockSettle += OnBlockSettle;

        _normalPeriod = _gameData.StartingPeriod;
        _currentPeriod = _normalPeriod;
    }

    private void OnDestroy()
    {
        _playerInput.OnSpeedDown -= OnSpeedDown;
        _playerInput.OnSpeedUp -= OnSpeedUp;
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
                //OnGameOver.Invoke();
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

        //Destroy required objects

        //Canvas
        GameObject canvasGO = FindObjectOfType<Canvas>().gameObject;
        
        if (canvasGO != null)
        {
            Destroy(canvasGO);
        }

        //SceneManager.LoadSceneAsync("Game Over", LoadSceneMode.Additive);
    }
}
