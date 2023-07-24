using UnityEngine;

namespace CyberNet.Meta
{
    public class CampaignUIMono : MonoBehaviour
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
        
        public void OnClickContinueGame()
        {
            CampaignUIAction.ContinueCampaign?.Invoke();
        }

        public void OnClickNewGame()
        {
            CampaignUIAction.NewCampaign?.Invoke();
        }

        public void OnClickBackMainMenu()
        {
            CampaignUIAction.BackMainMenu?.Invoke();
        }
    }
}