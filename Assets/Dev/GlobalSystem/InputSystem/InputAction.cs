using System;
using UnityEngine;

namespace Input
{
    public static class InputAction
    {
        public static Action LeftMouseButtonClick;
        public static Action RightMouseButtonClick;

        public static Func<Vector2> GetCurrentMousePositionsToScreen;
    }
}