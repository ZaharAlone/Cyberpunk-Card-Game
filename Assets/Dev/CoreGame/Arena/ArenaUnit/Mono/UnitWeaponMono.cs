using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class UnitWeaponMono : MonoBehaviour
    {
        [SerializeField]
        private bool _isEnableStart;
        [SerializeField]
        private GameObject _weaponGO;
        [SerializeField]
        private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
        [SerializeField]
        private ParticleSystem _vfxAttack;

        public void OnEnable()
        {
            _weaponGO.SetActive(_isEnableStart);
        }
        
        public void EnableWeaponEffect()
        {
            //Заглушка
            _weaponGO.SetActive(true);
        }

        public void DisableWeaponEffect()
        {
            //Заглушка
            _weaponGO.SetActive(false);
        }

        public void PlayVFXAttack()
        {
            _vfxAttack.Play();
        }

        public void StopVFXAttack()
        {
            _vfxAttack.Stop();
        }
    }
}