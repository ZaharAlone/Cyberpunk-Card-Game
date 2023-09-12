using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.City
{
    public class TowerMono : MonoBehaviour
    {
        public string GUID;
        public bool IsFirstBasePlayer;
        public CountPlayerEnum ActiveOnCountPlayer;
        public List<SolidPointMono> SolidPoints = new List<SolidPointMono>();
        public BoxCollider ColliderTower;

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
                    solidPoint.GetCollider();
                    SolidPoints.Add(solidPoint);
                    counter++;
                }

                ColliderTower = gameObject.GetComponent<BoxCollider>();
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

        public void ActivateSolidPointCollider()
        {
            foreach (var solidPoint in SolidPoints)
            {
                solidPoint.ActivateCollider();
            }
        }

        public void DeactivateSolidPointCollider()
        {
            foreach (var solidPoint in SolidPoints)
            {
                solidPoint.DeactivateCollider();
            }
        }

        public Vector3 GetColliderSize()
        {
            return ColliderTower.size;;
        }
    }
}