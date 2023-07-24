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
    public class BoardGameCameraSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ref var camera = ref _dataWorld.OneData<BoardGameCameraComponent>();
            camera.BoardGameCamera = camera.Camera.GetComponent<BoardGameCamera>();
            camera.MainCamera = camera.BoardGameCamera.MainCamera;
            camera.CoreVirtualCamera = camera.BoardGameCamera.CoreVirtualCamera;

            BoardGameCameraEvent.GetDamageCameraShake += GetDamage;
        }

        public void Run()
        {
            ref var camera = ref _dataWorld.OneData<BoardGameCameraComponent>();
            
            if (camera.ShakeCamera)
            {
                if (camera.ShakeTime > 0)
                    camera.ShakeTime -= Time.deltaTime;
                else
                    OffShakeCamera();
            }
        }

        private void GetDamage()
        {
            ShakeCamera(3f, 0.1f);
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