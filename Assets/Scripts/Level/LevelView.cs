using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class LevelView : MonoBehaviour
    {
        public Text Level;
        public Text Lines;
        public Game Game;

        private void Update()
        {
            Level.text = Game.Level.Number.ToString();
            Lines.text = Game.Level.Lines.ToString();
        }
    }
}