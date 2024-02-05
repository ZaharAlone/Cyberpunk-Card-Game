using System;

namespace CyberNet.Global.Settings
{
    [Serializable]
    public struct AudioSettingsConfig
    {
        public int MasterVolume;
        public int MusicVolume;
        public int SFXVolume;
    }
}