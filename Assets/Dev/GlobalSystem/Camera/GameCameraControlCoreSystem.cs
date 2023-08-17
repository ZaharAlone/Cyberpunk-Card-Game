using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using Cinemachine;
using CyberNet.Core;
using Input;

namespace CyberNet.Global.GameCamera
{
    [EcsSystem(typeof(GlobalModule))]
    public class GameCameraControlCoreSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        private float MinZoomCamera = 20;
        private float MaxZoomCamera = 40
            ;

        public void PreInit()
        {
            GlobalCoreAction.FinishInitGameResource += ActivateCoreCamera;
            ModuleAction.DeactivateCoreModule += DeactivateCoreCamera;
        }
        
        private void ActivateCoreCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            ref var boardResourceData = ref _dataWorld.OneData<BoardGameResourceData>();
            camera.CoreVirtualCamera.m_Follow = boardResourceData.CityGO.transform;
            camera.MetaVirtualCamera.gameObject.SetActive(false);
            camera.CoreVirtualCamera.gameObject.SetActive(true);
            camera.IsCore = true;
        }

        private void DeactivateCoreCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            camera.MetaVirtualCamera.gameObject.SetActive(true);
            camera.CoreVirtualCamera.gameObject.SetActive(false);
            camera.IsCore = false;
        }
        
        public void Run()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            
            if (camera.IsCore)
                CheckInput();
        }
        
        private void CheckInput()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            
            if (inputData.Navigate != Vector2.zero)
                MoveCameraKeyboard(inputData.Navigate);

            if (inputData.LeftClickHold)
                MoveCameraMouse(inputData.MousePosition);
            
            if (inputData.ScrollWheel.y != 0f)
                ZoomCamera(inputData.ScrollWheel.y);
        }

        private void MoveCameraKeyboard(Vector2 moveVector)
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var followOffset = camera.CoreCinemachineTransposer.m_FollowOffset;
            followOffset.x += moveVector.x * 0.5f;
            followOffset.z += moveVector.y * 0.5f;
            camera.CoreCinemachineTransposer.m_FollowOffset = followOffset;
        }

        private void MoveCameraMouse(Vector2 mousePosition)
        {

        }

        private void ZoomCamera(float zoomValue)
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var value = camera.CoreVirtualCamera.m_Lens.FieldOfView - zoomValue * 0.25f;
            value = Mathf.Clamp(value, MinZoomCamera, MaxZoomCamera); 
            camera.CoreVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(camera.CoreVirtualCamera.m_Lens.FieldOfView, value, Time.deltaTime * 50f);
        }
    }
}