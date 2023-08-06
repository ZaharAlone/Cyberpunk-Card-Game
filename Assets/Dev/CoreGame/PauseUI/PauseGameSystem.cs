using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using Input;

namespace CyberNet.Core.PauseUI
{
    [EcsSystem(typeof(CoreModule))]
    public class PauseGameSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            PauseGameAction.OnPauseGame += OnPauseGame;
            PauseGameAction.OffPauseGame += OffPauseGame;
        }

        public void Run()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            if (inputData.ExitUI)
                SwitchPauseGame();
        }
        private void SwitchPauseGame()
        {
            var isPause = _dataWorld.Select<OnPauseGameComponent>().Count() > 0;
            
            if (isPause)
                OffPauseGame();
            else
            {
                OnPauseGame();
            }
        }

        private void OnPauseGame()
        {
            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new OnPauseGameComponent());
            //Logic
            
            PauseGameAction.OpenUIPauseGame?.Invoke();
        }
        
        private void OffPauseGame()
        {
            _dataWorld.Select<OnPauseGameComponent>().SelectFirstEntity().Destroy();
            PauseGameAction.CloseUIPauseGame?.Invoke();
            //Logic
        }
    }
}