using CyberNet.Meta;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    public class AbilitySelectElementUIMono : MonoBehaviour
    {
        public GameObject Panel;
        
        public Localize HeaderText;
        public Localize DescrText;
        
        public InteractiveButtonHideShowElement CancelButton;
        public InteractiveButtonHideShowElement ConfimButton;

        public LocalizedString NoneLocButton;
        
        public void OpenWindow(bool isEnableCancelButton)
        {
            ConfimButton.SetLocalizeTerm(NoneLocButton);
            CancelButton.gameObject.SetActive(isEnableCancelButton);
            
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Panel.SetActive(false);
        }

        public void SetView(string header, string descr)
        {
            HeaderText.Term = header;
            DescrText.Term = descr;
        }

        public void SetTextButtonConfirm(string buttonText)
        {
            ConfimButton.SetText(buttonText);
        }

        public void OnClickCancelButton()
        {
            AbilitySelectElementAction.CancelSelect?.Invoke();
        }

        public void OnClickConfimButton()
        {
            AbilitySelectElementAction.ConfimSelect?.Invoke();
        }
    }
}