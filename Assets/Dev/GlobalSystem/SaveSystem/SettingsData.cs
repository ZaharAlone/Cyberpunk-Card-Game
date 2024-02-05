using System;
using CyberNet.Global.Settings;

namespace CyberNet.SaveSystem
{
    [Serializable]
    public struct SettingsData
    {
        public GameSettingsConfig GameSettings;
        public VideoSettingsConfig VideoSettings;
        public ControlsSettingsConfig ControlsSettings;
        public AudioSettingsConfig AudioSettings;
    }
}