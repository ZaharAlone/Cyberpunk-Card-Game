using UnityEngine;
using UnityEngine.UI;
namespace CyberNet.Core.WinLose
{
    public class WinLoseUIMono : MonoBehaviour
    {
        public GameObject Background;
        public GameObject Panel;

        public Image AvatarLose;
        public Image AvatarWin;

        public void OpenWindow(Sprite avatarWin, Sprite avatarLose)
        {
            AvatarWin.sprite = avatarWin;
            AvatarLose.sprite = avatarLose;

            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void OnClickExit()
        {
            WinLoseAction.CloseScreen?.Invoke();
        }
    }
}