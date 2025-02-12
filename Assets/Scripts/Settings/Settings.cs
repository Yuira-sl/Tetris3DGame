﻿using UnityEngine;

namespace Octamino
{
    public static class Settings
    {
        private static readonly string MusicEnabledKey = "settings.musicEnabled";
        
        public delegate void SettingsDelegate();
        public static SettingsDelegate ChangedEvent = delegate { };

        public static bool MusicEnabled
        {
            get => PlayerPrefs.GetInt(MusicEnabledKey, 1).BoolValue();
            set
            {
                PlayerPrefs.SetInt(MusicEnabledKey, value.IntValue());
                PlayerPrefs.Save();
                ChangedEvent.Invoke();
            }
        }
    }
}
