using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace CyberNet.Global.GameCamera
{
    public struct GameCameraData
    {
        public GameObject CameraGO;
        public GameCameraMono GameCameraMono;
        public CinemachineVirtualCamera CoreVirtualCamera;
        public CinemachineTransposer CoreCinemachineTransposer;
        public Vector3 FollowOffsetCoreCamera;
        public CinemachineVirtualCamera MetaVirtualCamera;
        public Camera MainCamera;
        public bool IsCore;

        public bool ShakeCamera;
        public float ShakeTime;
    }
}