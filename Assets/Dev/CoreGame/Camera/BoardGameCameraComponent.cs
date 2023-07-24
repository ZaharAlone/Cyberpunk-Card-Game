using UnityEngine;
using Cinemachine;

namespace CyberNet.Core
{
    public struct BoardGameCameraComponent
    {
        public GameObject Camera;
        public BoardGameCamera BoardGameCamera;
        public CinemachineVirtualCamera CoreVirtualCamera;
        public Camera MainCamera;

        public bool ShakeCamera;
        public float ShakeTime;
    }
}