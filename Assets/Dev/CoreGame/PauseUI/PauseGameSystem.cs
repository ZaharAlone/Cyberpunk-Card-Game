using CyberNet.Core.AbilityCard;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Meta;
using Input;
using UnityEngine;

namespace CyberNet.Core.PauseUI
{
    [EcsSystem(typeof(CoreModule))]
    public class PauseGameSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _isOpenPopupExitGame;
        
        public void PreInit()
        {
            PauseGameAction.OnPauseGame += OnPauseGame;
            PauseGameAction.OffPauseGame += OffPauseGame;
            PauseGameAction.OpenPopupExitGame += OpenPopupExitGame;
        }

        public void Run()
        {
            ref var inputData = ref _dataWorld.OneData<InputData>();
            if (inputData.ExitUI)
                SwitchPauseGame();
        }
        
        private void SwitchPauseGame()
        {
            var isSelectTargetAbility = _dataWorld.Select<AbilitySelectElementComponent>().Count() > 0;
            var isSelectingAbility = _dataWorld.Select<SelectingPlayerAbilityComponent>().Count() > 0;

            if (isSelectTargetAbility || isSelectingAbility)
            {
                Debug.LogError($"select target abiblity {isSelectTargetAbility} or selecting ability {isSelectingAbility}" );
                return;
            }
            
            var isPause = _dataWorld.Select<OnPauseGameComponent>().Count() > 0;

            if (isPause)
            {
                if (_isOpenPopupExitGame)
                {
                    ClosePopupExitGame();
                }
                else
                {
                    OffPauseGame();
                }
            }
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
        
        private void OpenPopupExitGame()
        {
            _isOpenPopupExitGame = true;
        }

        private void ClosePopupExitGame()
        {
            _isOpenPopupExitGame = false;
            PopupAction.CloseConfirmPopup?.Invoke();
            PauseGameAction.ShowPanelUIPauseGame?.Invoke();
        }

        public void Destroy()
        {
            PauseGameAction.OnPauseGame -= OnPauseGame;
            PauseGameAction.OffPauseGame -= OffPauseGame;
            PauseGameAction.OpenPopupExitGame -= OpenPopupExitGame;

            var OnPauseGameQuery = _dataWorld.Select<OnPauseGameComponent>();
            if (OnPauseGameQuery.Count() > 0)
                OnPauseGameQuery.SelectFirstEntity().Destroy();
        }
    }
}