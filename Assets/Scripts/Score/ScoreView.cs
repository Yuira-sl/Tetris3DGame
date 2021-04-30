using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class ScoreView : MonoBehaviour
    {
        public Text ScoreText;
        public Game Game;

        private void Update()
        {
            var padLength = Constant.ScoreFormat.Length;
            var padCharacter = Constant.ScoreFormat.PadCharacter;
            ScoreText.text = Game.Score.Value.ToString().PadLeft(padLength, padCharacter);
        }
    }
}