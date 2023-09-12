using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.City
{
    public class ConnectPointMono : MonoBehaviour
    {
        public CountPlayerEnum ActiveOnCountPlayer;
        public ConnectPointStruct PreviousPoint;
        public ConnectPointStruct NextPoint;
        
        [Space]
        public SolidPointMono SolidPointMono;
        public string GUID;

        public void GetSolid()
        {
            SolidPointMono = transform.GetChild(0).GetComponent<SolidPointMono>();
            SolidPointMono.SetGUID(GUID);
            SolidPointMono.SetIndex(0);
            SolidPointMono.GetCollider();
        }
        
        public void ActivateSolidPointCollider()
        {
            SolidPointMono.ActivateCollider();
        }

        public void DeactivateSolidPointCollider()
        {
            SolidPointMono.DeactivateCollider();
        }
        
        #if UNITY_EDITOR
        [Button("Preview - Tower, Next - Connect")]
        public void SetTypePrevieTowerNextConnect()
        {
            PreviousPoint.TypeCityPoint = TypeCityPoint.Tower;
            NextPoint.TypeCityPoint = TypeCityPoint.ConnectPoint;
        }
        
        [Button("Preview - Connect, Next - Tower")]
        public void SetTypePrevieConnectNextTower()
        {
            PreviousPoint.TypeCityPoint = TypeCityPoint.ConnectPoint;
            NextPoint.TypeCityPoint = TypeCityPoint.Tower;
        }
        
        [Button("All Tower")]
        public void SetAllConnectPointTypeTower()
        {
            PreviousPoint.TypeCityPoint = TypeCityPoint.Tower;
            NextPoint.TypeCityPoint = TypeCityPoint.Tower;
        }
        
        [Button("All Connect")]
        public void SetAllConnectPointTypeConnect()
        {
            PreviousPoint.TypeCityPoint = TypeCityPoint.ConnectPoint;
            NextPoint.TypeCityPoint = TypeCityPoint.ConnectPoint;
        }
        #endif
    }

    public enum TypeCityPoint
    {
        Tower,
        ConnectPoint,
        None
    }

    [Serializable]
    public struct ConnectPointStruct
    {
        public TypeCityPoint TypeCityPoint;
        [ShowIf("TypeCityPoint", TypeCityPoint.Tower)]
        public TowerMono TowerPoint;
        [ShowIf("TypeCityPoint", TypeCityPoint.ConnectPoint)]
        public ConnectPointMono ConnectPoint;
    }
}