using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Octamino
{
    public class SettingsView : MonoBehaviour
    {
        private UnityAction OnCloseCallback;

        public Text TitleText;
        public Toggle MusicToggle;
        public Button CloseButton;
        public AudioPlayer AudioPlayer;
        
        public void Show(UnityAction onCloseCallback)
        {
            OnCloseCallback = onCloseCallback;
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            TitleText.text = Constant.Text.Settings;

            MusicToggle.isOn = Settings.MusicEnabled;
            MusicToggle.GetComponentInChildren<Text>().text = Constant.Text.Music;
            MusicToggle.onValueChanged.AddListener(enabled =>
            {
                Settings.MusicEnabled = enabled;
                PlayToggleAudioClip(enabled);
            });
            
            CloseButton.onClick.AddListener(() =>
            {
                Hide();
                OnCloseCallback.Invoke();
            });

            CloseButton.gameObject.GetComponent<PointerHandler>()
                .OnPointerDown.AddListener(() => { AudioPlayer.PlayResumeClip(); });

            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        private void PlayToggleAudioClip(bool enabled)
        {
            if (enabled)
            {
                AudioPlayer.PlayToggleOnClip();
            }
            else
            {
                AudioPlayer.PlayToggleOffClip();
            }
        }
    }
}