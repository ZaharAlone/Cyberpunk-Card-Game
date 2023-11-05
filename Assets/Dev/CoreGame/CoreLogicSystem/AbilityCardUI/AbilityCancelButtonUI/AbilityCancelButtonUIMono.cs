using UnityEngine;

namespace CyberNet.Core.AbilityCard.UI
{   
    /// <summary>
    /// Контролирует включение/выключения кнопки отмены действия карты в коре
    /// </summary>
    public class AbilityCancelButtonUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;

        public void Awake()
        {
            _panel.SetActive(false);
        }

        public void EnableButton()
        {
            _panel.SetActive(true);
        }

        public void DisableButton()
        {
            _panel.SetActive(false);
        }
    }
}