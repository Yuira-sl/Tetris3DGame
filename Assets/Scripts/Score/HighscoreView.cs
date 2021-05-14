using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Octamino
{
    public class HighscoreView : MonoBehaviour
    {
        private const string Highscore = "highscores";
        private const int MaxScoreEntries = 3;
        private HighscoreData _highscoreData;
        private UnityAction _onCloseCallback;
        private int _currentScore;
        
        public HighscoreEntryView HighscoreEntryView;
        public Text TitleText;
        public RectTransform TableContainer;
        public Button CloseButton;
        
        public void Show(UnityAction onCloseCallback)
        {
            _onCloseCallback = onCloseCallback;
            gameObject.SetActive(true);
            
            AddScore(new HighscoreEntry(Game.Instance.Score.Value));
            var scores = BuildBoard();
            RemoveUnnecessaryElements(scores);
        }
        
        private void Awake()
        {
            TitleText.text = Constant.Text.HighScore;

            CloseButton.onClick.AddListener(() =>
            {
                Hide();
                _onCloseCallback.Invoke();
            });

            _highscoreData = new HighscoreData();
            Load();
            Hide();
        }
        
        private void RemoveUnnecessaryElements(HighscoreEntry[] scores)
        {
            if (scores.Length > 3)
            {
                _highscoreData.Highscores.Clear();
                _highscoreData.Highscores.AddRange(scores);
                _highscoreData.Highscores.RemoveRange(
                    _highscoreData.Highscores.Count - 1, 
                    _highscoreData.Highscores.Count - 3);
            }
        }
        
        private HighscoreEntry[] BuildBoard()
        {
            var scores = GetSortedScores();
            for (int i = 0; i < MaxScoreEntries; i++)
            {
                var entryView = Instantiate(HighscoreEntryView);
                var entryRectTransform = entryView.GetComponent<RectTransform>();
                entryRectTransform.SetParent(TableContainer, false);
                entryView.Rank.text = RankName(i + 1);
                entryView.ScoreText.text = scores[i].Score.ToString();
            }

            return scores;
        }

        private string RankName(int value)
        {
            switch (value)
            {
                case 1:
                    return value + "TH";
                case 2:
                    return value + "ND";
                case 3:
                    return value + "RD";
            }
            return String.Empty;
        }
        
        private HighscoreEntry[] GetSortedScores()
        {
            var highscoreEntriesList = _highscoreData.Highscores;
            var highscoreEntriesArray = _highscoreData.Highscores.ToArray();
            
            if (highscoreEntriesArray.Length == 0 || highscoreEntriesArray.Length < 3)
            {
                while (highscoreEntriesList.Count < 3)
                {
                    highscoreEntriesList.Add(new HighscoreEntry(0));
                }

                highscoreEntriesArray = highscoreEntriesList.ToArray();
                return highscoreEntriesArray;
            }
            
            if (highscoreEntriesArray.Length > 0)
            {
                Array.Sort(highscoreEntriesArray);
                Array.Reverse(highscoreEntriesArray);
            }
            
            return highscoreEntriesArray;
        }
        
        private void AddScore(HighscoreEntry highscoreEntry)
        {
            if (highscoreEntry.Score > _currentScore)
            {
                _highscoreData.Highscores.Add(highscoreEntry);
                _currentScore = highscoreEntry.Score;
            }
        }

        private void Save()
        {
            var json = JsonUtility.ToJson(_highscoreData);
            PlayerPrefs.SetString(Highscore, json);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            var json = PlayerPrefs.GetString(Highscore, "{}");
            _highscoreData = JsonUtility.FromJson<HighscoreData>(json);
        }
        
        private void Hide()
        {
            for (var i = TableContainer.childCount - 1; i >= 0; i--)
            {
                var go = TableContainer.GetChild(i).gameObject;
                if (!go.Equals(TitleText.gameObject) && !go.Equals(CloseButton.gameObject)) Destroy(go);
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Save();
        }
    }
}