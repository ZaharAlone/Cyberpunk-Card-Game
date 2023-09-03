using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.CoreGame.City
{
    public class TowerMono : MonoBehaviour
    {
        public string GUID;
        public bool IsFirstBasePlayer;
        public CountPlayerEnum ActiveOnCountPlayer;
        public List<SolidPointMono> SolidPoints = new List<SolidPointMono>();

        public void GetAllSolidPoint()
        {
            try
            {
                var counter = 0;
                SolidPoints.Clear();
                foreach (Transform child in transform)
                {
                    var solidPoint = child.GetComponent<SolidPointMono>();
                    if (solidPoint == null)
                        continue;
                    solidPoint.SetIndex(counter);
                    solidPoint.SetGUID(GUID);
                    SolidPoints.Add(solidPoint);
                    counter++;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception Get Solid Point tower name: {gameObject.name}");
            }
        }
    }
}