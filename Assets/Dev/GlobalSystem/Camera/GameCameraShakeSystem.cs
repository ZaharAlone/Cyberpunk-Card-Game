using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using Cinemachine;

namespace CyberNet.Global.GameCamera
{
    /// <summary>
    /// Система отвечающая за тряску камеры, реализует различные варианты тряски в зависимости от ситуации
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class GameCameraShakeSystem : IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            GameCameraAction.GetDamageCameraShake += GetDamage;
        }

        public void Run()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            
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
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var cinemachineShake = camera.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = amplitude;
            camera.ShakeCamera = true;
            camera.ShakeTime = time;
        }

        private void OffShakeCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var cinemachineShake = camera.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = 0f;
            camera.ShakeCamera = false;
        }
    }
}