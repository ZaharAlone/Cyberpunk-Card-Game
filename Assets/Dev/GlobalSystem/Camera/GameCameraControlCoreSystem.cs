using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core;
using CyberNet.Core.Arena;
using CyberNet.Tutorial;
using Input;

namespace CyberNet.Global.GameCamera
{
    [EcsSystem(typeof(CoreModule))]
    public class GameCameraControlCoreSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private float MinZoomCamera = 0;
        private float MaxZoomCamera = 150;
        
        private float MinFOVCamera = 25;
        private float MaxFOVCamera = 15;
        
        private float _cameraZoomMoveSpeed = 0.20f;
        private float _movementTimeZoom = 4.5f;

        private Vector3 _newPosition;
        private Vector3 _newZoom;
        private Quaternion _newRotateCamera;
        
        private Vector3 _zoomAmount = new Vector3(0, -50, 0);
        private float _cameraMoveSpeed = 0.14f;
        private float _cameraFastMoveSpeed = 0.35f;
        private float _cameraMoveSpeedEndScreen = 0.07f;
        private float _movementTime = 3;
        private float edgeTolerance = 0.05f;
        private Vector3 startDrag;
        private float _timerStartMove;
        private Vector2 _startDragPosition;

        private Vector2 _horizontalClampMove = new Vector2(-20, 20);
        private Vector2 _verticalClampMove = new Vector2(-30, 20);
        
        public void PreInit()
        {
            GlobalCoreAction.FinishInitGameResource += ActivateCoreCamera;
            ModuleAction.DeactivateCoreModule += DeactivateCoreCamera;
            
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            _newPosition = camera.CoreCameraRig.position;
            _newZoom = camera.CoreCameraTransform.localPosition;
            _newRotateCamera = camera.CoreCameraTransform.rotation;
        }

        private void ActivateCoreCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
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
            var isStopCameraInput = _dataWorld.Select<BlockCameraInputComponent>().Count() != 0;
            if (isStopCameraInput)
                return;
            
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.CurrentGameStateMapVSArena == GameStateMapVSArena.Arena)
                return;
            
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            
            if (camera.IsCore)
                CheckInput();
        }

        private void CheckInput()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();

            if (inputData.Navigate != Vector2.zero)
            {
                ReadInputMoveKeyboard();
            }

            if (inputData.RightClickDown || inputData.MiddleClickDown)
            {
                _startDragPosition = inputData.MousePosition;
            }

            if (inputData.RightClickHold || inputData.MiddleClickHold)
            {
                DragCamera();
            }
            
            //Read mouse Move
            MoveCamera();
            
            if (inputData.ScrollWheel.y != 0f || inputData.ZoomAdd || inputData.ZoomSub)
            {
                var valueZoom = Mathf.Sign(inputData.ScrollWheel.y);

                if (inputData.ZoomAdd)
                    valueZoom = 10 * Time.deltaTime;
                if (inputData.ZoomSub)
                    valueZoom = -10 * Time.deltaTime;
                
                ZoomCameraReadInput(valueZoom);
            }
            
            ZoomCamera();
        }

        private void ReadInputMoveKeyboard()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            
            var cameraSpeed = _cameraMoveSpeed;
            if (inputData.FastMoveCamera)
                cameraSpeed = _cameraFastMoveSpeed;
            
            _newPosition += new Vector3(inputData.Navigate.x * cameraSpeed, 0, inputData.Navigate.y * cameraSpeed);
        }
        
        //GD вопрос, подумать и решить нужно или нет
        private void CheckMouseAtScreenEdge()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            var mousePosition = inputData.MousePosition;
            var moveDirection = Vector3.zero;

            if (mousePosition.x < edgeTolerance * Screen.width)
            {
                moveDirection += new Vector3(-1, 0, 0);
            }
            else if (mousePosition.x > (1f - edgeTolerance) * Screen.width)
            {
                moveDirection += new Vector3(1, 0, 0);
            }

            if (mousePosition.y < edgeTolerance * Screen.height)
            {
                moveDirection += new Vector3(0, 0, -1);
            }
            else if (mousePosition.y > (1f - edgeTolerance) * Screen.height)
            {
                moveDirection += new Vector3(0, 0, 1);
            }
            
            _newPosition += moveDirection * _cameraMoveSpeedEndScreen;
        }

        private void MoveCamera()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            ClampCamera();
            camera.CoreCameraRig.position = Vector3.Lerp(camera.CoreCameraRig.position, _newPosition, Time.deltaTime * _movementTime);
        }

        private void ZoomCamera()
        {
            var camera = _dataWorld.OneData<GameCameraData>();
            camera.CoreCameraTransform.localPosition = Vector3.Lerp(camera.CoreCameraTransform.localPosition, _newZoom, Time.deltaTime * _movementTimeZoom);

            var valueDistanceCamera = Mathf.InverseLerp(MinZoomCamera, MaxZoomCamera, camera.CoreCameraTransform.localPosition.y);
            var angle = Mathf.Lerp(75f, 90f, valueDistanceCamera);
            var cameraRotate = camera.CoreCameraTransform.transform;
            _newRotateCamera = Quaternion.Euler(angle, cameraRotate.rotation.y, cameraRotate.rotation.z);
            cameraRotate.rotation = Quaternion.Lerp(camera.CoreCameraTransform.rotation, _newRotateCamera, Time.deltaTime * _movementTimeZoom);
            
            camera.CoreVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(MinFOVCamera, MaxFOVCamera, valueDistanceCamera);
        }

        private void ZoomCameraReadInput(float zoomValue)
        {
            _newZoom += new Vector3(_zoomAmount.x * zoomValue, _zoomAmount.y * zoomValue, _zoomAmount.z * zoomValue) * _cameraZoomMoveSpeed;

            _newZoom = new Vector3(Mathf.Clamp(_newZoom.x, MinZoomCamera, MaxZoomCamera),
                Mathf.Clamp(_newZoom.y, MinZoomCamera, MaxZoomCamera),
                Mathf.Clamp(_newZoom.z, MinZoomCamera, MaxZoomCamera));
        }

        private void DragCamera()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            var vectorMove = inputData.MousePosition - _startDragPosition;
            
            if (vectorMove == Vector2.zero)
                return;
            
            _newPosition += new Vector3(-vectorMove.x * 0.025f, 0, -vectorMove.y * 0.025f);
            _startDragPosition = inputData.MousePosition;
        }
        
        private void ClampCamera()
        {
            var newX = Mathf.Clamp(_newPosition.x, _horizontalClampMove.x, _horizontalClampMove.y);
            var newZ = Mathf.Clamp(_newPosition.z, _verticalClampMove.x, _verticalClampMove.y);

            _newPosition = new Vector3(newX, _newPosition.y, newZ);
        }
        
        public void Destroy()
        {
            GlobalCoreAction.FinishInitGameResource -= ActivateCoreCamera;
            ModuleAction.DeactivateCoreModule -= DeactivateCoreCamera;
        }
    }
}