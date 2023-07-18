using UnityEngine;
namespace CyberNet.Meta
{
    public class OnlineGameUIMono : MonoBehaviour
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

        public void OnClickBackMainMenu()
        {
            OnlineGameUIAction.BackMainMenu?.Invoke();
        }

        public void OnClickFindMatch()
        {
            OnlineGameUIAction.FindMatch?.Invoke();
        }

        public void OnClickCallFriend()
        {
            OnlineGameUIAction.CallFriend?.Invoke();
        }
    }
}