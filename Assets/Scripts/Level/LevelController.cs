using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LevelCallback();

public class LevelController: MonoBehaviour
{
    private ScoreManager _scoreManager;
    private LevelScores _levelScores;
    private List<int> _scores = new List<int>();
    private int _currentScoreLevel;
    private int _level = 1;
    
    [SerializeField] private Text _currentLevel;

    public event LevelCallback OnLevelUp;

    private void Awake()
    {
        _scoreManager = GetComponent<ScoreManager>();
    }

    private void Start()
    {
        _level = 1;
        _currentLevel.text = _level.ToString();
        
        var values = (int[]) Enum.GetValues(typeof(LevelScores));
        for (int i = 0; i < values.Length; i++)
        {
            _scores.Add(values[i]);
        }
        _currentScoreLevel = _scores[0];
    }

    private void Update()
    {
        // TODO: Need to fix
        if (_currentScoreLevel <= _scores[_scores.Count - 1])
        {
            var index = _scores.IndexOf(_currentScoreLevel);
            if (CurrentScore() >= _currentScoreLevel)
            {
                var i = index + 1;
                if (i <= _scores.Count - 1)
                {
                    _currentScoreLevel = _scores[i];
                    _level++;
                    OnLevelUp?.Invoke();
                    _currentLevel.text = _level.ToString();
                }
            }
        }
    }
    
    private int CurrentScore()
    {
        return _scoreManager.GetCurrentScore();
    }
}