using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CyberNet.Meta
{
    public static class PopupAction
    {
        public static Action<string> WaitPopup;
        public static Action<string, string, string, Action> WarningPopup;
        public static Action<string, string, string, string, Action> ConfirmPopup;

        public static Action CloseWaitPopup;
        public static Action CloseWarningPopup;
        public static Action CloseConfirmPopup;
    }
}