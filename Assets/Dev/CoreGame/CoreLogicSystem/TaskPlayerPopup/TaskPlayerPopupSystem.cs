using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI.CorePopup;

namespace CyberNet.Core.UI.TaskPlayerPopup
{
    [EcsSystem(typeof(CoreModule))]
    public class TaskPlayerPopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TaskPlayerPopupAction.OpenPopupSelectFirstBase += OpenPopupSelectFirstBase;
            TaskPlayerPopupAction.HidePopup += HidePopup;
            TaskPlayerPopupAction.OpenPopup += OpenPopup;
        }
        
        private void OpenPopupSelectFirstBase()
        {
            _dataWorld.OneData<CorePopupData>().CorePopupTaskConfig.TryGetValue("popupTask_selectStartAreal", out var configPopup);
            OpenPopup(configPopup.HeaderLoc, configPopup.DescrLoc);
        }
        
        private void OpenPopup(string header, string descr)
        {
            ref var taskPlayerPopupUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TaskPlayerPopupUIMono;
            taskPlayerPopupUI.OpenWindowSetLocalizeTerm(header, descr);
        }
        
        private void HidePopup()
        {
            ref var taskPlayerPopupUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TaskPlayerPopupUIMono;
            taskPlayerPopupUI.CloseWindow();
        }

        public void Destroy()
        {
            TaskPlayerPopupAction.OpenPopupSelectFirstBase -= OpenPopupSelectFirstBase;
            TaskPlayerPopupAction.HidePopup -= HidePopup;
            TaskPlayerPopupAction.OpenPopup -= OpenPopup;
        }
    }
}