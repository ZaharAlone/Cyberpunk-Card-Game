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

        public GameObject PopupConfirmGO;
        public PopupConfirmUIMono PopupConfirmClass;

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

        public void OpenConfirmPopup(string header, string descr, string buttonRight, string buttonLeft, Action action)
        {
            BackgroundImage.SetActive(true);
            PopupConfirmGO.SetActive(true);
            PopupConfirmClass.OpenPopup(header, descr, buttonRight, buttonLeft, action);
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

        public void CloseConfirmPopup()
        {
            BackgroundImage.SetActive(false);
            PopupConfirmGO.SetActive(false);
        }
    }
}