using I2.Loc;
using System;
using TMPro;
using UnityEngine;

namespace CyberNet.Meta
{
    public class PopupConfirmUIMono : MonoBehaviour
    {
        [Header("Main Element")]
        public GameObject Background;
        public GameObject Panel;
        
        [Header("View element")]
        public TextMeshProUGUI Header;
        public TextMeshProUGUI Descr;
        public InteractiveButtonHideShowElement ButtonLeft;
        public InteractiveButtonHideShowElement ButtonRight;

        //Element for animations
        
        private Action ActionConfim;
        private Action ActionCancel;

        public void Awake()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }

        public void OpenPopup(PopupConfimStruct popupData)
        {
            Debug.Log(popupData.HeaderLoc);
            Header.text = popupData.HeaderLoc;
            Descr.text = popupData.DescrLoc;
            ButtonRight.SetText(popupData.ButtonConfimLoc);
            ButtonLeft.SetText(popupData.ButtonCancelLoc);
            ActionConfim = popupData.ButtonConfimAction;
            ActionCancel = popupData.ButtonCancelAction;
            OpenPopupAnimations();
        }

        private void OpenPopupAnimations()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void OnConfimClick()
        {
            ActionConfim?.Invoke();
        }

        public void OnClickCancel()
        {
            ClosePopupAnimation();
            ActionCancel?.Invoke();
        }

        public void ClosePopupAnimation()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }
    }
}