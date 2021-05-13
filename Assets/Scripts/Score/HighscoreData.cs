using System.Collections.Generic;

namespace Octamino
{
    [System.Serializable]
    public class HighscoreData
    {
        public List<HighscoreEntry> Highscores;

        public HighscoreData()
        {
            Highscores = new List<HighscoreEntry>();
        }
    }
}