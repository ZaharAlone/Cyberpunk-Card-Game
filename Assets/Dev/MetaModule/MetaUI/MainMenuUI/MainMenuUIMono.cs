using UnityEngine;

namespace CyberNet.Meta
{
    public class MainMenuUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _background;
        [SerializeField]
        private GameObject _panel;

        [Header("Element UI")]
        [SerializeField]
        private GameObject _tutorialButton;
        [SerializeField]
        private GameObject _campaignButton;
        [SerializeField]
        private GameObject _onlineGameButton;
        [SerializeField]
        private GameObject _wishlistButton;
        
        public void OpenMainMenu()
        {
            _background.SetActive(true);
            _panel.SetActive(true);
            
            #if DEMO
            _tutorialButton.SetActive(true);
            _campaignButton.SetActive(false);
            _onlineGameButton.SetActive(false);
            #else
            _tutorialButton.SetActive(false);
            _campaignButton.SetActive(true);
            _onlineGameButton.SetActive(true);
            #endif
            
            #if DEMO && STEAM
            _wishlistButton.SetActive(true);
            #else
            _wishlistButton.SetActive(false);
            #endif
        }
        
        public void CloseMainMenu()
        {
            _background.SetActive(false);
            _panel.SetActive(false);
        }

        public void OnClickTutorial()
        {
            MainMenuAction.StartTutorial?.Invoke();
        }
        
        public void OnClickCampaign()
        {
            MainMenuAction.OpenCampaign?.Invoke();
        }

        public void OnClickLocalGame()
        {
            MainMenuAction.OpenLocalGame?.Invoke();
        }

        public void OnClickServerGame()
        {
            MainMenuAction.OpenServerGame?.Invoke();
        }
        
        public void OnClickSettings()
        {
            MainMenuAction.OpenSettingsGame?.Invoke();
        }
        
        public void OnClickExitGame()
        {
            MainMenuAction.OpenExitGame?.Invoke();
        }
        
        public void OnClickAddWishlist()
        {
            Application.OpenURL("Steam://openurl/https://store.steampowered.com/app/1965780/");
        }
    }
}