using UnityEngine;
using UnityEngine.UI;

public class MuteSetting : MonoBehaviour
{
    [SerializeField] private Sprite _soundTexture;
    [SerializeField] private Sprite _muteTexture;
    
    private Toggle _toggle;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = LoadSoundMode().Equals(0);
    }

    public void SwapTexture()
    {
        _toggle.image.sprite = _toggle.isOn ? _muteTexture : _soundTexture;
    }
    
    public void SetSoundMode()
    {
        AudioListener.volume = !_toggle.isOn ? 1 : 0;
        SaveSoundMode();
    }

    private void SaveSoundMode()
    {
        PlayerPrefs.SetFloat("SoundMode", AudioListener.volume);
    }

    private float LoadSoundMode()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("SoundMode");
        return AudioListener.volume;
    }
}