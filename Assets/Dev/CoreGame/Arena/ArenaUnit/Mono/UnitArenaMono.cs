using CyberNet.Core.City;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class UnitArenaMono : MonoBehaviour
    {
        public string GUID;
        public UnitPointVFXMono UnitPointVFXMono;
        [SerializeField]
        private Collider _unitCollider;
        
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Transform _unitTransform;

        [Header("Unit weapon")]
        [SerializeField]
        private UnitWeaponMono _gun;
        [SerializeField]
        private UnitWeaponMono _granade;
        
        [SerializeField]
        private GameObject _shield;

        private bool isShoot;
        
        private const string shooting_animations_trigger = "Shoot";
        private const string aim_animations_trigger = "Aim";
        private const string idle_animations_trigger = "Idle";
        private const string hit_animations_trigger = "Hit";

        public void OnEnable()
        {
            OffShield();
        }

        public void PlayAnimation()
        {
            
        }

        public void EnableCollider()
        {
            _unitCollider.enabled = true;
        }
        
        public void DisableCollider()
        {
            _unitCollider.enabled = false;
        }

        public void ViewToTargetUnit(Transform transform)
        {
            _unitTransform.LookAt(transform.position);
            var newRotation = _unitTransform.eulerAngles;
            newRotation.y += 7.5f;
            _unitTransform.eulerAngles = newRotation;
        }

        public void OnAimAnimations()
        {
            _animator.SetTrigger(aim_animations_trigger);
        }

        public void StartShootingAnimations()
        {
            _animator.SetTrigger(shooting_animations_trigger);
        }
        
        public void ShootingGunPlayVFX()
        {
            _gun.PlayVFXAttack();
        }

        public void HitAnimations()
        {
            Debug.LogError("Animations play hit");
            _animator.SetTrigger(hit_animations_trigger);
        }

        public BulletMono ShootingCreateBullet()
        {
            var bullet = _gun.ShootingGunCreateBullet();
            return bullet;
        }

        public void OnShield()
        {
            _shield.SetActive(true);
        }

        public void OffShield()
        {
            _shield.SetActive(false);
        }

        public void ShootingAnimationsEvent()
        {
            
        }
    }
}