using UnityEngine;
using Cinemachine;

namespace BoardGame.Core
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