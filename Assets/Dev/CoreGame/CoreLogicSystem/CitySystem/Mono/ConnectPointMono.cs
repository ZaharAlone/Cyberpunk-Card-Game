using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public class ConnectPointMono : MonoBehaviour
    {
        [FormerlySerializedAs("ActiveOnCountPlayer")]
        public CountPlayerInGameEnum activeOnCountPlayerInGame;
        public ConnectPointStruct PreviousPoint;
        public ConnectPointStruct NextPoint;
        
        [FormerlySerializedAs("SolidPointMono")]
        [Space]
        public SquadPointMono squadPointMono;
        public string GUID;

        public void GetSolid()
        {
            squadPointMono = transform.GetChild(0).GetComponent<SquadPointMono>();
            squadPointMono.SetGUID(GUID);
            squadPointMono.SetIndex(0);
            squadPointMono.GetCollider();
        }
        
        public void ActivateSolidPointCollider()
        {
            squadPointMono.ActivateCollider();
        }

        public void DeactivateSolidPointCollider()
        {
            squadPointMono.DeactivateCollider();
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