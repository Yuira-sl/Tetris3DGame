using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class SpriteSwapper: MonoBehaviour
    {
        private bool _isInitialized;
        
        [SerializeField] private Sprite _source;
        [SerializeField] private Sprite _target;

        private void Update()
        {
            if(!_isInitialized)
            {
                Game.Instance.OnPaused += OnPaused;
                Game.Instance.OnResumed += OnResumed;

                _isInitialized = true;
            }
        }

        private void OnPaused()
        {
            GetComponent<Image>().sprite = _target;
        }

        private void OnResumed()
        {
            GetComponent<Image>().sprite = _source;
        }

        public void SwapTextures()
        {
            GetComponent<Image>().sprite = GetComponent<Toggle>().isOn ? _target : _source;
        }

        private void OnDestroy()
        {
            Game.Instance.OnPaused -= OnPaused;
            Game.Instance.OnResumed -= OnResumed;
        }
    }
}