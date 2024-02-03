#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace CyberNet.Core.City
{
    public class IconsUnitInMapMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _vfxSelectUnit;
        [SerializeField]
        private BoxCollider _collider;
        [SerializeField]
        private string _unitGUID;
        [SerializeField]
        private SpriteRenderer _iconsUnit;
        
        public void OnEnable()
        {
            _vfxSelectUnit.SetActive(false);
        }

        public void SetColorUnit(Color32 color)
        {
            _iconsUnit.color = color;
        }

        public void OnSelectUnitEffect()
        {
            _vfxSelectUnit.SetActive(true);
        }

        public void OffSelectUnitEffect()
        {
            _vfxSelectUnit.SetActive(false);
        }

        public void SetGUID(string guid)
        {
            _unitGUID = guid;
        }

        public string GetGUID()
        {
            return _unitGUID;
        }
        
        public void ActivateCollider()
        {
            _collider.enabled = true;
        }
        
        public void DeactivateCollider()
        {
            _collider.enabled = false;
        }
        
        #if UNITY_EDITOR
        [Button]
        public void GetCollider()
        {
            _collider = gameObject.GetComponent<BoxCollider>();
        }
        #endif
    }
}