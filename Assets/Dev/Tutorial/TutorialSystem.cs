using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.Dialog;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global.Analytics;
using CyberNet.Tutorial.UI;
using Input;

namespace CyberNet.Tutorial
{
    [EcsSystem(typeof(TutorialGameModule))]
    public class TutorialSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private bool _checkMoveCameraInput;
        private bool _followMoveCameraMouse;
        private bool _followMoveCameraKeyboard;
        private bool _checkZoomCameraInput;
        private bool _followZoomCamera;
        
        public void PreInit()
        {
            TutorialAction.StartFirstPlayerRound += StartTutorialPlayerRound;
        }

        public void Run()
        {
            if (_checkMoveCameraInput)
            {
                CheckMoveCamera();
            }
            
            if (_checkZoomCameraInput)
            {
                CheckZoomCamera();
            }
        }

        private void CheckMoveCamera()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            if (inputData.Navigate != Vector2.zero)
            {
                _followMoveCameraKeyboard = true;
            }
            else
            {
                if (_followMoveCameraKeyboard)
                    MoveCameraComplete();
            }

            
            if (inputData.RightClickHold || inputData.MiddleClickHold)
            {
                _followMoveCameraMouse = true;
            }
            else
            {
                if (_followMoveCameraMouse)
                    MoveCameraComplete();
            }
        }

        private void CheckZoomCamera()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            if (inputData.ScrollWheel.y != 0f || inputData.ZoomAdd || inputData.ZoomSub)
            {
                _followZoomCamera = true;
            }
            else
            {
                if (_followZoomCamera)
                    ZoomCameraComplete();
            }
        }

        private async void StartTutorialPlayerRound()
        {
            AnalyticsEvent.StartProgressEvent?.Invoke("start_tutorial");
            Debug.LogError("Start tutorial player Round");
            var uiRound = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ChangeRoundUI;
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            uiRound.NewRoundView(playerViewComponent.Avatar, playerViewComponent.Name);
            await Task.Delay(1500);
            
            DialogAction.StartDialog?.Invoke("tutorial_start_intro");
            DialogAction.EndDialog += FinishIntroDialog;
        }
        
        private void FinishIntroDialog()
        {
            DialogAction.EndDialog -= FinishIntroDialog;
            ShowZoomCamera();
        }

        private async void ShowZoomCamera()
        {
            await Task.Delay(500);
            TutorialUIAction.OpenPopupZoomCamera?.Invoke();
            _checkZoomCameraInput = true;
        }

        private async void ZoomCameraComplete()
        {
            _checkZoomCameraInput = false;
            TutorialUIAction.ClosePopup?.Invoke();

            await Task.Delay(500);
            TutorialUIAction.OpenPopupMoveCamera?.Invoke();
            _checkMoveCameraInput = true;
        }
        
        private async void MoveCameraComplete()
        {
            _checkMoveCameraInput = false;
            TutorialUIAction.ClosePopup?.Invoke();
            await Task.Delay(500);
            DialogAction.StartDialog?.Invoke("tutorial_end_move_camera");
            DialogAction.EndDialog += StartSelectFirstBase;
        }

        private void StartSelectFirstBase()
        {
            DialogAction.EndDialog -= StartSelectFirstBase;
            SelectFirstBaseAction.CheckInstallFirstBase.Invoke();
        }
        
        public void Destroy()
        {
            
        }
    }
}