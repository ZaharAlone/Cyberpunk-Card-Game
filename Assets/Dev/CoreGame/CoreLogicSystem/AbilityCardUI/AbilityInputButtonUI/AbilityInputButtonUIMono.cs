using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard.UI
{   
    /// <summary>
    /// Контролирует включение/выключения кнопки отмены действия карты в коре
    /// </summary>
    public class AbilityInputButtonUIMono : MonoBehaviour
    {
        [Header("Global panel")]
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private RectTransform _panelButtons;
        
        [Header("Settings view")]
        [SerializeField]
        private GameObject _headerGO;
        [SerializeField]
        private TextMeshProUGUI _headerText;
        
        [SerializeField]
        private GameObject _cancelButtonGO;
        [SerializeField]
        private TextMeshProUGUI _cancelText;

        [SerializeField]
        private GameObject _confirmButtonGO;
        [SerializeField]
        private TextMeshProUGUI _confirmText;

        [Header("Position Y block button")]
        [SerializeField]
        private float _defaultPositionY = 290;
        [SerializeField]
        private float _upPositionY = 360;
        
        public void Awake()
        {
            _panel.SetActive(false);
        }

        public void EnableButton()
        {
            _panel.SetActive(true);
        }

        public void SetView(string headerText, string cancelText, string confirmText, bool isShowButtonUp = false)
        {
            _headerGO.SetActive(headerText != "");
            _headerText.text = headerText;

            _cancelButtonGO.SetActive(cancelText != "");
            _cancelText.text = cancelText;
            
            _confirmButtonGO.SetActive(confirmText != "");
            _confirmText.text = confirmText;

            var positionPanelButton = _panelButtons.anchoredPosition;
            if (isShowButtonUp)
                positionPanelButton.y = _upPositionY;
            else
                positionPanelButton.y = _defaultPositionY;

            _panelButtons.anchoredPosition = positionPanelButton;
        }

        public void DisableButton()
        {
            _panel.SetActive(false);
        }
    }
}