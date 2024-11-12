#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Map
{
    public class IconsContainerUnitInMapMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _vfxSelectUnit;
        [SerializeField]
        private BoxCollider _collider;
        [SerializeField]
        private string _unitGUID;
        [SerializeField]
        private SpriteRenderer _iconsUnit;

        [Header("VFX Kill unit")]
        [SerializeField]
        [Required]
        private ParticleSystem _vfxKillUnit;
        [SerializeField]
        [Required]
        private GameObject _iconsKillUnit;
        
        public void OnEnable()
        {
            _vfxSelectUnit.SetActive(false);
            _iconsKillUnit.SetActive(false);
        }

        public void SetViewUnit(Sprite iconsUnit, Color32 colorUnit)
        {
            _iconsUnit.gameObject.SetActive(true);
            _iconsUnit.sprite = iconsUnit;
            _iconsUnit.color = colorUnit;
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

        public void PlayVFXKillUnitInMap()
        {
            _iconsUnit.gameObject.SetActive(false);
            _iconsKillUnit.SetActive(true);
            _vfxKillUnit.Play();
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