using I2.Loc;
using System;
using UnityEngine;

namespace CyberNet.Meta
{
    public class PopupWaitUIMono : MonoBehaviour
    {
        public Localize Header;

        public void OpenPopup(string header)
        {
            Header.Term = header;
        }
    }
}