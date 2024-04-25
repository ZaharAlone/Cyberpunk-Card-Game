using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class UnitArenaFollowAnimatorEvent : MonoBehaviour
    {
        public void EventShoot()
        {
            UnitArenaAction.GunShootingVFX?.Invoke();
        }

        public void EventFinishShoot()
        {
            UnitArenaAction.EndShootingAnimations?.Invoke();
        }
        
        public void EventStartGranade()
        {
            
        }

        public void EventThrowGranade()
        {
            
        }
    }
}