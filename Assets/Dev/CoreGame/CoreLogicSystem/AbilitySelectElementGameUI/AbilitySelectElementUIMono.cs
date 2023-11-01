using CyberNet.Meta;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    public class AbilitySelectElementUIMono : MonoBehaviour
    {
        public GameObject Panel;
        public RectTransform PanelRectTransform;
        
        public Localize HeaderText;
        public Localize DescrText;
        
        public InteractiveButtonHideShowElement CancelButton;
        public InteractiveButtonHideShowElement ConfimButton;

        public LocalizedString NoneLocButton;
        public LocalizedString ConfimLocButton;

        [SerializeField]
        public float _standartYPositionForm;
        [SerializeField]
        public float _upYPositionForm;
        
        public void OpenWindow(bool isEnableCancelButton, bool defaultPosition = true)
        {
            ConfimButton.SetLocalizeTerm(NoneLocButton);
            CancelButton.gameObject.SetActive(isEnableCancelButton);

            var positionPanel = PanelRectTransform.anchoredPosition;
            if (defaultPosition)
                positionPanel.y = _standartYPositionForm;
            else
                positionPanel.y = _upYPositionForm;

            PanelRectTransform.anchoredPosition = positionPanel;
            
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Debug.LogError("close panel");
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
            Debug.LogError("On click confim button");
            AbilitySelectElementAction.ConfimSelect?.Invoke();
        }
    }
}