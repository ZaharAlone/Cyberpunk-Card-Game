using System.Collections.Generic;
using CyberNet.Core.City;
using UnityEngine.Serialization;

namespace CyberNet.Core.AI
{
    public struct BuildFreeSlotStruct
    {
        public int CountFreeSlot;
        public string GUID;
        public List<UnitZoneMono> SolidPointsMono;
    }
}