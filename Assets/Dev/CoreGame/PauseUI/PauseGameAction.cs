using System;
namespace CyberNet.Core.PauseUI
{
    public static class PauseGameAction
    {
        public static Action OnPauseGame;
        public static Action OffPauseGame;

        public static Action OpenUIPauseGame;
        public static Action CloseUIPauseGame;
        
        public static Action ResumeGame;
        public static Action SettingsGame;
        public static Action ReturnMenu;
        public static Action ConfimReturnMenu;
        public static Action QuitGame;
    }
}