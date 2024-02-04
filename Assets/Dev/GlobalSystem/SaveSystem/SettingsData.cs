using System;
using CyberNet.Meta.Settings;

namespace CyberNet.SaveSystem
{
    [Serializable]
    public struct SettingsData
    {
        public string Language;
        public bool IsShowDistrickPopup;
        public AudioSettingsConfig AudioSettings;
    }
}