using System;
using UnityEngine;
using UnityEngine.UI;

namespace Octamino
{
    public class SpriteSwapper: MonoBehaviour
    {
        private bool _isInitialized;
        private Image _image;
        
        [SerializeField] private Sprite _source;
        [SerializeField] private Sprite _target;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
        
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
            _image.sprite = _target;
        }

        private void OnResumed()
        {
            _image.sprite = _source;
        }

        public void SwapTextures()
        {
            _image.sprite = GetComponent<Toggle>().isOn ? _target : _source;
        }

        private void OnDestroy()
        {
            Game.Instance.OnPaused -= OnPaused;
            Game.Instance.OnResumed -= OnResumed;
        }
    }
}