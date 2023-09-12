using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public struct ConnectPointComponent
    {
        public string GUID;
        public GameObject ConnectPointGO;
        public ConnectPointMono ConnectPointMono;
        public SolidPointMono SolidPointMono;
        public List<ConnectPointTypeGUID> ConnectPointsTypeGUID;
    }
    
    [Serializable]
    public struct ConnectPointTypeGUID
    {
        public TypeCityPoint TypeCityPoint;
        public string GUIDPoint;
    }
}