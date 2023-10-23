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
        public Collider ColliderTower;

        public List<SquadZoneMono> SquadZonesMono = new List<SquadZoneMono>();

        public void GetAllSquadZone()
        {
            try
            {
                var counter = 0;
                foreach (Transform child in transform)
                {
                    var squadZone = child.GetComponent<SquadZoneMono>();
                    if (squadZone == null)
                        continue;
                    
                    squadZone.SetIndex(counter);
                    squadZone.SetGUID(GUID);
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
    }
}