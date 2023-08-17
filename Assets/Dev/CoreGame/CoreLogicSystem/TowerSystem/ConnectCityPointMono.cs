using System;
using Sirenix.OdinInspector;
using UnityEngine;
namespace CyberNet.CoreGame.Tower
{
    public class ConnectCityPointMono : MonoBehaviour
    {
        public ConnectPointStruct PreviousPoint;
        public ConnectPointStruct NextPoint;
        public SolidPointMono SolidPointMono;
        public string GUID;

        public void GetSolid()
        {
            SolidPointMono = transform.GetChild(0).GetComponent<SolidPointMono>();
        }

        public void GetConnectPoints(Transform parentObject, int indexChild)
        {
            var countChild = parentObject.childCount;
            if (indexChild - 1 > 0)
            {
                PreviousPoint = GetPoint(parentObject.GetChild(indexChild - 1));
            }

            if (indexChild + 1 < countChild)
            {
                NextPoint = GetPoint(parentObject.GetChild(indexChild + 1));
            }
        }

        private ConnectPointStruct GetPoint(Transform target)
        {
            if (target.GetComponent<ConnectCityPointMono>())
            {
                var connectPoint = target.GetComponent<ConnectCityPointMono>();
                return new ConnectPointStruct {
                    TypeCityPoint = TypeCityPoint.ConnectPoint,
                    ConnectPoint = connectPoint
                };
            }
            else if (target.GetComponent<TowerMono>())
            {
                var tower = target.GetComponent<TowerMono>();
                return new ConnectPointStruct {
                    TypeCityPoint = TypeCityPoint.Tower,
                    TowerPoint = tower
                };
            }
            else
            {
                return new ConnectPointStruct();
            }
        }
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
        [HideIf("TypeCityPoint", Tower.TypeCityPoint.ConnectPoint)]
        public TowerMono TowerPoint;
        [HideIf("TypeCityPoint", Tower.TypeCityPoint.Tower)]
        public ConnectCityPointMono ConnectPoint;
    }
}