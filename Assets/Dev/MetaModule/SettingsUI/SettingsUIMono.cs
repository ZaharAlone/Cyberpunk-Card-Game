using System;
using UnityEngine;
namespace CyberNet.Meta.SettingsUI
{
    public class SettingsUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private GameObject _background;

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

        public void OnClickBack()
        {
            SettingsUIAction.CloseSettingsUI?.Invoke();
        }
    }
}