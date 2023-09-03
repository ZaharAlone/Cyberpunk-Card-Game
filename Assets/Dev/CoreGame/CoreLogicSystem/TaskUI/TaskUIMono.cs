using I2.Loc;
using UnityEngine;
namespace CyberNet.CoreGame.TaskUI
{
    public class TaskUIMono : MonoBehaviour
    {
        public GameObject TaskPanel;
        public Localize LocTextTask;

        public void OpenPopup()
        {
            TaskPanel.SetActive(true);
        }
        
        public void ClosePopup()
        {
            TaskPanel.SetActive(false);
        }

        public void SetText(string locTerm)
        {
            LocTextTask.mTerm = locTerm;
        }
    }
}