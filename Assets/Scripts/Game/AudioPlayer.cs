using UnityEngine;

namespace Octamino
{
    public class AudioPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;

        public AudioClip PauseClip;
        public AudioClip ResumeClip;
        public AudioClip NewGameClip;
        public AudioClip PieceMoveClip;
        public AudioClip PieceRotateClip;
        public AudioClip PieceDropClip;

        public void PlayPauseClip() => _audioSource.PlayOneShot(PauseClip);
        public void PlayResumeClip() => _audioSource.PlayOneShot(ResumeClip);
        public void PlayNewGameClip() => _audioSource.PlayOneShot(NewGameClip);
        public void PlayPieceMoveClip() => _audioSource.PlayOneShot(PieceMoveClip);
        public void PlayPieceRotateClip() => _audioSource.PlayOneShot(PieceRotateClip);
        public void PlayPieceDropClip() => _audioSource.PlayOneShot(PieceDropClip);
        public void PlayToggleOnClip() => _audioSource.PlayOneShot(ResumeClip);
        public void PlayToggleOffClip() => _audioSource.PlayOneShot(PauseClip);

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    } 
}
