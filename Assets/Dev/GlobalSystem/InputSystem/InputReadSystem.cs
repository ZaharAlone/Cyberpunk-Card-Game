using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace Input
{
    [EcsSystem(typeof(GlobalModule))]
    public class InputReadSystem : IInitSystem, IRunSystem, IPostRunSystem
    {
        public enum StateInput
        {
            Game,
            Blocked
        }

        private DataWorld _dataWorld;
        private StateInput _stateInput;
        public PlayerController_Action Control;
        private IPostRunSystem _postRunSystemImplementation;

        public void Init()
        {
            Control = new PlayerController_Action();
            Control.Enable();
            _stateInput = StateInput.Game;

            UpdateTypeInput();
            SubscriptionCallback();
        }
        private void SubscriptionCallback()
        {
            Control.Game.Click.started += ClickDownCallback;
        }
        private void ClickDownCallback(UnityEngine.InputSystem.InputAction.CallbackContext callback)
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            inputData.ClickDown = true;
        }

        public void Run()
        {
            switch (_stateInput)
            {
                case StateInput.Game:
                    ReadInput();
                    break;
                case StateInput.Blocked:
                    BlockedInput();
                    break;
            }
        }

        private void UpdateTypeInput()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();

            if (inputData.PlayerInput.devices.Count < 1)
                return;

            var currentDeviceRawPath = inputData.PlayerInput.devices[0].ToString();

            if (inputData.CurrentController == currentDeviceRawPath)
                return;

            inputData.CurrentController = currentDeviceRawPath;

            if (inputData.PlayerInput.currentControlScheme == "Gamepad")
                inputData.CurrentControllerType = TypeController.Gamepad;
            else
                inputData.CurrentControllerType = TypeController.Keyboard;
        }

        private void ReadInput()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            inputData.MousePosition = Control.Game.MousePositions.ReadValue<Vector2>();
            inputData.RotateCamera = Control.Game.RotateCamera.ReadValue<Vector2>();
            inputData.Navigate = Control.Game.Navigate.ReadValue<Vector2>();
            inputData.ScrollWheel = Control.Game.ScrollWheel.ReadValue<Vector2>();
            inputData.Click = Control.Game.Click.triggered;
            inputData.LeftClickHold = Control.Game.LeftClickHold.IsPressed();
            inputData.RightClick = Control.Game.RightClick.triggered;
            inputData.ExitUI = Control.Game.Exit.triggered;
            inputData.Cancel = Control.Game.Cancel.triggered;
            inputData.Submit = Control.Game.Submit.triggered;
            
            if (inputData.Click)
                InputAction.LeftMouseButtonClick?.Invoke();
            if (inputData.RightClick)
                InputAction.RightMouseButtonClick?.Invoke();
        }

        public void PostRun()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            inputData.ClickDown = false;
        }
        
        private void BlockedInput()
        {

        }
    }
}