using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.City
{
    public class TowerMono : MonoBehaviour
    {
        public string GUID;
        public string Key = "tower_1";
        public bool IsFirstBasePlayer;

        public MeshRenderer VisualEffectZone;
        public CountPlayerInGameEnum ActiveOnCountPlayerInGame;
        public GameObject CloseInteractiveZoneEffect;
        public Collider ColliderTower;

        public List<UnitZoneMono> SquadZonesMono = new List<UnitZoneMono>();
        public List<TowerMono> ZoneConnect = new List<TowerMono>();

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

                ColliderTower = gameObject.GetComponent<Collider>();
                if (ColliderTower == null)
                    Debug.LogError($"Collider is null go name {gameObject.name}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception Get Solid Point tower name: {gameObject.name}");
            }
        }

        public void DeactivateCollider()
        {
            ColliderTower.enabled = false;
        }


        public void ActivateCollider()
        {
            ColliderTower.enabled = true;
        }
        
        public void CloseInteractiveZoneVisualEffect()
        {
            CloseInteractiveZoneEffect.SetActive(true);
        }
        
        public void OpenInteractiveZoneVisualEffect()
        {
            CloseInteractiveZoneEffect.SetActive(false);
        }
    }
}