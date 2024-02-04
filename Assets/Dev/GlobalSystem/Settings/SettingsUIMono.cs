using System;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Meta.Settings
{
    public class SettingsUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private GameObject _background;
        
        [Header("Property")]
        [SerializeField]
        private Toggle _toglleDistrictPopup;

        public void Awake()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void OpenWindow()
        {
            _panel.SetActive(true);
            _background.SetActive(true);
        }
        
        public void CloseWindow()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }
        
        public void SetShowDistrictPopup(bool value)
        {
            SettingsUIAction.SetShowDistrictPopup?.Invoke(value);
        }

        public void SetViewDistrict(bool value)
        {
            _toglleDistrictPopup.isOn = value;
        }

        public void OnClickBack()
        {
            SettingsUIAction.CloseSettingsUI?.Invoke();
        }
    }
}