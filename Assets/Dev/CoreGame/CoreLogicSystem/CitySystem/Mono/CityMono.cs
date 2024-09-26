using System.Collections.Generic;
using CyberNet.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Map
{
    public class CityMono : MonoBehaviour
    {
        public Transform InteractiveObjectContainer;
        public GameObject SolidContainer;
        public List<TowerMono> Towers = new List<TowerMono>();
        [SerializeField]
        private GameObject _cityLight;

        public void OnCityLight()
        {
            _cityLight.SetActive(true);
        }
        
        public void OffCityLight()
        {
            _cityLight.SetActive(false);
        }
        
        #if UNITY_EDITOR
        [Button("Заполнить данные в листах")]
        public void SetAllData()
        {
            Towers.Clear();
            
            var counterChild = 0;
            foreach (Transform child in InteractiveObjectContainer)
            {
                if (child.GetComponent<TowerMono>())
                {
                    var tower = child.GetComponent<TowerMono>();
                    tower.GUID = CreateGUID.Create();
                    tower.GetAllSquadZone();
                    Towers.Add(tower);
                }
                
                counterChild++;
            }
        }
        #endif
    }
}