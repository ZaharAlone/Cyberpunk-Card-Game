using Animancer;
using CyberNet.Core.Map;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class UnitArenaMono : MonoBehaviour
    {
        public string GUID;
        public UnitPointVFXMono UnitPointVFXMono;
        [SerializeField]
        private Collider _unitCollider;
        
        [Required]
        [SerializeField]
        private AnimancerComponent _animancer;
        [Required]
        [SerializeField]
        private Transform _unitTransform;

        [Header("Unit weapon")]
        [Required]
        [SerializeField]
        private UnitWeaponMono _gun;
        [Required]
        [SerializeField]
        private UnitWeaponMono _granade;
        
        [SerializeField]
        private GameObject _shield;

        private bool isShoot;

        [Header("Animations")]
        [Required]
        [SerializeField]
        private AnimationClip _shoot_animations;
        [Required]
        [SerializeField]
        private AnimationClip _aim_animations;
        [Required]
        [SerializeField]
        private AnimationClip _idle_animations;
        [Required]
        [SerializeField]
        private AnimationClip _hit_animations;

        private const float _fadeAnimationsDurations = 0.35f;

        public void OnEnable()
        {
            _animancer.Play(_idle_animations);
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
            _animancer.Play(_aim_animations, _fadeAnimationsDurations);
        }

        public void OnShootingAnimations()
        {
            _animancer.Play(_shoot_animations, _fadeAnimationsDurations);
        }

        public void OnIdleAnimations()
        {
            _animancer.Play(_idle_animations, _fadeAnimationsDurations);
        }
        
        public void ShootingGunPlayVFX()
        {
            _gun.PlayVFXAttack();
        }

        public void HitAnimations()
        {
            _animancer.Play(_hit_animations);
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