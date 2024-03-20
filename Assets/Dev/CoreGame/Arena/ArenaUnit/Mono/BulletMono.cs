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
            Debug.LogError($"Layer collision {other.gameObject.layer}");
            if (other.gameObject.layer == 7)
            {
                UnitArenaAction.BulletCollision?.Invoke(_guid);
            }
        }

        public void PlayEffectHit()
        {
            _vfxHit.SetActive(true);
            _bullet.SetActive(false);
        }
        
        public void DestroyBulletToTime()
        {
            Destroy(gameObject);
        }
    }
}