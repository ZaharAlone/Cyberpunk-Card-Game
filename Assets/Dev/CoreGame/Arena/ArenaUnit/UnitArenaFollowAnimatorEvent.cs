using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena
{
    public class UnitArenaFollowAnimatorEvent : MonoBehaviour
    {
        [SerializeField]
        private UnitArenaMono _unitArenaMono;
        
        public void animationEvent_shot()
        {
            _unitArenaMono.ShootingAnimationsEvent();
        }
    }   
}
