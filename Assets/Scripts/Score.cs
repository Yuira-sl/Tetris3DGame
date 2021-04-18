using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private GameManager gameManager;
    private int _currentScore;
    private int _highScore;
    
    [SerializeField] private ScoreData _scoreData;
    [SerializeField] private BlockController _blockController;
    [SerializeField] private Text _currentScoreText;
    [SerializeField] private Text _highScoreText;
    
    public void AddRowScore()
    {
        _currentScore += _scoreData.PointsPerRow * gameManager.GetLevel();

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
        gameManager = GetComponent<GameManager>();

        if (_blockController != null)
        {
            _blockController.OnBlockSettle += OnBlockSettle;
        }
    }

    private void Start()
    {
        _highScore = GetHighScore();
        UpdateScoreText();
    }

    private void OnDestroy()
    {
        _blockController.OnBlockSettle -= OnBlockSettle;
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
            ? _scoreData.PointsWithoutSpeed * gameManager.GetLevel()
            : _scoreData.PointsWithSpeed * gameManager.GetLevel();

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
        _currentScoreText.text = _currentScore.ToString("000000");
        _highScoreText.text = _highScore.ToString("000000");
    }
}
