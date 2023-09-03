using UnityEditor;
using UnityEngine;
namespace CyberNet.Meta
{
    public class MainMenuUIMono : MonoBehaviour
    {
        public GameObject Background;
        public GameObject Panel;
        
        public void OpenMainMenu()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }
        
        public void CloseMainMenu()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
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
    }
}