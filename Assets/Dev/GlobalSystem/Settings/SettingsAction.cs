using System;

namespace CyberNet.Global.Settings
{
    public static class SettingsAction
    {
        //OpenBaseUI
        public static Action OpenSettingsUI;
        public static Action CloseSettingsUI;

        //UI control
        public static Action OpenGameTab;
        public static Action OpenVideoTab;
        public static Action OpenAudioTab;
        public static Action OpenControlsTab;
        public static Action OpenCreditsTab;
        
        //GAME
        public static Action<bool> SetShowDistrictPopup;
        public static Action<bool> SetShowConfirmEndTurnPopup;

        //AUDIO
        public static Action<int> SetMasterVolume;
        public static Action<int> SetMusicVolume;
        public static Action<int> SetSFXVolume;
    }
}