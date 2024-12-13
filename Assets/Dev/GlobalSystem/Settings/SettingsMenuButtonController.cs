using System;
using UnityEngine;

namespace CyberNet.Global.Settings
{
    public class SettingsMenuButtonController : MonoBehaviour
    {
        [SerializeField]
        private InteractiveButtonTabsSettings _gameButton;
        [SerializeField]
        private InteractiveButtonTabsSettings _videoButton;
        [SerializeField]
        private InteractiveButtonTabsSettings _audioButton;
        [SerializeField]
        private InteractiveButtonTabsSettings _controlsButton;
        [SerializeField]
        private InteractiveButtonTabsSettings _creditsButton;
        [SerializeField]
        private InteractiveButtonTabsSettings _backButton;

        public void OnEnable()
        {
            ForceDeactivateAllButtons();
            _gameButton.SetForceActivateButton();
        }

        public void OnClickGameButton()
        {
            DeactivateAllButtons();
            _gameButton.ActivateButtonAnimation();
            SettingsAction.OpenGameTab?.Invoke();
        }
        
        public void OnClickVideoButton()
        {
            DeactivateAllButtons();
            _videoButton.ActivateButtonAnimation();
            SettingsAction.OpenVideoTab?.Invoke();
        }
        
        public void OnClickAudioButton()
        {
            DeactivateAllButtons();
            _audioButton.ActivateButtonAnimation();
            SettingsAction.OpenAudioTab?.Invoke();
        }
        
        public void OnClickControlsButton()
        {
            DeactivateAllButtons();
            _controlsButton.ActivateButtonAnimation();
            SettingsAction.OpenControlsTab?.Invoke();
        }
        
        public void OnClickCreditsButton()
        {
            DeactivateAllButtons();
            _creditsButton.ActivateButtonAnimation();
            SettingsAction.OpenCreditsTab?.Invoke();
        }

        public void OnClickBackButton()
        {
            DeactivateAllButtons();
            _backButton.ActivateButtonAnimation();
        }
        
        public void DeactivateAllButtons()
        {
            _gameButton.DeactivateButtonAnimation();
            _videoButton.DeactivateButtonAnimation();
            _audioButton.DeactivateButtonAnimation();
            _controlsButton.DeactivateButtonAnimation();
            _creditsButton.DeactivateButtonAnimation();
            _backButton.DeactivateButtonAnimation();
        }
        
        public void ForceDeactivateAllButtons()
        {
            _gameButton.SetForceDeactivateButton();
            _videoButton.SetForceDeactivateButton();
            _audioButton.SetForceDeactivateButton();
            _controlsButton.SetForceDeactivateButton();
            _creditsButton.SetForceDeactivateButton();
            _backButton.SetForceDeactivateButton();
        }
    }
}