using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core;
using Input;

namespace CyberNet.Global.GameCamera
{
    [EcsSystem(typeof(GlobalModule))]
    public class GameCameraControlCoreSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        private float MinZoomCamera = 25;
        private float MaxZoomCamera = 55;
        
        public void PreInit()
        {
            GlobalCoreAction.FinishInitGameResource += ActivateCoreCamera;
            GlobalCoreAction.StartFocusCameraArena += StartFocusCameraArena;
            GlobalCoreAction.FinishFocusCameraArena += FinishFocusCameraArena;
            ModuleAction.DeactivateCoreModule += DeactivateCoreCamera;
        }

        private void ActivateCoreCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            ref var cityData = ref _dataWorld.OneData<CityData>();
            camera.CoreVirtualCamera.m_Follow = cityData.CityGO.transform;
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
            followOffset.x += moveVector.x / 15f;
            followOffset.z += moveVector.y / 15f;
            camera.CoreCinemachineTransposer.m_FollowOffset = followOffset;
        }

        private void MoveCameraMouse(Vector2 mousePosition)
        {

        }

        private void ZoomCamera(float zoomValue)
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var value = camera.CoreVirtualCamera.m_Lens.FieldOfView - 3 * Mathf.Sign(zoomValue);
            value = Mathf.Clamp(value, MinZoomCamera, MaxZoomCamera);
            camera.CoreVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(camera.CoreVirtualCamera.m_Lens.FieldOfView, value, Time.deltaTime * 50f);
            var angle = Mathf.Lerp(65f, 90f, Mathf.InverseLerp(MinZoomCamera, MaxZoomCamera, value));
            var cameraRotate = camera.CoreVirtualCamera.transform.rotation;
            camera.CoreVirtualCamera.transform.rotation = Quaternion.Euler(angle, cameraRotate.y, cameraRotate.z);
        }
        
        private void StartFocusCameraArena()
        {
            /*
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var value = camera.CoreVirtualCamera.m_Lens.FieldOfView - 3 * Mathf.Sign(zoomValue);
            value = Mathf.Clamp(value, MinZoomCamera, MaxZoomCamera);
            camera.CoreVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(camera.CoreVirtualCamera.m_Lens.FieldOfView, value, Time.deltaTime * 50f);
            var angle = Mathf.Lerp(65f, 90f, Mathf.InverseLerp(MinZoomCamera, MaxZoomCamera, value));
            var cameraRotate = camera.CoreVirtualCamera.transform.rotation;
            camera.CoreVirtualCamera.transform.rotation = Quaternion.Euler(angle, cameraRotate.y, cameraRotate.z); */
        }
        
        private void FinishFocusCameraArena()
        {
            
        }
    }
}