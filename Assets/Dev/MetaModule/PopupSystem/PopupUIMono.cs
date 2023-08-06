using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace CyberNet.Meta
{
    public class PopupUIMono : MonoBehaviour
    {
        public GameObject BackgroundImage;
        public GameObject PopupWaitGO;
        public PopupWaitUIMono PopupWaitClass;

        public GameObject PopupWarningGO;
        public PopupWarningUIMono PopupWarningClass;

        public PopupConfirmUIMono PopupConfirmUIMono;

        public void OpenWaitPopup(string header)
        {
            BackgroundImage.SetActive(true);
            PopupWaitGO.SetActive(true);
            PopupWaitClass.OpenPopup(header);
        }

        public void OpenWarningPopup(string header, string descr, string button, Action action)
        {
            BackgroundImage.SetActive(true);
            PopupWarningGO.SetActive(true);
            PopupWarningClass.OpenPopup(header, descr, button, action);
        }
        
        public void CloseWaitPopup()
        {
            BackgroundImage.SetActive(false);
            PopupWaitGO.SetActive(false);
        }

        public void CloseWarningPopup()
        {
            BackgroundImage.SetActive(false);
            PopupWarningGO.SetActive(false);
        }
    }
}