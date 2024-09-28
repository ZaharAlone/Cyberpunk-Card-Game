using UnityEngine;

namespace CyberNet.Core.Map
{
    public class UnitZoneMono : MonoBehaviour
    {
        public bool StartIsNeutralSolid;
        public int Index;
        public string GUIDTower;
        public BoxCollider Collider;
        
        public void SetIndex(int index)
        {
            Index = index;
        }

        public void SetGUIDTower(string guid)
        {
            GUIDTower = guid;
        }

        public void GetCollider()
        {
            Collider = gameObject.GetComponent<BoxCollider>();
        }
    }
}