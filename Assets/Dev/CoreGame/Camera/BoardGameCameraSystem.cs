using EcsCore;
using UnityEngine;
using Cinemachine;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardGameCameraSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ref var camera = ref _dataWorld.OneData<BoardGameCameraComponent>();
            camera.BoardGameCamera = camera.Camera.GetComponent<BoardGameCamera>();
            camera.MainCamera = camera.BoardGameCamera.MainCamera;
            camera.CoreVirtualCamera = camera.BoardGameCamera.CoreVirtualCamera;
        }

        private void ShakeCamera(float amplitude, float time)
        {
            ref var camera = ref _dataWorld.OneData<BoardGameCameraComponent>();
            var cinemachineShake = camera.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = amplitude;
            camera.ShakeCamera = true;
            camera.ShakeTime = time;
        }

        private void OffShakeCamera()
        {
            ref var camera = ref _dataWorld.OneData<BoardGameCameraComponent>();
            var cinemachineShake = camera.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = 0f;
            camera.ShakeCamera = false;
        }
    }
}