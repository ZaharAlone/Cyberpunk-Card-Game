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

        [SerializeField]
        private Transform _shootingPoint;
        [SerializeField]
        private BulletMono _bullet;

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

        public BulletMono ShootingGunCreateBullet()
        {
            var bullet = Instantiate(_bullet);
            bullet.transform.position = _shootingPoint.position;
            bullet.transform.rotation = _shootingPoint.rotation;
            return bullet;
        }

        public void StopVFXAttack()
        {
            _vfxAttack.Stop();
        }
    }
}