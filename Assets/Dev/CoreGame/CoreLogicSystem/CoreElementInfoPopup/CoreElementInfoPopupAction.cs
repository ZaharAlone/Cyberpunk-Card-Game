using System;
using UnityEngine;

namespace CyberNet.Core.UI.CorePopup
{
    public static class CoreElementInfoPopupAction
    {
        public static Action<string, bool> OpenPopupCard;
        public static Action<RectTransform, string> OpenPopupButton;
        public static Action ClosePopupCard;
    }
}