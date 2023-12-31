using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core;
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
        
        private float _cameraZoomMoveSpeed = 0.16f;
        private float _movementTimeZoom = 4.5f;

        private Vector3 _newPosition;
        private Vector3 _oldPosition;
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

        private Vector2 _horizontalMaxMove = new Vector2(-25, 25);
        private Vector2 _verticalMaxMove = new Vector2(-25, 25);
        
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
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.CurrentRoundState == RoundState.Arena)
                return;
            
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            
            if (camera.IsCore)
                CheckInput();
        }

        private void CheckInput()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();

            _oldPosition = _newPosition;
            if (inputData.Navigate != Vector2.zero)
            {
                ReadInputMoveKeyboard();
            }

            //CheckMouseAtScreenEdge();
            //DragCamera();
            //Read mouse Move
            MoveCamera();
            
            //if (inputData.LeftClickHold)
            //    ReadInputMouse(inputData.MousePosition);
            
            if (inputData.ScrollWheel.y != 0f)
                ZoomCameraReadInput(inputData.ScrollWheel.y);
            
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
            
            if (Physics.Raycast(camera.CoreCameraRig.position, _newPosition, out RaycastHit hit, 15))
            {
                Debug.DrawRay(camera.CoreCameraRig.position, _newPosition, Color.green);
                if (hit.collider.gameObject.layer == 6)
                {
                    Debug.LogError("stop camera");
                    _newPosition = camera.CoreCameraRig.position;
                    return;
                }
            }
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
            if (zoomValue > 0)
            {
                _newZoom += _zoomAmount * _cameraZoomMoveSpeed;
            }
            else
            {
                _newZoom -= _zoomAmount * _cameraZoomMoveSpeed;
            }

            _newZoom = new Vector3(Mathf.Clamp(_newZoom.x, MinZoomCamera, MaxZoomCamera),
                Mathf.Clamp(_newZoom.y, MinZoomCamera, MaxZoomCamera),
                Mathf.Clamp(_newZoom.z, MinZoomCamera, MaxZoomCamera));
/*
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var cameraRotate = camera.CoreVirtualCamera.transform.rotation;
            Debug.LogError($"Zoom value {zoomValue}");
            var value = 3 * Mathf.Sign(zoomValue);
            Debug.LogError($"Value camera {value}");
            value = Mathf.Clamp(value, MinZoomCamera, MaxZoomCamera);
            //camera.CoreVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(camera.CoreVirtualCamera.m_Lens.FieldOfView, value, Time.deltaTime * 50f);
            var angle = Mathf.Lerp(65f, 90f, Mathf.InverseLerp(MinZoomCamera, MaxZoomCamera, value));

            _newRotateCamera = Quaternion.Euler(angle, cameraRotate.y, cameraRotate.z);

            Debug.LogError($"new rotate {_newRotateCamera}");
  */ //camera.CoreVirtualCamera.transform.rotation = Quaternion.Euler(angle, cameraRotate.y, cameraRotate.z);
        }

        private void DragCamera()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();

            if (inputData.ClickDown)
            {
                Debug.LogError("Click down");
                _startDragPosition = inputData.MousePosition;
            }
            
            if (!inputData.Click)
            {
                Debug.LogError("Not click");
                return;
            }
            /*
            if (_timerStartMove < 0.1f)
            {
                _timerStartMove += Time.deltaTime;
                return;
            }*/

            var camera = _dataWorld.OneData<GameCameraData>();
            
            var pos = camera.MainCamera.ScreenToViewportPoint(inputData.MousePosition + _startDragPosition);
            _newPosition = new Vector3(pos.x, 0, pos.y);
            _startDragPosition = inputData.MousePosition;
        }

        public void Destroy()
        {
            GlobalCoreAction.FinishInitGameResource -= ActivateCoreCamera;
            ModuleAction.DeactivateCoreModule -= DeactivateCoreCamera;
        }
    }
}