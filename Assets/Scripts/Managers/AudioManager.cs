using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audio;

    [SerializeField] private AudioClip[] _clips;
    public AudioClip[] Clips => _clips;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        Stop();
        _audio.clip = clip;
        _audio.Play();
    }

    public void Stop()
    {
        _audio.Stop();
    }
}