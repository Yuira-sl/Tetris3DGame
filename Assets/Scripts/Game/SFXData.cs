using UnityEngine;

namespace Octamino
{
    [CreateAssetMenu()]
    public class SFXData: ScriptableObject
    {
        public AudioClip PauseClip;
        public AudioClip ResumeClip;
        public AudioClip NewGameClip;
        public AudioClip PieceMoveClip;
        public AudioClip PieceRotateClip;
        public AudioClip PieceDropClip;
        public AudioClip CollectRowClip;
    }
}