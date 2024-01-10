using UnityEngine;
namespace CyberNet.Core.PauseUI
{
    public class PauseGameUIMono : MonoBehaviour
    {
        public GameObject Background;
        public GameObject Panel;

        public void OpenWindow()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }

        public void HidePanelMenu()
        {
            Panel.SetActive(false);
        }

        public void ShowPanelMenu()
        {
            Panel.SetActive(true);
        }
        
        public void OnClickResumeGame()
        {
            PauseGameAction.ResumeGame?.Invoke();
        }

        public void OnClickSettings()
        {
            PauseGameAction.SettingsGame?.Invoke();
        }

        public void OnClickReturnMenu()
        {
            PauseGameAction.ReturnMenu?.Invoke();
        }

        public void OnClickQuitGame()
        {
            PauseGameAction.QuitGame?.Invoke();
        }
    }
}