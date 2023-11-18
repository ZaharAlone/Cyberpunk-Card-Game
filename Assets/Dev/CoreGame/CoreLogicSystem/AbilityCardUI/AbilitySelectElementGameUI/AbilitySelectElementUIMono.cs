using I2.Loc;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    public class AbilitySelectElementUIMono : MonoBehaviour
    {
        public GameObject Panel;
        
        public Localize HeaderText;
        public Localize DescrText;
        
        public void OpenWindow(string header, string descr)
        {
            HeaderText.Term = header;
            DescrText.Term = descr;
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Panel.SetActive(false);
        }
    }
}