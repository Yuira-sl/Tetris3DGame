using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void LevelCallback();

public class LevelController: MonoBehaviour
{
    private int _currentScoreToAchieveLevel;
    private int _level = 1;

    [SerializeField] private ScoreData _scoreData;
    [SerializeField] private Text _currentLevel;

    public event LevelCallback OnLevelUp;

    private void Start()
    {
        _level = 1;
        _currentLevel.text = _level.ToString();
        _currentScoreToAchieveLevel = _scoreData.TargetScores[0];
        GameManager.Instance.BlockController.OnBlockSettle += OnBlockSettle;
    }
    
    private void OnBlockSettle(List<Vector2Int> positions, bool speed)
    {
        // TODO: Need to fix balance
        if (_currentScoreToAchieveLevel <= _scoreData.TargetScores[_scoreData.TargetScores.Count - 1])
        {
            var index = _scoreData.TargetScores.IndexOf(_currentScoreToAchieveLevel);
            if (CurrentScore() >= _currentScoreToAchieveLevel)
            {
                var i = index + 1;
                if (i <= _scoreData.TargetScores.Count - 1)
                {
                    _currentScoreToAchieveLevel = _scoreData.TargetScores[i];
                    _level++;
                    OnLevelUp?.Invoke();
                    _currentLevel.text = _level.ToString();
                }
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.BlockController.OnBlockSettle -= OnBlockSettle;
    }

    private int CurrentScore()
    {
        return ScoreManager.Instance.GetCurrentScore();
    }
}