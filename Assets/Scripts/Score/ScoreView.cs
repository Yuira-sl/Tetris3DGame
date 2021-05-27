using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class ScoreView : MonoBehaviour
    {
        public Text ScoreText;

        private void Update()
        {
            var padLength = Constant.ScoreFormat.Length;
            var padCharacter = Constant.ScoreFormat.PadCharacter;
            ScoreText.text = Game.Instance.Score.Value.ToString().PadLeft(padLength, padCharacter);
        }
    }
}