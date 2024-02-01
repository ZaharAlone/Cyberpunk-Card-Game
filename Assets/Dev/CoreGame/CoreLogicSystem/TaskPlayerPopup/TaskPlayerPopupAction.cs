using System;
namespace CyberNet.Core.UI.TaskPlayerPopup
{
    public static class TaskPlayerPopupAction
    {
        public static Action OpenPopupSelectFirstBase;
        public static Action HidePopup;

        public static Action<string, string> OpenPopup;
    }
}