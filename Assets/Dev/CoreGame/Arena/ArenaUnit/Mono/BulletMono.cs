using CyberNet.Core.Arena.Bullet;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class BulletMono : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _speedMove = 1;

        [SerializeField]
        private GameObject _bullet;
        [SerializeField]
        private GameObject _vfxHit;
        
        private string _guid;
        
        public void SetGUID(string guid)
        {
            _guid = guid;
        }
        
        public void Move()
        {
            _rigidbody.velocity = transform.forward * _speedMove;
        }

        public void OnCollisionEnter(Collision other)
        {
            var layerCollision = other.gameObject.layer;
            var bulletCollision = new BulletCollisionStruct {
                GUID = _guid, LayerCollision = layerCollision,
            };
            UnitArenaAction.BulletCollision?.Invoke(bulletCollision);
        }

        public void PlayEffectHit()
        {
            _vfxHit.SetActive(true);
            _bullet.SetActive(false);
        }

        public void StopFlyBullet()
        {
            _rigidbody.velocity = Vector3.zero;
        }
        
        public void DestroyBulletToTime()
        {
            Destroy(gameObject);
        }
    }
}