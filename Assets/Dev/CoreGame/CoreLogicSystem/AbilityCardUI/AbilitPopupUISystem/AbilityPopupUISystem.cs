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
    public class AbilityPopupUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _isSubscription;
        
        public void PreInit()
        {
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo += OpenPopup;
            AbilityPopupUISystemAction.ClosePopup += CloseWindow;
        }

        private void OpenPopup(AbilityType abilityType, int indexDescr, bool basePositionFrame = true)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            if (playerComponent.playerOrAI != PlayerOrAI.Player)
                return;
            
            var abilityConfig = _dataWorld.OneData<CardsConfigData>().AbilityCard;
            abilityConfig.TryGetValue(abilityType.ToString(), out var actionVisualConfig);

            if (indexDescr == 0)
                TaskPlayerPopupAction.OpenPopup?.Invoke(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr);   
            else
                TaskPlayerPopupAction.OpenPopup?.Invoke(actionVisualConfig.SelectFrameHeader, actionVisualConfig.SelectFrameDescr_2);
        }
        
        private void CloseWindow()
        {
            TaskPlayerPopupAction.ClosePopup?.Invoke();
            TraderowUIAction.ForceShowTraderow?.Invoke();
        }

        public void Destroy()
        {
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo -= OpenPopup;
            AbilityPopupUISystemAction.ClosePopup -= CloseWindow;
        }
    }
}