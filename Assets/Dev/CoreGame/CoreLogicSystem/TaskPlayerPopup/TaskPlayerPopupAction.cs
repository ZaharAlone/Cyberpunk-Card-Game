using System;
namespace CyberNet.Core.UI.TaskPlayerPopup
{
    public static class TaskPlayerPopupAction
    {
        public static Action OpenPopupSelectFirstBase;
        public static Action ClosePopup;

        public static Action<string, string> OpenPopup;
        public static Action<string, string,string> OpenPopupParam;
    }
}