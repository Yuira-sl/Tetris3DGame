using System;

namespace Octamino
{
    [Serializable]
    public class HighscoreEntry: IComparable<HighscoreEntry>
    {
        public int Score;

        public HighscoreEntry(int score)
        {
            Score = score;
        }

        public int CompareTo(HighscoreEntry other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Score.CompareTo(other.Score);
        }
    }
}