using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.CoreGame.TaskUI
{
    [EcsSystem(typeof(CoreModule))]
    public class TaskUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TaskUIAction.OpenTaskUI += OpenTask;
            TaskUIAction.CloseTaskUI += CloseTask;
        }
        
        private void OpenTask()
        {
            ref var taskUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TaskUIMono;
            taskUI.OpenPopup();
        }
        
        private void CloseTask()
        {
            ref var taskUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TaskUIMono;
            taskUI.ClosePopup();
        }
    }
}