using System;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Global.Settings
{
    public class SettingsUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private GameObject _background;
        
        [Header("Tabs")]
        [SerializeField]
        private GameObject _gameTab;
        [SerializeField]
        private GameObject _videoTab;
        [SerializeField]
        private GameObject _audioTab;
        [SerializeField]
        private GameObject _controlsTab;
        [SerializeField]
        private GameObject _creditsTab;

        [Header("Settings Mono")]
        [SerializeField]
        private GameSettingsUIMono _gameSettings;
        [SerializeField]
        private VideoSettingsUIMono _videoSettings;
        [SerializeField]
        private AudioSettingsUIMono _audioSettings;
        [SerializeField]
        private ControlsSettingsUIMono _controlsSettings;

        public void Awake()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void OpenWindow()
        {
            _panel.SetActive(true);
            _background.SetActive(true);
            SetActiveTab(_gameTab);
        }
        
        public void CloseWindow()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void OpenGameTab(GameSettingsConfig config)
        {
            _gameSettings.SetView(config);
            SetActiveTab(_gameTab);
        }
        
        public void OpenVideoTab(VideoSettingsConfig config)
        {
            SetActiveTab(_videoTab);
        }
        
        public void OpenAudioTab(AudioSettingsConfig config)
        {
            _audioSettings.SetView(config);
            SetActiveTab(_audioTab);
        }
        
        public void OpenControlsTab(ControlsSettingsConfig config)
        {
            SetActiveTab(_controlsTab);
        }
        
        public void OpenCreditsTab()
        {
            SetActiveTab(_creditsTab);
        }

        public void OnClickBack()
        {
            SettingsAction.CloseSettingsUI?.Invoke();
        }

        private void SetActiveTab(GameObject tab)
        {
            _gameTab.SetActive(false);
            _videoTab.SetActive(false);
            _audioTab.SetActive(false);
            _controlsTab.SetActive(false);
            _creditsTab.SetActive(false);
            
            tab.SetActive(true);
        }
    }
}