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
        public TextMeshProUGUI ButtonTextLeft;
        public TextMeshProUGUI ButtonTextRight;

        //Element for animations
        
        private Action ActionConfim;

        public void OpenPopup(PopupConfimStruct popupData)
        {
            Debug.Log(popupData.HeaderLoc);
            Header.text = popupData.HeaderLoc;
            Descr.text = popupData.DescrLoc;
            ButtonTextRight.text = popupData.ButtonConfimLoc;
            ButtonTextLeft.text = popupData.ButtonCancelLoc;
            ActionConfim = popupData.ButtonConfimAction;
            
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
        }

        public void ClosePopupAnimation()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }
    }
}