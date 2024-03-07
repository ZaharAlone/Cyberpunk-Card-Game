using System.Threading.Tasks;
using CyberNet.Core.City;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class UnitArenaMono : MonoBehaviour
    {
        public string GUID;
        public UnitPointVFXMono UnitPointVFXMono;
        [SerializeField]
        private CapsuleCollider _unitCollider;
        
        [SerializeField]
        private Animator _animator;

        [Header("Unit weapon")]
        [SerializeField]
        private UnitWeaponMono _gun;
        [SerializeField]
        private UnitWeaponMono _granade;
        
        [SerializeField]
        private GameObject _shield;

        private bool isShoot;

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

        public void ShowTargetUnit(Transform transform)
        {
            transform.LookAt(transform.position);
        }

        public async void Shooting()
        {
            _animator.SetTrigger("Shoot");
            
            _gun.PlayVFXAttack();
            await Task.Delay(1500);
            _animator.SetTrigger("Idle");
            ArenaAction.ArenaUnitFinishAttack?.Invoke();
            _gun.StopVFXAttack();
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