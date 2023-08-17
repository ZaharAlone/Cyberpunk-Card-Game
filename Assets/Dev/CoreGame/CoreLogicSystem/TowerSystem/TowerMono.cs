using System.Collections.Generic;
using UnityEngine;
namespace CyberNet.CoreGame.Tower
{
    public class TowerMono : MonoBehaviour
    {
        public string GUID;
        public bool CanFirstBasePlayer;
        public Dictionary<int, SolidPointMono> SolidPointDict = new Dictionary<int, SolidPointMono>();

        public void GetAllSolidPoint()
        {
            var counter = 0;
            foreach (Transform child in transform)
            {
                var solidPoint = child.GetComponent<SolidPointMono>();
                solidPoint.SetIndex(counter);
                SolidPointDict.Add(counter, solidPoint);
                counter++;
            }
        }
    }
}