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
        ConnectPoint
    }

    [Serializable]
    public struct ConnectPointStruct
    {
        public TypeCityPoint TypeCityPoint;
        [HideIf("TypeCityPoint", TypeCityPoint.ConnectPoint)]
        public TowerMono TowerPoint;
        [HideIf("TypeCityPoint", TypeCityPoint.Tower)]
        public ConnectPointMono ConnectPoint;
    }
}