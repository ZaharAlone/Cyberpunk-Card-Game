using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class ReaderBulletCollisionMono : MonoBehaviour
    {
        private const string _unitLayer = "Unit";
        public void OnParticleCollision(GameObject collisionObject)
        {
            var collisionLayerObject = collisionObject.layer;

            if (collisionLayerObject == LayerMask.NameToLayer(_unitLayer))
            {
                UnitArenaAction.BulletCollisionUnit?.Invoke();
            }
            else
            {
                UnitArenaAction.BulletCollisionNotUnit?.Invoke();
            }
        }
    }
}