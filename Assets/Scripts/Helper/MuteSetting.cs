using UnityEngine;
using UnityEngine.UI;

public class MuteSetting : MonoBehaviour
{
    private Toggle _toggle;
    
    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = LoadSoundMode().Equals(0);
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