using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public enum TypeController
    {
        Keyboard,
        Gamepad
    }

    public class InputData
    {
        public PlayerInput PlayerInput;
        public InputActionRebindingExtensions.RebindingOperation RebindOperation;

        public TypeController CurrentControllerType;
        public string CurrentController;

        // In Game Input Element
        public Vector3 Move;
        public Vector2 MousePosition;
        public bool Heal;
        public bool Dodge;
        public bool BaseAttack;
        public bool BaseAttack_Hold;
        public bool AddAttack;
        public bool AddAttack_Hold;
        public bool SkillLeft;
        public bool SkillRight;
        public bool Menu;
        public bool Use;
        public bool Use_Hold;
        public bool Map;

        //UI Input Element
        public Vector2 Navigate;
        public Vector2 ScrollWheel;
        public bool Submit;
        public bool Cancel;
        public bool ExitUI;
        public bool SwitchLeft;
        public bool SwitchRight;
        public bool SwitchWeapon;
        public bool SwitchSkill;
    }
}