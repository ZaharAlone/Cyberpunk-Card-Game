using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Global.Cursor
{
    [EcsSystem(typeof(GlobalModule))]
    public class CustomCursorSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            CustomCursorAction.OnBaseCursor += OnBaseCursor;
            CustomCursorAction.OnAimCursor += OnAimCursor;
        }

        public void Init()
        {
            OnBaseCursor();
        }
        
        private void OnBaseCursor()
        {
            var cursorConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CursorConfigSO;
            UnityEngine.Cursor.SetCursor(cursorConfig.BaseCursorTexture, Vector2.zero, CursorMode.Auto);
        }

        private void OnAimCursor()
        {
            var cursorConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.CursorConfigSO;
            UnityEngine.Cursor.SetCursor(cursorConfig.AimCursorTexture, Vector2.zero, CursorMode.Auto);
        }

        public void Destroy()
        {
            CustomCursorAction.OnBaseCursor -= OnBaseCursor;
            CustomCursorAction.OnAimCursor -= OnAimCursor;
        }
    }
}