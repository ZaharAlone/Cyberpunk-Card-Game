using EcsCore;
using UnityEngine;
using Cinemachine;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardGameCameraSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            _dataWorld.TrySelectFirst<BoardGameCameraComponent>(out var component);

            component.BoardGameCamera = component.Camera.GetComponent<BoardGameCamera>();
            component.MainCamera = component.BoardGameCamera.MainCamera;
            component.CoreVirtualCamera = component.BoardGameCamera.CoreVirtualCamera;
        }

        private void ShakeCamera(float amplitude, float time)
        {
            _dataWorld.TrySelectFirst<BoardGameCameraComponent>(out var component);

            var cinemachineShake = component.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = amplitude;
            component.ShakeCamera = true;
            component.ShakeTime = time;
        }

        private void OffShakeCamera()
        {
            _dataWorld.TrySelectFirst<BoardGameCameraComponent>(out var component);

            var cinemachineShake = component.CoreVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineShake.m_AmplitudeGain = 0f;
            component.ShakeCamera = false;
        }
    }
}