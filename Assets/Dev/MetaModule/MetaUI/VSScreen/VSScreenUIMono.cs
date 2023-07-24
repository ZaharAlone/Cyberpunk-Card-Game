using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Meta
{
    public class VSScreenUIMono : MonoBehaviour
    {
        public GameObject Background;
        public GameObject Panel;
        
        public Image LeaderLeft;
        public Image LeaderRight;

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

        public void SetLeader(Sprite leaderLeft, Sprite leaderRight)
        {
            LeaderLeft.sprite = leaderLeft;
            LeaderRight.sprite = leaderRight;
        }
    }
}