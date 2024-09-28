using System.Collections.Generic;
using CyberNet.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Map
{
    public class CityMono : MonoBehaviour
    {
        public Transform InteractiveObjectContainer;
        public GameObject SolidContainer;
        [FormerlySerializedAs("Towers")]
        public List<DistrictMono> Disctrict = new List<DistrictMono>();
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
            Disctrict.Clear();
            
            var counterChild = 0;
            foreach (Transform child in InteractiveObjectContainer)
            {
                if (child.GetComponent<DistrictMono>())
                {
                    var tower = child.GetComponent<DistrictMono>();
                    tower.GUID = CreateGUID.Create();
                    tower.GetAllSquadZone();
                    Disctrict.Add(tower);
                }
                
                counterChild++;
            }
        }
        #endif
    }
}