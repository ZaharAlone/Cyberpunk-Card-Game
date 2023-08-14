using UnityEngine;
using Cinemachine;

namespace CyberNet.Global.GameCamera
{
    public class GameCameraMono : MonoBehaviour
    {
        public Camera MainCamera;
        public CinemachineVirtualCamera CoreVirtualCamera;
        public CinemachineVirtualCamera CoreVirtualCameraMaxScale;
        public CinemachineVirtualCamera MetaVirtualCamera;

        public Transform CoreTargetFollow;
    }
}