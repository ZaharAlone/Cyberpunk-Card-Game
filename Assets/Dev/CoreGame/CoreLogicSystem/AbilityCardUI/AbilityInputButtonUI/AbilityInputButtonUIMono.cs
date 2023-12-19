using TMPro;
using UnityEngine;

namespace CyberNet.Core.AbilityCard.UI
{   
    /// <summary>
    /// Контролирует включение/выключения кнопки отмены действия карты в коре
    /// </summary>
    public class AbilityInputButtonUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;

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
        
        public void Awake()
        {
            _panel.SetActive(false);
        }

        public void EnableButton()
        {
            _panel.SetActive(true);
        }

        public void SetView(string headerText, string cancelText, string confirmText)
        {
            _headerGO.SetActive(headerText != "");
            _headerText.text = headerText;

            _cancelButtonGO.SetActive(cancelText != "");
            _cancelText.text = cancelText;
            
            _confirmButtonGO.SetActive(confirmText != "");
            _confirmText.text = confirmText;
        }

        public void DisableButton()
        {
            _panel.SetActive(false);
        }
    }
}