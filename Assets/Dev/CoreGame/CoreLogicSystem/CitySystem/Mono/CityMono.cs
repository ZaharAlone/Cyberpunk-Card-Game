using System.Collections.Generic;
using CyberNet.Tools;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.CoreGame.City
{
    public class CityMono : MonoBehaviour
    {
        
        public Transform InteractiveObjectContainer;
        public GameObject SolidConteiner;
        public List<TowerMono> Towers = new List<TowerMono>();
        public List<ConnectPointMono> ConnectPoints = new List<ConnectPointMono>();

        #if UNITY_EDITOR
        [Button("Заполнить данные в листах")]
        public void SetAllData()
        {
            Towers.Clear();
            ConnectPoints.Clear();
            
            var counterConnect = 0;
            var counterChild = 0;
            foreach (Transform child in InteractiveObjectContainer)
            {
                if (child.GetComponent<TowerMono>())
                {
                    var tower = child.GetComponent<TowerMono>();
                    tower.GUID = CreateGUID.Create();
                    tower.GetAllSolidPoint();
                    Towers.Add(tower);
                }

                if (child.GetComponent<ConnectPointMono>())
                {
                    var connectPoint = child.GetComponent<ConnectPointMono>();
                    connectPoint.GUID = CreateGUID.Create();
                    connectPoint.GetSolid();
                    ConnectPoints.Add(connectPoint);
                    counterConnect++;
                }

                counterChild++;
            }
        }
#endif
    }
}