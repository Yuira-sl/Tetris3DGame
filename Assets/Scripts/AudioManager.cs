using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audio;

    [SerializeField] private AudioClip _gameOver;
    [SerializeField] private AudioClip _clearLevel;
    [SerializeField] private AudioClip _dropDown;
    public AudioClip GameOver => _gameOver;
    public AudioClip ClearLevel => _clearLevel;
    public AudioClip DropDown => _dropDown;
    

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        _audio.clip = clip;
        _audio.Play();
    }
}