using System;

namespace CyberNet.Meta.Settings
{
    public static class SettingsUIAction
    {
        public static Action OpenSettingsUI;
        public static Action CloseSettingsUI;

        public static Action<bool> SetShowDistrictPopup;
    }
}