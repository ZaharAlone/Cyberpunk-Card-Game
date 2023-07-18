using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Meta.Leaders
{
    public class SelectLeadersUIMono : MonoBehaviour
    {
        [Header("Global UI")]
        public GameObject Background;
        public GameObject Panel;
        
        [Header("Select Leaders Frame")]
        public Image SelectLeadersImageCard;
        public Localize SelectLeadersNameLoc;
        public Localize SelectLeadersDescrLoc;

        public void OpenSelectLeadersUI()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }
        
        public void SetSelectViewLeaders(Sprite imageCard, string name, string descr)
        {
            SelectLeadersImageCard.sprite = imageCard;
            SelectLeadersNameLoc.Term = name;
            SelectLeadersDescrLoc.Term = descr;
        }
    }
}