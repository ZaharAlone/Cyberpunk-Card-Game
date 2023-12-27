using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace CyberNet.Global.GameCamera
{
    public class GameCameraMono : MonoBehaviour
    {
        public Camera MainCamera;
        public Transform CoreCameraRig;
        public Transform CoreCameraTransform;
        public CinemachineVirtualCamera CoreVirtualCamera;
        public CinemachineVirtualCamera MetaVirtualCamera;
    }
}