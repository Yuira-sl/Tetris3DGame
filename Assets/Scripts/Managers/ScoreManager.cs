using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    private int _currentScore;
    private int _highScore;
    
    [SerializeField] private ScoreData _scoreData;
    [SerializeField] private Text _currentScoreText;
    [SerializeField] private Text _highScoreText;
    [SerializeField] private Text _currentPeriodText;
    
    public void AddRowScore()
    {
        _currentScore += _scoreData.PointsPerRow * GameManager.Instance.GetLevel();

        //Updates high score variable if needed
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            SetHighScore(_highScore);
        }

        UpdateScoreText();
    }
    
    public int GetCurrentScore()
    {
        return _currentScore;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GameManager.Instance.BlockController.OnBlockSettle += OnBlockSettle;
    }

    private void Start()
    {
        _highScore = GetHighScore();
        UpdateScoreText();
    }

    private void OnDestroy()
    {
        GameManager.Instance.BlockController.OnBlockSettle -= OnBlockSettle;
    }

    private void OnBlockSettle(List<Vector2Int> positions, bool speed)
    {
        AddBlockScore(speed);
    }
    
    private int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore");
    }

    private void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
    }

    private void AddBlockScore(bool withSpeed)
    {
        _currentScore += !withSpeed
            ? _scoreData.PointsWithoutSpeed * GameManager.Instance.GetLevel()
            : _scoreData.PointsWithSpeed * GameManager.Instance.GetLevel();

        //Updates high score variable if needed
        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            SetHighScore(_highScore);
        }

        UpdateScoreText();
    }
    
    //Updates current score and high score text
    private void UpdateScoreText()
    {
        _currentScoreText.text = _currentScore.ToString();
        _highScoreText.text = _highScore.ToString();
        _currentPeriodText.text = GameManager.Instance.GetPeriod().ToString();
    }
}
