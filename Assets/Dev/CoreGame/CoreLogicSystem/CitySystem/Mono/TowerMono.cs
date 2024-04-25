using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public class TowerMono : MonoBehaviour
    {
        public string GUID;
        public string Key = "tower_1";
        public bool IsFirstBasePlayer;
        public CountPlayerInGameEnum ActiveOnCountPlayerInGame;
        
        public MeshRenderer VisualEffectZone;
        [SerializeField]
        private GameObject _closeInteractiveZoneEffect;
        [SerializeField]
        private GameObject _openInteractiveZoneEffect;
        [SerializeField]
        private Collider _colliderTower;

        public List<UnitZoneMono> SquadZonesMono = new List<UnitZoneMono>();
        public List<TowerMono> ZoneConnect = new List<TowerMono>();

        [SerializeField]
        private GameObject _iceZone;

        [HideInInspector]
        public bool IsInteractiveTower;

        public void GetAllSquadZone()
        {
            try
            {
                SquadZonesMono.Clear();
                var counter = 0;
                
                foreach (Transform child in transform)
                {
                    var squadZone = child.GetComponent<UnitZoneMono>();
                    if (squadZone == null)
                        continue;
                    
                    squadZone.SetIndex(counter);
                    squadZone.SetGUIDTower(GUID);
                    squadZone.GetCollider();
                    SquadZonesMono.Add(squadZone);
                    counter++;
                }

                _colliderTower = gameObject.GetComponent<Collider>();
                if (_colliderTower == null)
                    Debug.LogError($"Collider is null go name {gameObject.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception Get Solid Point tower name: {gameObject.name}");
            }
        }

        public void OffInteractiveTower()
        {
            IsInteractiveTower = false;
        }
        
        public void OnInteractiveTower()
        {
            IsInteractiveTower = true;
        }
        
        public void CloseInteractiveZoneVisualEffect()
        {
            _closeInteractiveZoneEffect.SetActive(true);
            _openInteractiveZoneEffect.SetActive(false);
        }
        
        public void OpenInteractiveZoneVisualEffect()
        {
            _closeInteractiveZoneEffect.SetActive(false);
            _openInteractiveZoneEffect.SetActive(true);
        }

        public void EnableIceZone()
        {
            _iceZone.SetActive(true);
        }

        public void DisableIceZone()
        {
            _iceZone.SetActive(false);
        }
    }
}