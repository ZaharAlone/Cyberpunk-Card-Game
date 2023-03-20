using I2.Loc;
using System;
using UnityEngine;

namespace BoardGame.Meta
{
    public class PopupWarningUIMono : MonoBehaviour
    {
        public Localize Header;
        public Localize Descr;
        public Localize ButtonText;
        private Action TargetAction;

        public void OpenPopup(string header, string descr, string button, Action action)
        {
            Header.Term = header;
            Descr.Term = descr;
            ButtonText.Term = button;
            TargetAction = action;
        }

        public void OnClick()
        {
            TargetAction?.Invoke();
        }
    }
}