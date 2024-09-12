using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using Input;

namespace CyberNet.Global.Input
{
    [EcsSystem(typeof(CoreModule))]
    public class InputSystemGetData : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InputAction.GetCurrentMousePositionsToScreen += GetCurrentMousePositionsToScreen;
        }
        
        private Vector2 GetCurrentMousePositionsToScreen()
        {
            var rectTransformGlobalCanvas = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.UIRect;
            var mousePositions = _dataWorld.OneData<InputData>().MousePosition;
            
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformGlobalCanvas, mousePositions, null, out var canvasPosition))
                return canvasPosition;
            else
                return Vector2.zero;
        }

        public void Destroy()
        {
            InputAction.GetCurrentMousePositionsToScreen -= GetCurrentMousePositionsToScreen;
        }
    }
}