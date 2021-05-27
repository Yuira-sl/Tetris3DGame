using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class LevelView : MonoBehaviour
    {
        public Text Level;
        public Text Lines;

        private void Update()
        {
            Level.text = Game.Instance.Level.Number.ToString();
            Lines.text = Game.Instance.Level.Lines.ToString();
        }
    }
}