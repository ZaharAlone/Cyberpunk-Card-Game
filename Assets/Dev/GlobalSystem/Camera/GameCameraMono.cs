using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace CyberNet.Global.GameCamera
{
    public class GameCameraMono : MonoBehaviour
    {
        public Camera MainCamera;
        public CinemachineVirtualCamera CoreVirtualCamera;
        public CinemachineVirtualCamera MetaVirtualCamera;
    }
}