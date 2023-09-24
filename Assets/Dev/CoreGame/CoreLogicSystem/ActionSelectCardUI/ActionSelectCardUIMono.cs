using CyberNet.Meta;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    public class ActionSelectCardUIMono : MonoBehaviour
    {
        public GameObject Background;
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
            
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Background.SetActive(false);
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
            ActionSelectCardAction.CloseWindowAbilitySelectCard?.Invoke();
        }

        public void OnClickConfimButton()
        {
            ActionSelectCardAction.SelectCard?.Invoke();
        }
    }
}