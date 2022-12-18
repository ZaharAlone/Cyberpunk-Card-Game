using EcsCore;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;

namespace Input
{
    [EcsSystem(typeof(GlobalModule))]
    public class InputReadSystem : IInitSystem, IRunSystem
    {
        public enum StateInput
        {
            InGame,
            Blocked,
            InUI
        }

        //private EcsFilter<ButtonIconsComponent> _filterButtonIcons;
        private InputData _inputData;
        private StateInput _stateInput;

        public PlayerController_Action Control;

        public void Init()
        {
            Control = new PlayerController_Action();
            Control.Enable();
            _stateInput = StateInput.InGame;

            //EcsWorldEventsBlackboard.AddEventHandler<EventInputToGame>(SwitchInputGame);
            //EcsWorldEventsBlackboard.AddEventHandler<EventInputToUI>(SwitchInputUI);
            //EcsWorldEventsBlackboard.AddEventHandler<EventInputBlocked>(SwitchInputBlocked);

            UpdateTypeInput();
            InputUser.onChange += InputUser_onChange;
        }

        public void Run()
        {
            switch (_stateInput)
            {
                case StateInput.InGame:
                    InGameReadInput();
                    break;
                case StateInput.InUI:
                    UIReadInput();
                    break;
                case StateInput.Blocked:
                    BlockedInput();
                    break;
            }
        }
        /*
        private void SwitchInputGame(EventInputToGame _)
        {
            Time.timeScale = 1;
            _inputData.PlayerInput.SwitchCurrentActionMap("Player");
            _stateInput = StateInput.InGame;
        }

        private void SwitchInputUI(EventInputToUI _)
        {
            Time.timeScale = 0;
            _inputData.PlayerInput.SwitchCurrentActionMap("UI");
            _stateInput = StateInput.InUI;
        }

        private void SwitchInputBlocked(EventInputBlocked _)
        {
            _stateInput = StateInput.Blocked;
        }
        */
        public void RunPhysics()
        {
            if (_stateInput == StateInput.InGame)
                InGamePhysicsReadInput();
        }

        private void InputUser_onChange(InputUser arg1, InputUserChange arg2, InputDevice arg3)
        {
            UpdateTypeInput();
        }

        private void UpdateTypeInput()
        {
            if (_inputData.PlayerInput.devices.Count < 1)
                return;

            var currentDeviceRawPath = _inputData.PlayerInput.devices[0].ToString();

            if (_inputData.CurrentController == currentDeviceRawPath)
                return;

            _inputData.CurrentController = currentDeviceRawPath;
            /*
            foreach (var index in _filterButtonIcons)
            {
                ref var entity = ref _filterButtonIcons.GetEntity(index);
                entity.Replace(new ButtonIconsUpdateComponent());
            }
            */
            if (_inputData.PlayerInput.currentControlScheme == "Gamepad")
                _inputData.CurrentControllerType = TypeController.Gamepad;
            else
                _inputData.CurrentControllerType = TypeController.Keyboard;
        }

        private void InGamePhysicsReadInput()
        {
            _inputData.Move = Control.Player.Move.ReadValue<Vector2>();
        }

        private void InGameReadInput()
        {
            _inputData.MousePosition = Control.Player.MousePositions.ReadValue<Vector2>();
            _inputData.Use = Control.Player.Use.triggered;
            _inputData.Heal = Control.Player.Heal.triggered;

            _inputData.BaseAttack = Control.Player.BaseAttack.triggered;
            _inputData.BaseAttack_Hold = Control.Player.BaseAttack.ReadValue<float>() > 0;
            _inputData.AddAttack = Control.Player.AddAttack.triggered;
            _inputData.AddAttack_Hold = Control.Player.AddAttack.ReadValue<float>() > 0;
            _inputData.Dodge = Control.Player.Dodge.triggered;
            _inputData.SkillLeft = Control.Player.SkillLeft.triggered;
            _inputData.SkillRight = Control.Player.SkillRight.triggered;
            _inputData.Menu = Control.Player.Menu.triggered;
            _inputData.Map = Control.Player.Map.triggered;
        }

        private void UIReadInput()
        {
            _inputData.MousePosition = Control.UI.MousePositions.ReadValue<Vector2>();
            _inputData.Navigate = Control.UI.Navigate.ReadValue<Vector2>();
            _inputData.ScrollWheel = Control.UI.ScrollWheel.ReadValue<Vector2>();
            _inputData.ExitUI = Control.UI.Exit.triggered;
            _inputData.Cancel = Control.UI.Cancel.triggered;
            _inputData.Submit = Control.UI.Submit.triggered;
            _inputData.SwitchLeft = Control.UI.SwitchLeft.triggered;
            _inputData.SwitchRight = Control.UI.SwitchRight.triggered;
            _inputData.SwitchWeapon = Control.UI.SwitchWeapon.triggered;
            _inputData.SwitchSkill = Control.UI.SwitchSkill.triggered;
        }

        private void BlockedInput()
        {

        }
        //Заготовка для изменения инпута, по идеи этого должно быть достаточно
        private void StartRebindProcess()
        {
            var input = _inputData.PlayerInput.actions.FindAction("BaseAttack");
            _inputData.RebindOperation = input.PerformInteractiveRebinding()
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Gamepad>/Start")
                .WithControlsExcluding("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindCompleted());

            _inputData.RebindOperation.Start();
        }

        void RebindCompleted()
        {
            _inputData.RebindOperation.Dispose();
            _inputData.RebindOperation = null;
            /*
            foreach (var index in _filterButtonIcons)
            {
                ref var entity = ref _filterButtonIcons.GetEntity(index);
                entity.Replace(new ButtonIconsUpdateComponent());
            }*/
        }

        public void ResetBinding()
        {
            var input = _inputData.PlayerInput.actions;
            InputActionRebindingExtensions.RemoveAllBindingOverrides(input);
        }
    }
}