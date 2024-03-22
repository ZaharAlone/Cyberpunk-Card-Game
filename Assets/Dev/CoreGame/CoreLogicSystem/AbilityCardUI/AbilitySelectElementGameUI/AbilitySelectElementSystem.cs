using CyberNet.Core.Player;
using CyberNet.Core.Traderow;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using CyberNet.Core.UI.TaskPlayerPopup;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Визуал выбора карты для ability карты
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySelectElementSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _isSubscription;
        
        public void PreInit()
        {
            AbilitySelectElementUIAction.OpenSelectAbilityCard += OpenWindow;
            AbilitySelectElementUIAction.ClosePopup += CloseWindow;
            AbilitySelectElementUIAction.SelectElement += SelectElement;
        }

        private void OpenWindow(AbilityType abilityType, int indexDescr, bool basePositionFrame = true)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            if (playerComponent.playerOrAI != PlayerOrAI.Player)
                return;
            
            ref var abilityConfig = ref _dataWorld.OneData<CardsConfig>().AbilityCard;
            
            abilityConfig.TryGetValue(abilityType.ToString(), out var actionVisualConfig);

            if (indexDescr == 0)
            {
                TaskPlayerPopupAction.OpenPopup?.Invoke(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr);   
            }
            else
            {
                TaskPlayerPopupAction.OpenPopup?.Invoke(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr_2);
            }
            
            TraderowUIAction.ForceHideTraderow?.Invoke();
        }

        private void SelectElement(string textButton)
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TaskPlayerPopupUIMono;

            /*
            if (!_isSubscription)
            {
                _isSubscription = true;
                AbilitySelectElementAction.ConfimSelect += ConfimSelect;
            }*/
        }

        private void ConfimSelect()
        {
            /*
            _isSubscription = false;
            AbilitySelectElementAction.ConfimSelect -= ConfimSelect;
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            uiActionSelectCard.CloseWindow();
            AbilityCardAction.ConfimSelectElement?.Invoke();*/
        }
        
        private void CloseWindow()
        {
            TaskPlayerPopupAction.HidePopup?.Invoke();
            TraderowUIAction.ForceShowTraderow?.Invoke();
        }

        public void Destroy()
        {
            AbilitySelectElementUIAction.OpenSelectAbilityCard -= OpenWindow;
            AbilitySelectElementUIAction.ClosePopup -= CloseWindow;
            AbilitySelectElementUIAction.SelectElement -= SelectElement;
        }
    }
}