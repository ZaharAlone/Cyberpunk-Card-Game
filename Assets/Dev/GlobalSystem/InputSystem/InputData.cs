using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public enum TypeController
    {
        Keyboard,
        Gamepad
    }

    public struct InputData
    {
        public PlayerInput PlayerInput;
        public InputActionRebindingExtensions.RebindingOperation RebindOperation;

        public TypeController CurrentControllerType;
        public string CurrentController;

        public Vector2 MousePosition;
        public Vector2 Navigate;
        public Vector2 ScrollWheel;
        public bool Click;
        public bool LeftClickHold;
        public bool RightClick;
        public bool Submit;
        public bool Cancel;
        public bool ExitUI;
    }
}