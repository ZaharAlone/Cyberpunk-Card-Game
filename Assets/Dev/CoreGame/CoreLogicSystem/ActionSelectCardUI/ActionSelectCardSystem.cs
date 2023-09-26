using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Визуал выбора карты для action карты
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class ActionSelectCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ActionSelectCardAction.SelectCard += SelectCard;
            ActionSelectCardAction.CloseWindowAbilitySelectCard += CloseWindow;
            ActionSelectCardAction.OpenSelectAbilityCard += OpenWindow;
        }
        
        private void OpenWindow()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            
            if (playerComponent.playerTypeEnum != PlayerTypeEnum.Player)
                return;
            
            var entity = _dataWorld.Select<CardComponent>().With<ActionSelectCardComponent>().SelectFirstEntity();
            ref var actionSelectCardComponent = ref entity.GetComponent<ActionSelectCardComponent>();
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.actionSelectCardUIMono;
            ref var actionConfig = ref _dataWorld.OneData<ActionCardConfigData>().ActionCardViewConfig;
            actionConfig.TryGetValue(actionSelectCardComponent.AbilityCard.AbilityType.ToString(), out var actionVisualConfig);
            
            uiActionSelectCard.SetView(actionVisualConfig.HeaderLoc, actionVisualConfig.DescrLoc);
            var isEnableCancelButton = true;
            
            uiActionSelectCard.OpenWindow(isEnableCancelButton);
        }

        private void CloseWindow()
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.actionSelectCardUIMono;
            uiActionSelectCard.CloseWindow();
        }
        
        private void SelectCard()
        {
            
        }
    }
}