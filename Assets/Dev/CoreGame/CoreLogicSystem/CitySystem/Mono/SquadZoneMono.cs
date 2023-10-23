using UnityEngine;
namespace CyberNet.Core.City
{
    public class SquadZoneMono : MonoBehaviour
    {
        public bool StartIsNeutralSolid;
        public int Index;
        public string GUID;
        public BoxCollider Collider;

        public GameObject PointVFX;

        public void SetIndex(int index)
        {
            Index = index;
        }

        public void SetGUID(string guid)
        {
            GUID = guid;
        }

        public void GetCollider()
        {
            Collider = gameObject.GetComponent<BoxCollider>();
        }

        public void ActivateCollider()
        {
            Collider.enabled = true;
        }
        
        public void DeactivateCollider()
        {
            Collider.enabled = false;
        }
    }
}