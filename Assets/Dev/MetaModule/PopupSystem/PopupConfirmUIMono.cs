using I2.Loc;
using System;
using UnityEngine;

namespace CyberNet.Meta
{
    public class PopupConfirmUIMono : MonoBehaviour
    {
        public Localize Header;
        public Localize Descr;
        public Localize ButtonTextLeft;
        public Localize ButtonTextRight;
        private Action TargetAction;

        public void OpenPopup(string header, string descr, string buttonRight, string buttonLeft, Action action)
        {
            Header.Term = header;
            Descr.Term = descr;
            ButtonTextRight.Term = buttonRight;
            ButtonTextLeft.Term = buttonLeft;
            TargetAction = action;
        }

        public void OnClick()
        {
            TargetAction?.Invoke();
        }
    }
}