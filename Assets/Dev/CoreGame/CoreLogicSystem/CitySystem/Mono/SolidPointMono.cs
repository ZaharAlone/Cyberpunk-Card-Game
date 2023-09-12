using UnityEngine;
namespace CyberNet.Core.City
{
    public class SolidPointMono : MonoBehaviour
    {
        public bool StartIsNeutralSolid;
        public int Index;
        public string GUID;
        public SphereCollider Collider;

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
            Collider = gameObject.GetComponent<SphereCollider>();
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