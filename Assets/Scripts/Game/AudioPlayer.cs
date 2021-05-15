using UnityEngine;

namespace Octamino
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        
        [SerializeField] private SFXData _sfxData;

        public void PlayPauseClip() => _audioSource.PlayOneShot(_sfxData.PauseClip);
        public void PlayResumeClip() => _audioSource.PlayOneShot(_sfxData.ResumeClip);
        public void PlayNewGameClip() => _audioSource.PlayOneShot(_sfxData.NewGameClip);
        public void PlayPieceMoveClip() => _audioSource.PlayOneShot(_sfxData.PieceMoveClip);
        public void PlayPieceRotateClip() => _audioSource.PlayOneShot(_sfxData.PieceRotateClip);
        public void PlayPieceDropClip() => _audioSource.PlayOneShot(_sfxData.PieceDropClip);
        public void PlayToggleOnClip() => _audioSource.PlayOneShot(_sfxData.ResumeClip);
        public void PlayToggleOffClip() => _audioSource.PlayOneShot(_sfxData.PauseClip);
        public void PlayCollectRowClip() => _audioSource.PlayOneShot(_sfxData.CollectRowClip);

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    } 
}
