using UnityEngine;
namespace CyberNet.Core.City
{
    public class SolidPointMono : MonoBehaviour
    {
        public bool StartIsNeutralSolid;
        public int Index;
        public string GUID;

        public void SetIndex(int index)
        {
            Index = index;
        }

        public void SetGUID(string guid)
        {
            GUID = guid;
        }
    }
}